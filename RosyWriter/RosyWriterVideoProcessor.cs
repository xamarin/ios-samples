using System;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using System.IO;
using MonoTouch.CoreMedia;
using System.Collections.Generic;
using MonoTouch.AssetsLibrary;
using System.Threading.Tasks;
using MonoTouch.AudioToolbox;
using MonoTouch.CoreVideo;
using System.Runtime.InteropServices;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using MonoTouch;

namespace RosyWriter
{
	public delegate void RosyWriterVideoProcessorDelegate ();
	
	public class RosyWriterVideoProcessor : NSObject
	{
		public double VideoFrameRate{ get; private set; }
		public SizeF VideoDimensions { get; private set; }
		public uint VideoType { get; private set; }
		public AVCaptureVideoOrientation ReferenceOrientation { get; set; }
		public bool IsRecording { get; private set; }
	
		public event NSAction RecordingDidStart;
		public event NSAction RecordingWillStop;
		public event NSAction RecordingDidStop;
		public event Action<CVImageBuffer> PixelBufferReadyForDisplay;
		public event NSAction RecordingWillStart;
		
		const int BYTES_PER_PIXEL = 4;
		AVCaptureSession captureSession;
		AVCaptureConnection videoConnection;
		NSUrl movieURL;
		AVAssetWriter assetWriter;
		AVAssetWriterInput assetWriterAudioIn;
		AVAssetWriterInput assetWriterVideoIn;
	    DispatchQueue movieWritingQueue;
	    List<CMTime> previousSecondTimestamps;
		AVCaptureVideoOrientation videoOrientation;
		CMBufferQueue previewBufferQueue;
		
		// Only Accessed on movie writing Queue
		bool readyToRecordAudio;
		bool readyToRecordVideo;
		bool recordingWillBeStarted;
		bool recordingWillBeStopped;
		
		// Capture Delegates
		AVCaptureAudioDataOutputSampleBufferDelegate audioDataOutputDelegate;
		AVCaptureVideoDataOutputSampleBufferDelegate videoDataOutputDelegate;
		
		public RosyWriterVideoProcessor ()
		{
			previousSecondTimestamps = new List<CMTime> ();
			ReferenceOrientation = AVCaptureVideoOrientation.Portrait;
			
			// The temp path for the video before saving it to photo album
			movieURL = NSUrl.FromFilename (Path.Combine (Path.GetTempPath (), "Movie.MOV"));
			
			audioDataOutputDelegate = new AudioOutputDataDelegate (this);
			videoDataOutputDelegate = new VideoOutputDataDelegate (this);
		}
		
		void RemoveFile (NSUrl fileURL)
		{
			NSFileManager fileManager = NSFileManager.DefaultManager;
			NSString filePath = new NSString (fileURL.Path);
			
			if (fileManager.FileExists (filePath))
			{
				NSError error;
				bool success = fileManager.Remove (filePath, out error);
				if (!success)
					ShowError (error);
			}
		}
		
	    float AngleOffsetFromPortraitOrientationToOrientation (AVCaptureVideoOrientation orientation)
		{
			float angle = 0.0F;
			
			switch (orientation){
			case AVCaptureVideoOrientation.Portrait:
				angle = 0.0F;
				break;
			case AVCaptureVideoOrientation.LandscapeRight:
				angle = -Convert.ToSingle (Math.PI / 2.0);
				break;
			case AVCaptureVideoOrientation.LandscapeLeft:
				angle = Convert.ToSingle (Math.PI / 2.0);
				break;
			case AVCaptureVideoOrientation.PortraitUpsideDown:
				angle = Convert.ToSingle(Math.PI);
				break;
			default:
				break;
			}
			
			return angle;
		}
		
		/// <summary>
		/// Saves the movie to the camera roll.
		/// </summary>
	    void SaveMovieToCameraRoll ()
		{
			Console.WriteLine ("Save movie to camera roll");
			using (ALAssetsLibrary library = new ALAssetsLibrary ()){
				library.WriteVideoToSavedPhotosAlbum (movieURL, (NSUrl assetUrl, NSError error) => 
				{
					if (error != null)
						ShowError (error);
					else
						RemoveFile (movieURL);
													
					movieWritingQueue.DispatchAsync (() => {
						recordingWillBeStopped = false;
						this.IsRecording = false;
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
			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			
			// Calculate offsets from an arbitrary reference orientation (portrait)
			float orientationAngleOffset = AngleOffsetFromPortraitOrientationToOrientation (orientation);
			float videoOrientationAngleOffset = AngleOffsetFromPortraitOrientationToOrientation (videoOrientation);
				
			// Find the difference in angle between the passed in orientation and the current video orientation
			float angleOffset = orientationAngleOffset - videoOrientationAngleOffset;
			transform = CGAffineTransform.MakeRotation (angleOffset);
			
			return transform;
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
			Console.WriteLine ("Start Recording");
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
				if (recordingWillBeStopped || IsRecording == false)
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
		
		List<CMSampleBuffer> inuse = new List<CMSampleBuffer> ();
		
		// This is used to solve the issue with the movieWriter queue and the DisplayPixelBuffer
		// thread not releasing CMSampleBuffers when 
		void CompleteBufferUse (CMSampleBuffer buf)
		{
			lock (inuse){
				if (inuse.Contains (buf))
				{
					inuse.Remove (buf);
					buf.Dispose ();
				} else 
					inuse.Add (buf);
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
		AVCaptureDevice VideoDeviceWithPosition (AVCaptureDevicePosition position)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);
			foreach (var device in devices)
			{
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
		AVCaptureDevice AudioDevice ()
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Audio);
			if (devices.Length > 0)
				return devices [0];
			
			return null;
		}
	
	    bool SetupCaptureSession ()
		{
			Console.WriteLine ("SetupCaptureSession");
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
			var audioDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Audio); //AudioDevice ();
			if (audioDevice == null)
				return false; // e.g. simulator

			AVCaptureDeviceInput audioIn = new AVCaptureDeviceInput (audioDevice, out error);
			if (captureSession.CanAddInput (audioIn))
				captureSession.AddInput (audioIn);
			
			AVCaptureAudioDataOutput audioOut = new AVCaptureAudioDataOutput ();

			// Add the Delegate to capture each sample that comes through
			audioOut.SetSampleBufferDelegatequeue (audioDataOutputDelegate, movieWritingQueue);
			
			if (captureSession.CanAddOutput (audioOut))
				captureSession.AddOutput (audioOut);
			
			// Create Video Session
			var videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video); //VideoDeviceWithPosition (AVCaptureDevicePosition.Back);
			AVCaptureDeviceInput videoIn = new AVCaptureDeviceInput (videoDevice, out error);
			
			if (captureSession.CanAddInput (videoIn))
				captureSession.AddInput (videoIn);
			
			// RosyWriter prefers to discard late video frames early in the capture pipeline, since its
			// processing can take longer than real-time on some platforms (such as iPhone 3GS).
			// Clients whose image processing is faster than real-time should consider setting AVCaptureVideoDataOutput's
			// alwaysDiscardsLateVideoFrames property to NO.
			AVCaptureVideoDataOutput videoOut = new AVCaptureVideoDataOutput (){
				AlwaysDiscardsLateVideoFrames = true,
				VideoSettings = new AVVideoSettings (CVPixelFormatType.CV32BGRA)
			};
			
			// Create a DispatchQueue for the Video Processing
			DispatchQueue videoCaptureQueue = new DispatchQueue ("Video Capture Queue");
			videoOut.SetSampleBufferDelegateAndQueue (videoDataOutputDelegate, videoCaptureQueue);
			
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
			Console.WriteLine ("SetupAndStartCapture Session");
			
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
				//previewBufferQueue.Dispose ();
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
		
		#region Capture Delegate Classes
		class AudioOutputDataDelegate : AVCaptureAudioDataOutputSampleBufferDelegate
		{
			RosyWriterVideoProcessor processor;
			
			public AudioOutputDataDelegate (RosyWriterVideoProcessor processor)
			{
				this.processor = processor;
			}
			
			// This runs on the movieWritingQueue already
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{		
				try {
					if (processor.assetWriter != null) {
						var formatDescription = sampleBuffer.GetFormatDescription ();
						bool wasReadyToRecord = (processor.readyToRecordAudio && processor.readyToRecordVideo);
						
						// Initalize the audio input if this is not done yet
						if (!processor.readyToRecordAudio)
							processor.readyToRecordAudio = SetupAssetWriterAudioInput (formatDescription);
							
						// Write audio data to file
						if (processor.readyToRecordAudio && processor.readyToRecordVideo)
							processor.WriteSampleBuffer (sampleBuffer, AVMediaType.Audio);
			
						bool isReadyToRecord = (processor.readyToRecordAudio && processor.readyToRecordVideo);
						
						if (!wasReadyToRecord && isReadyToRecord) {
							processor.recordingWillBeStarted = false;
							processor.IsRecording = true;
							
							if (processor.RecordingDidStart != null)
								processor.RecordingDidStart ();
						}
					}
				} finally {
					sampleBuffer.Dispose();
				}
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
								
				NSDictionary audioCompressionSettings = NSDictionary.FromObjectsAndKeys (
					new NSObject[]
					{ 
						  NSNumber.FromInt32 ((int)AudioFormatType.MPEG4AAC), 
					      NSNumber.FromDouble (currentASBD.SampleRate),
						  NSNumber.FromInt32 (64000),
						  NSNumber.FromInt32 (currentASBD.ChannelsPerFrame),
						  currentChannelLayoutData
					},
					new NSObject[]
					{ 
						AVAudioSettings.AVFormatIDKey,
						AVAudioSettings.AVSampleRateKey,
						AVAudioSettings.AVEncoderBitRateKey,
						AVAudioSettings.AVNumberOfChannelsKey,
						new NSString("AVChannelLayoutKey") //AVAudioSettings.AVChannelLayoutKey,
					});
				
				if (processor.assetWriter.CanApplyOutputSettings (audioCompressionSettings, AVMediaType.Audio)){
					processor.assetWriterAudioIn = new AVAssetWriterInput (AVMediaType.Audio, audioCompressionSettings);
					processor.assetWriterAudioIn.ExpectsMediaDataInRealTime = true;
					
					if (processor.assetWriter.CanAddInput (processor.assetWriterAudioIn))
						processor.assetWriter.AddInput (processor.assetWriterAudioIn);
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
		}
		
		class VideoOutputDataDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
		{
			RosyWriterVideoProcessor processor;
			
			public VideoOutputDataDelegate (RosyWriterVideoProcessor processor)
			{
				Console.WriteLine ("Creating VideoOutputDataDelegate");
				this.processor = processor;
			}
			
			[DllImportAttribute (Constants.CoreFoundationLibrary)]
			extern static IntPtr CFCopyDescription (IntPtr obj);
			
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				CMFormatDescription formatDescription = sampleBuffer.GetFormatDescription ();

				if (connection == processor.videoConnection) {
					// Get framerate
					CMTime timestamp = sampleBuffer.PresentationTimeStamp;
					CalculateFramerateAtTimestamp (timestamp);			
						
					// Get frame dimensions (for onscreen display)
					if (processor.VideoDimensions.Width == 0 && processor.VideoDimensions.Height == 0)
						processor.VideoDimensions = formatDescription.GetVideoPresentationDimensions (true, false);
						
					// Get the buffer type
					if (processor.VideoType == 0)
						processor.VideoType = formatDescription.MediaSubType;
					// TODO: processor.VideoType = (CMVideoCodecType)Enum.ToObject (typeof(CMVideoCodecType), formatDescription.MediaSubType);
					
					// Synchronously process the pixel buffer to de-green it.
					using (var pixelBuffer = sampleBuffer.GetImageBuffer ())
						ProcessPixelBuffer (pixelBuffer);

					processor.previewBufferQueue.Enqueue (sampleBuffer);
						
					//var writeBuffer = sampleBuffer.Duplicate ();
					InvokeOnMainThread (() => {
						var j = processor.previewBufferQueue.Dequeue ();
				
						var sbuf = j as CMSampleBuffer;
						if (sbuf == null) {
							// Record the current sampleBuffer.ClassHandle
							// Then run another iteration and on the next one, print the ClassHandle
							Console.WriteLine ("The type is {0}", new NSString (CFCopyDescription (j.Handle)));
							return;
						}
						
						using (CVImageBuffer pixBuf = sbuf.GetImageBuffer ()){
							if (processor.PixelBufferReadyForDisplay != null)
								processor.PixelBufferReadyForDisplay (pixBuf);
						}
		
						if(processor.assetWriter == null)
							sbuf.Dispose();
						else
							processor.CompleteBufferUse (sbuf);
					});
				}
				
				
				processor.movieWritingQueue.DispatchAsync (() => {
					if (processor.assetWriter != null) {
						bool wasReadyToRecord = (processor.readyToRecordAudio && processor.readyToRecordVideo);
					
						// Initialize the video input if this is not done yet
						if (!processor.readyToRecordVideo)
							processor.readyToRecordVideo = SetupAssetWriterVideoInput (formatDescription);
						
						// Write the video data to file
						if (processor.readyToRecordVideo && processor.readyToRecordAudio)
							processor.WriteSampleBuffer (sampleBuffer, AVMediaType.Video);
			
						bool isReadyToRecord = (processor.readyToRecordAudio && processor.readyToRecordVideo);
					
						if (!wasReadyToRecord && isReadyToRecord) {
							processor.recordingWillBeStarted = false;
							processor.IsRecording = true;
						
							if (processor.RecordingDidStart != null)
								processor.RecordingDidStart ();
						}
						
						processor.CompleteBufferUse (sampleBuffer);
					}
				});	
			}
			
			public bool SetupAssetWriterVideoInput (CMFormatDescription currentFormatDescription)
			{
				Console.WriteLine ("Setting up Video Asset Writer");
				float bitsPerPixel;
				var dimensions = currentFormatDescription.VideoDimensions;
				int numPixels = dimensions.Width * dimensions.Height;
				int bitsPerSecond; 
				
				// Assume that lower-than-SD resolution are intended for streaming, and use a lower bitrate
				if (numPixels < (640 * 480))
					bitsPerPixel = 4.05F; // This bitrate matches the quality produced by AVCaptureSessionPresetMedium or Low.
				else
					bitsPerPixel = 11.4F; // This bitrate matches the quality produced by AVCaptureSessionPresetHigh.	
				
				bitsPerSecond = Convert.ToInt32 ((float)numPixels * bitsPerPixel);
				
				NSDictionary videoCompressionSettings = NSDictionary.FromObjectsAndKeys (
					new NSObject[] 
					{   // The Compression Settings Values
						AVVideo.CodecH264,
						NSNumber.FromInt32 (dimensions.Width),
						NSNumber.FromInt32 (dimensions.Height),
						NSDictionary.FromObjectsAndKeys (
							new object[] 
							{	// Compression Property Values
								NSNumber.FromInt32 (bitsPerSecond),
								NSNumber.FromInt32 (30)
							},
							new object[]
							{	// Compression Property Keys
								AVVideo.AverageBitRateKey,
								AVVideo.MaxKeyFrameIntervalKey
							})
					},
					new NSObject[]
					{	// The Compression Settings Keys
						AVVideo.CodecKey,
						AVVideo.WidthKey,
						AVVideo.HeightKey,
						AVVideo.CompressionPropertiesKey
					}
					);
				
				if (processor.assetWriter.CanApplyOutputSettings (videoCompressionSettings, AVMediaType.Video)){
					processor.assetWriterVideoIn = new AVAssetWriterInput (AVMediaType.Video, videoCompressionSettings);
					processor.assetWriterVideoIn.ExpectsMediaDataInRealTime = true;
					processor.assetWriterVideoIn.Transform = processor.TransformFromCurrentVideoOrientationToOrientation (processor.ReferenceOrientation);
					
					if (processor.assetWriter.CanAddInput (processor.assetWriterVideoIn))
						processor.assetWriter.AddInput (processor.assetWriterVideoIn);
					else {
						Console.WriteLine ("Couldn't add asset writer video input.");
						return false;
					}
				} else 
					Console.WriteLine ("Couldn't apply video output settings.");	
				
				return true;
			}
			
			public void CalculateFramerateAtTimestamp (CMTime timeStamp)
			{
				processor.previousSecondTimestamps.Add (timeStamp);
				
				var oneSecond = CMTime.FromSeconds (1, 1);
				var oneSecondAgo = CMTime.Subtract (timeStamp, oneSecond);
				
				while (processor.previousSecondTimestamps.Count > 0 && CMTime.Compare(processor.previousSecondTimestamps[0], oneSecondAgo) < 0)
					processor.previousSecondTimestamps.RemoveAt (0);
				
				double newRate = Convert.ToDouble (processor.previousSecondTimestamps.Count);
				
				processor.VideoFrameRate = (processor.VideoFrameRate + newRate) / 2;
			}
			
			public unsafe void ProcessPixelBuffer (CVImageBuffer imageBuffer)
			{
				using (CVPixelBuffer pixelBuffer = imageBuffer as CVPixelBuffer)
				{
					pixelBuffer.Lock (CVOptionFlags.None);
					
					int bufferWidth = pixelBuffer.Width;
					int bufferHeight = pixelBuffer.Height;
					byte* pixelPtr = (byte*)pixelBuffer.BaseAddress.ToPointer();
					
					int position = 0;
					for (var row = 0; row < bufferHeight; row++){
						for (var column = 0; column < bufferWidth; column++) {
							// De-green (Second pixel in BGRA is green)
							*(pixelPtr+1) = 0;
							pixelPtr += BYTES_PER_PIXEL;
							position += BYTES_PER_PIXEL; // For each pixel increase the offset by the number of bytes per pixel
						}
					}
					
					pixelBuffer.Unlock (CVOptionFlags.None);
				}
			}
		}
		#endregion
	}
}

