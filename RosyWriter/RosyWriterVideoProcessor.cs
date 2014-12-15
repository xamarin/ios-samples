using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CoreGraphics;
using AVFoundation;
using AssetsLibrary;
using AudioToolbox;
using CoreFoundation;
using CoreMedia;
using CoreVideo;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace RosyWriter
{
	public delegate void RosyWriterVideoProcessorDelegate ();

	public class RosyWriterVideoProcessor : NSObject, IAVCaptureVideoDataOutputSampleBufferDelegate, IAVCaptureAudioDataOutputSampleBufferDelegate
	{
		public double VideoFrameRate{ get; private set; }
		public CGSize VideoDimensions { get; private set; }
		public uint VideoType { get; private set; }
		public AVCaptureVideoOrientation ReferenceOrientation { get; set; }
		public bool IsRecording { get; private set; }

		public event Action RecordingDidStart;
		public event Action RecordingWillStop;
		public event Action RecordingDidStop;
		public event Action<CVImageBuffer> PixelBufferReadyForDisplay;
		public event Action RecordingWillStart;

		const int BYTES_PER_PIXEL = 4;
		AVCaptureSession captureSession;
		AVCaptureConnection videoConnection;
		AVCaptureConnection audioConnection;
		readonly NSUrl movieURL;
		AVAssetWriter assetWriter;
		AVAssetWriterInput assetWriterAudioIn;
		AVAssetWriterInput assetWriterVideoIn;
	    DispatchQueue movieWritingQueue;
	    readonly List<CMTime> previousSecondTimestamps;
		AVCaptureVideoOrientation videoOrientation;
		CMBufferQueue previewBufferQueue;

		// Only Accessed on movie writing Queue
		bool readyToRecordAudio;
		bool readyToRecordVideo;
		bool recordingWillBeStarted;
		bool recordingWillBeStopped;

		public RosyWriterVideoProcessor ()
		{
			previousSecondTimestamps = new List<CMTime> ();
			ReferenceOrientation = AVCaptureVideoOrientation.Portrait;

			// The temp path for the video before saving it to photo album
			movieURL = NSUrl.FromFilename (Path.Combine (Path.GetTempPath (), "Movie.MOV"));
		}

		static void RemoveFile (NSUrl fileURL)
		{
			var filePath = fileURL.Path;
			if (File.Exists (filePath))
				File.Delete (filePath);
		}

	    static float AngleOffsetFromPortraitOrientationToOrientation (AVCaptureVideoOrientation orientation)
		{
			switch (orientation) {
			case AVCaptureVideoOrientation.LandscapeRight:
				return (float) (-Math.PI / 2.0);
			case AVCaptureVideoOrientation.LandscapeLeft:
				return (float) (Math.PI / 2.0);
			case AVCaptureVideoOrientation.PortraitUpsideDown:
				return (float) Math.PI;
			//case AVCaptureVideoOrientation.Portrait:
			default:
				return 0.0f;
			}
		}

		/// <summary>
		/// Saves the movie to the camera roll.
		/// </summary>
	    void SaveMovieToCameraRoll ()
		{
			//Console.WriteLine ("Save movie to camera roll");
			using (var library = new ALAssetsLibrary ()) {
				library.WriteVideoToSavedPhotosAlbum (movieURL, (assetUrl, error) => {
					if (error != null)
						ShowError (error);
					else
						RemoveFile (movieURL);

					movieWritingQueue.DispatchAsync (() => {
						recordingWillBeStopped = false;
						IsRecording = false;
						if (RecordingDidStop != null)
							RecordingDidStop ();
					});
				});
			}
		}

		/// <summary>
		/// Creates a Transform to apply to the OpenGL view to display the Video at the proper orientation
		/// </summary>
		/// <returns>
		/// A transform to correct the orientation
		/// </returns>
		/// <param name='orientation'>
		/// Current Orientation
		/// </param>
		public CGAffineTransform TransformFromCurrentVideoOrientationToOrientation (AVCaptureVideoOrientation orientation)
		{
			// Calculate offsets from an arbitrary reference orientation (portrait)
			float orientationAngleOffset = AngleOffsetFromPortraitOrientationToOrientation (orientation);
			float videoOrientationAngleOffset = AngleOffsetFromPortraitOrientationToOrientation (videoOrientation);

			// Find the difference in angle between the passed in orientation and the current video orientation
			float angleOffset = orientationAngleOffset - videoOrientationAngleOffset;
			return CGAffineTransform.MakeRotation (angleOffset);
		}

		void WriteSampleBuffer (CMSampleBuffer sampleBuffer, NSString mediaType)
		{
			if (assetWriter.Status == AVAssetWriterStatus.Unknown){
				if (assetWriter.StartWriting ())
					assetWriter.StartSessionAtSourceTime (sampleBuffer.OutputPresentationTimeStamp);
				else
					ShowError (assetWriter.Error);
			}

			if (assetWriter.Status == AVAssetWriterStatus.Writing){
				if (mediaType == AVMediaType.Video){
					if (assetWriterVideoIn.ReadyForMoreMediaData){
						if (!assetWriterVideoIn.AppendSampleBuffer (sampleBuffer))
							ShowError (assetWriter.Error);
					}
				} else if (mediaType == AVMediaType.Audio){
					if (assetWriterAudioIn.ReadyForMoreMediaData){
						if (!assetWriterAudioIn.AppendSampleBuffer (sampleBuffer))
							ShowError (assetWriter.Error);
					}
				}
			}
		}

		#region Recording
		/// <summary>
		/// Starts the recording.
		/// </summary>
		public void StartRecording ()
		{
			//Console.WriteLine ("Start Recording");
			movieWritingQueue.DispatchAsync (() =>
			{
				if (recordingWillBeStarted || IsRecording)
					return;

				recordingWillBeStarted = true;

				// recordingDidStart is called from captureOutput.DidOutputSampleBuffer.FromeConnection one the asset writere is setup
				if (RecordingWillStart != null)
					RecordingWillStart ();

				// Remove the file if one with the same name already exists
				RemoveFile (movieURL);

				// Create an asset writer
				NSError error;
				assetWriter = new AVAssetWriter (movieURL, AVFileType.QuickTimeMovie, out error);
				if (error != null)
					ShowError (error);
			});
		}

		public void StopRecording ()
		{
			movieWritingQueue.DispatchAsync (() =>
			{
				if (recordingWillBeStopped || !IsRecording)
					return;

				recordingWillBeStopped = true;

				// recordingDidStop is called from saveMovieToCameraRoll
				if (RecordingWillStop != null)
					RecordingWillStop ();

				if (assetWriter.FinishWriting ()){
					if (assetWriterAudioIn != null)
						assetWriterAudioIn.Dispose ();
					if (assetWriterVideoIn != null)
						assetWriterVideoIn.Dispose ();

					lock(inuse){
						assetWriter.Dispose ();
						assetWriter = null;

						// Clear the 'Inuse' list when we're creating a new Recording session.
						inuse.Clear();
					}

					readyToRecordVideo = false;
					readyToRecordAudio = false;

					SaveMovieToCameraRoll ();
				} else
					ShowError (assetWriter.Error);
			});
		}

		#endregion

		#region Capture

		readonly List<CMSampleBuffer> inuse = new List<CMSampleBuffer> ();

		// This is used to solve the issue with the movieWriter queue and the DisplayPixelBuffer
		// thread not releasing CMSampleBuffers when
		void CompleteBufferUse (CMSampleBuffer buf)
		{
			lock (inuse){
				if (inuse.Contains (buf)) {
					inuse.Remove (buf);
					buf.Dispose ();
				} else {
					inuse.Add (buf);
				}
			}
		}

		/// <summary>
		/// Videos the device available for passed in position.
		/// </summary>
		/// <returns>
		/// The available device
		/// </returns>
		/// <param name='position'>
		/// The desired Position.
		/// </param>
		static AVCaptureDevice VideoDeviceWithPosition (AVCaptureDevicePosition position)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);
			foreach (var device in devices) {
				if (device.Position == position)
					return device;
			}
			return null;
		}

		/// <summary>
		/// Returns an audio device
		/// </summary>
		/// <returns>
		/// The audio device.
		/// </returns>
		static AVCaptureDevice AudioDevice ()
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Audio);
			return (devices.Length == 0) ? null : devices [0];
		}

	    bool SetupCaptureSession ()
		{
			//Console.WriteLine ("SetupCaptureSession");
			// Overview: RosyWriter uses separate GCD queues for audio and video capture.  If a single GCD queue
			// is used to deliver both audio and video buffers, and our video processing consistently takes
			// too long, the delivery queue can back up, resulting in audio being dropped.
			//
			// When recording, RosyWriter creates a third GCD queue for calls to AVAssetWriter.  This ensures
			// that AVAssetWriter is not called to start or finish writing from multiple threads simultaneously.
			//
			// RosyWriter uses AVCaptureSession's default preset, AVCaptureSessionPresetHigh.

			// Create Capture session
			captureSession = new AVCaptureSession ();
			captureSession.BeginConfiguration ();

			// Create audio connection
			NSError error;
			var audioDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Audio);
			if (audioDevice == null)
				return false; // e.g. simulator

			var audioIn = new AVCaptureDeviceInput (audioDevice, out error);
			if (captureSession.CanAddInput (audioIn))
				captureSession.AddInput (audioIn);

			var audioOut = new AVCaptureAudioDataOutput ();
			var audioCaptureQueue = new DispatchQueue ("Audio Capture Queue");

			// Add the Delegate to capture each sample that comes through
			audioOut.SetSampleBufferDelegateQueue (this, audioCaptureQueue);

			if (captureSession.CanAddOutput (audioOut))
				captureSession.AddOutput (audioOut);

			audioConnection = audioOut.ConnectionFromMediaType (AVMediaType.Audio);

			// Create Video Session
			var videoDevice = VideoDeviceWithPosition (AVCaptureDevicePosition.Back);
			var videoIn = new AVCaptureDeviceInput (videoDevice, out error);

			if (captureSession.CanAddInput (videoIn))
				captureSession.AddInput (videoIn);

			// RosyWriter prefers to discard late video frames early in the capture pipeline, since its
			// processing can take longer than real-time on some platforms (such as iPhone 3GS).
			// Clients whose image processing is faster than real-time should consider setting AVCaptureVideoDataOutput's
			// alwaysDiscardsLateVideoFrames property to NO.
			var videoOut = new AVCaptureVideoDataOutput {
				AlwaysDiscardsLateVideoFrames = true,
				// HACK: Change VideoSettings to WeakVideoSettings, and AVVideoSettings to CVPixelBufferAttributes
				// VideoSettings = new AVVideoSettings (CVPixelFormatType.CV32BGRA)
				WeakVideoSettings = new CVPixelBufferAttributes () {
					PixelFormatType = CVPixelFormatType.CV32BGRA
				}.Dictionary
			};

			// Create a DispatchQueue for the Video Processing
			var videoCaptureQueue = new DispatchQueue ("Video Capture Queue");
			videoOut.SetSampleBufferDelegateQueue (this, videoCaptureQueue);

			if (captureSession.CanAddOutput (videoOut))
				captureSession.AddOutput (videoOut);

			// Set the Video connection from the Video Output object
			videoConnection = videoOut.ConnectionFromMediaType (AVMediaType.Video);
			videoOrientation = videoConnection.VideoOrientation;

			captureSession.CommitConfiguration ();

			return true;
		}

		public void SetupAndStartCaptureSession ()
		{
			//Console.WriteLine ("SetupAndStartCapture Session");

			// Create a shallow queue for buffers going to the display for preview.
			previewBufferQueue = CMBufferQueue.CreateUnsorted (1);

			// Create serial queue for movie writing
			movieWritingQueue = new DispatchQueue ("Movie Writing Queue");

			if (captureSession == null)
				SetupCaptureSession ();

			NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.DidStopRunningNotification, CaptureSessionStoppedRunningNotification, captureSession);

			if (!captureSession.Running)
				captureSession.StartRunning ();
		}

		public void CaptureSessionStoppedRunningNotification (NSNotification notification)
		{
			movieWritingQueue.DispatchAsync (() => {
				if (IsRecording)
					StopRecording ();
			});
		}

		public void PauseCaptureSession ()
		{
			if (captureSession.Running)
				captureSession.StopRunning ();
		}

		public void ResumeCaptureSession ()
		{
			if (!captureSession.Running)
				captureSession.StartRunning ();
		}

		/// <summary>
		/// Stops the and tears down the capture session.
		/// </summary>
		public void StopAndTearDownCaptureSession ()
		{
			captureSession.StopRunning ();
			if (captureSession != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver (this, AVCaptureSession.DidStopRunningNotification, captureSession);
			captureSession.Dispose ();
			captureSession = null;

			if (previewBufferQueue != null){
				previewBufferQueue.Dispose ();
				previewBufferQueue = null;
			}

			if (movieWritingQueue != null){
				movieWritingQueue.Dispose ();
				movieWritingQueue = null;
			}
		}

		#endregion

		public void ShowError (NSError error)
		{
			InvokeOnMainThread (() => {
				using (var alertView = new UIAlertView (error.LocalizedDescription, error.ToString (), null, "OK", null))
					alertView.Show ();
			});
		}

		#region AVCapture[Audio|Video]DataOutputSampleBufferDelegate

		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		public virtual void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			// HACK: Change CMSampleBuffer.GetFormatDescription() to CMSampleBuffer.GetVideoFormatDescription()
			// HACK Change CMFormatDescription to CMVideoFormatDescription
			// CMFormatDescription formatDescription = sampleBuffer.GetFormatDescription ();
			CMVideoFormatDescription formatDescription = sampleBuffer.GetVideoFormatDescription ();

			if (connection == videoConnection) {
				// Get framerate
				CMTime timestamp = sampleBuffer.PresentationTimeStamp;
				CalculateFramerateAtTimestamp (timestamp);

				// Get frame dimensions (for onscreen display)
				if (VideoDimensions.IsEmpty)
					// HACK: Change GetVideoPresentationDimensions() to GetPresentationDimensions()
					// VideoDimensions = formatDescription.GetVideoPresentationDimensions (true, false);
					VideoDimensions = formatDescription.GetPresentationDimensions (true, false);

				// Get the buffer type
				if (VideoType == 0)
					VideoType = formatDescription.MediaSubType;

				// Synchronously process the pixel buffer to de-green it.
				using (var pixelBuffer = sampleBuffer.GetImageBuffer ())
					ProcessPixelBuffer (pixelBuffer);

				previewBufferQueue.Enqueue (sampleBuffer);

				//var writeBuffer = sampleBuffer.Duplicate ();
				InvokeOnMainThread (() => {
					INativeObject j = previewBufferQueue.Dequeue ();

					var sbuf = j as CMSampleBuffer;
					if (sbuf == null) {
#if DEBUG
						// Record the current sampleBuffer.ClassHandle
						// Then run another iteration and on the next one, print the ClassHandle
						Console.WriteLine ("The type is {0}", j.ToString());
#endif
						return;
					}

					using (CVImageBuffer pixBuf = sbuf.GetImageBuffer ()) {
						if (PixelBufferReadyForDisplay != null)
							PixelBufferReadyForDisplay (pixBuf);
					}
				});
			}
			// keep a reference to 'sampleBuffer', movieWritingQueue will remove it
			CompleteBufferUse (sampleBuffer);

			movieWritingQueue.DispatchAsync (() => {
				if (assetWriter != null) {
					bool wasReadyToRecord = (readyToRecordAudio && readyToRecordVideo);

					if (connection == videoConnection) {
						// Initialize the video input if this is not done yet
						if (!readyToRecordVideo)
							readyToRecordVideo = SetupAssetWriterVideoInput (formatDescription);

						// Write the video data to file
						if (readyToRecordVideo && readyToRecordAudio) {
							WriteSampleBuffer (sampleBuffer, AVMediaType.Video);
						}
					} else if (connection == audioConnection) {
						if (!readyToRecordAudio)
							readyToRecordAudio = SetupAssetWriterAudioInput (formatDescription);

						if (readyToRecordAudio && readyToRecordVideo)
							WriteSampleBuffer (sampleBuffer, AVMediaType.Audio);
					}
					bool isReadyToRecord = (readyToRecordAudio && readyToRecordVideo);

					if (!wasReadyToRecord && isReadyToRecord) {
						recordingWillBeStarted = false;
						IsRecording = true;

						if (RecordingDidStart != null)
							RecordingDidStart ();
					}
				}
				CompleteBufferUse (sampleBuffer);
			});
		}
		// HACK: Change CMFormatDescription to CMVideoFormatDescription
		public bool SetupAssetWriterVideoInput (CMVideoFormatDescription currentFormatDescription)
		{
			//Console.WriteLine ("Setting up Video Asset Writer");
			float bitsPerPixel;
			// HACK: Change VideoDimensions to Dimensions, as this type was changed to CMVideoFormatDescription
			var dimensions = currentFormatDescription.Dimensions;
			int numPixels = dimensions.Width * dimensions.Height;
			int bitsPerSecond;

			// Assume that lower-than-SD resolution are intended for streaming, and use a lower bitrate
			bitsPerPixel = numPixels < (640 * 480) ? 4.05F : 11.4F;

			bitsPerSecond = (int) (numPixels * bitsPerPixel);

			NSDictionary videoCompressionSettings = new NSDictionary (
				AVVideo.CodecKey, AVVideo.CodecH264,
				AVVideo.WidthKey, dimensions.Width,
				AVVideo.HeightKey,dimensions.Height,
				AVVideo.CompressionPropertiesKey, new NSDictionary(
					AVVideo.AverageBitRateKey, bitsPerSecond,
					AVVideo.MaxKeyFrameIntervalKey, 30
				)
			);

			if (assetWriter.CanApplyOutputSettings (videoCompressionSettings, AVMediaType.Video)){
				// HACK: Change NSDictionary into AVVideoSettingsCompressed created using that NSDictionary (videoCompressionSettings)
				assetWriterVideoIn = new AVAssetWriterInput (AVMediaType.Video, new AVVideoSettingsCompressed( videoCompressionSettings));
				assetWriterVideoIn.ExpectsMediaDataInRealTime = true;
				assetWriterVideoIn.Transform = TransformFromCurrentVideoOrientationToOrientation (ReferenceOrientation);

				if (assetWriter.CanAddInput (assetWriterVideoIn))
					assetWriter.AddInput (assetWriterVideoIn);
				else {
					Console.WriteLine ("Couldn't add asset writer video input.");
					return false;
				}
			} else
				Console.WriteLine ("Couldn't apply video output settings.");

			return true;
		}

		public bool SetupAssetWriterAudioInput (CMFormatDescription currentFormatDescription)
		{
			// If the AudioStreamBasicDescription is null return false;
			if (!currentFormatDescription.AudioStreamBasicDescription.HasValue)
				return false;

			var currentASBD = currentFormatDescription.AudioStreamBasicDescription.Value;

			// Get the Audio Channel Layout from the Format Description.
			var currentChannelLayout = currentFormatDescription.AudioChannelLayout;
			var currentChannelLayoutData = currentChannelLayout == null ? new NSData () : currentChannelLayout.AsData ();

			NSDictionary audioCompressionSettings = new NSDictionary (
				AVAudioSettings.AVFormatIDKey, AudioFormatType.MPEG4AAC,
				AVAudioSettings.AVSampleRateKey, currentASBD.SampleRate,
				AVAudioSettings.AVEncoderBitRateKey, 64000,
				AVAudioSettings.AVNumberOfChannelsKey, currentASBD.ChannelsPerFrame,
				AVAudioSettings.AVChannelLayoutKey, currentChannelLayoutData
			);

			if (assetWriter.CanApplyOutputSettings (audioCompressionSettings, AVMediaType.Audio)){
				// HACK: Change NSDictionary into AudioSettings created using that NSDictionary (audioCompressionSettings)
				assetWriterAudioIn = new AVAssetWriterInput (AVMediaType.Audio, new AudioSettings(audioCompressionSettings));
				assetWriterAudioIn.ExpectsMediaDataInRealTime = true;

				if (assetWriter.CanAddInput (assetWriterAudioIn))
					assetWriter.AddInput (assetWriterAudioIn);
				else {
					Console.WriteLine ("Couldn't add asset writer audio input.");
					return false;
				}
			} else {
				Console.WriteLine ("Couldn't apply audio output settings.");
				return false;
			}

			return true;
		}

		public void CalculateFramerateAtTimestamp (CMTime timeStamp)
		{
			previousSecondTimestamps.Add (timeStamp);

			var oneSecond = CMTime.FromSeconds (1, 1);
			var oneSecondAgo = CMTime.Subtract (timeStamp, oneSecond);

			while (previousSecondTimestamps.Count > 0 && CMTime.Compare (previousSecondTimestamps[0], oneSecondAgo) < 0)
				previousSecondTimestamps.RemoveAt (0);

			double newRate = Convert.ToDouble (previousSecondTimestamps.Count);

			VideoFrameRate = (VideoFrameRate + newRate) / 2;
		}

		public unsafe void ProcessPixelBuffer (CVImageBuffer imageBuffer)
		{
			using (var pixelBuffer = imageBuffer as CVPixelBuffer)
			{
				pixelBuffer.Lock (CVOptionFlags.None);

				//HACK: Cast nint to int
				int bufferWidth = (int)pixelBuffer.Width;
				int bufferHeight = (int)pixelBuffer.Height;
				// offset by one to de-green the BGRA array (green is second)
				byte* pixelPtr = (byte*)pixelBuffer.BaseAddress.ToPointer () + 1;

				for (var row = 0; row < bufferHeight; row++){
					for (var column = 0; column < bufferWidth; column++) {
						*pixelPtr = 0;
						pixelPtr += BYTES_PER_PIXEL;
					}
				}

				pixelBuffer.Unlock (CVOptionFlags.None);
			}
		}
	}
	#endregion
}
