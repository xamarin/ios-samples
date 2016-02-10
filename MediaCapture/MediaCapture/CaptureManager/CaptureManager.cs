using System;
using System.IO;

using Foundation;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreFoundation;

namespace MediaCapture {
	public class CaptureManager {
		// capture session
		AVCaptureSession session = null;
		bool isCapturing = false;
		bool captureImages = false;
		bool captureAudio = false;
		bool captureVideo = false;
		string movieRecordingDirectory;
		CameraType cameraType = CameraType.RearFacing;
		Resolution resolution = Resolution.Medium;

		// camera input objects
		AVCaptureDevice videoCaptureDevice = null;
		AVCaptureDeviceInput videoInput = null;

		// microphone input objects
		AVCaptureDevice audioCaptureDevice = null;
		AVCaptureDeviceInput audioInput = null;

		// frame grabber objects
		AVCaptureVideoDataOutput frameGrabberOutput = null;
		VideoFrameSamplerDelegate videoFrameSampler = null;
		DispatchQueue queue = null;

		// movie recorder objects
		AVCaptureMovieFileOutput movieFileOutput = null;
		MovieSegmentWriterDelegate movieSegmentWriter = null;
		string currentSegmentFile = null;
		DateTime currentSegmentStartedAt;
		uint nextMovieIndex = 1;
		int movieSegmentDurationInMilliSeconds = 20000;
		bool breakMovieIntoSegments = true;

		CaptureManager ()
		{
		}

		public CaptureManager (Resolution resolution, bool captureImages, bool captureAudio, bool captureVideo, CameraType cameraType, string movieRecordingDirectory, int movieSegmentDurationInMilliSeconds, bool breakMovieIntoSegments )
		{
			this.resolution = resolution;
			this.captureImages = captureImages;
			this.captureAudio = captureAudio;
			this.captureVideo = captureVideo;
			this.cameraType = cameraType;
			this.movieSegmentDurationInMilliSeconds = movieSegmentDurationInMilliSeconds;
			this.breakMovieIntoSegments = breakMovieIntoSegments;
			if ( captureAudio || captureVideo ) {
				this.movieRecordingDirectory =  Path.Combine (movieRecordingDirectory, GetDateTimeDirectoryName (DateTime.Now));
				if (Directory.Exists( this.movieRecordingDirectory ) == false)
					Directory.CreateDirectory( this.movieRecordingDirectory );
			}
		}

		#region events
		public EventHandler<MovieSegmentCapturedEventArgs> MovieSegmentCaptured;
		void OnMovieSegmentCaptured( MovieSegmentCapturedEventArgs args )
		{
			if (MovieSegmentCaptured != null)
				MovieSegmentCaptured (this, args);
		}

		public EventHandler<CaptureErrorEventArgs> CaptureError;
		void OnCaptureError( CaptureErrorEventArgs args )
		{
			if (CaptureError != null )
				CaptureError (this, args);
		}

		public EventHandler<ImageCaptureEventArgs> ImageCaptured;
		void OnImageCaptured( ImageCaptureEventArgs args )
		{
			if (ImageCaptured != null)
				ImageCaptured (this, args);
		}
		#endregion

		public int MovieSegmentDurationInMilliSeconds {
			get {
				return movieSegmentDurationInMilliSeconds;
			}
		}

		public bool IsCapturing {
			get {
				return isCapturing;
			}
			set {
				isCapturing = value;
			}
		}

		bool shouldRecord {
			get {
				return ((captureAudio || captureVideo ) && (string.IsNullOrEmpty (movieRecordingDirectory) == false));
			}
		}

		public bool StartCapture( out string message )
		{
			message = string.Empty;
			if ( isCapturing == true ) {
				message = "already capturing";
				return true;
			}

			isCapturing = true;
			if (SetupCaptureSessionInternal (out message) == false)
				return false;

			// start the capture
			session.StartRunning ();

			// start recording (if configured)
			if (shouldRecord)
				StartMovieWriter ();

			return true;
		}

		public void StopCapture()
		{
			if (isCapturing == false)
				return;

			isCapturing = false;

			// stop recording
			if (shouldRecord)
				StopMovieWriter ();

			// stop the capture session
			session.StopRunning ();

			UnsubscribeDelegateEvents ();
		}

		void StartMovieWriter ()
		{
			if (movieFileOutput == null)
				return;

			StartRecordingNextMovieFilename ();
		}

		void StartRecordingNextMovieFilename ()
		{
			// generate file name
			currentSegmentFile = Path.Combine (movieRecordingDirectory, string.Format ("video_{0}.mov", nextMovieIndex++));
			NSUrl segmentUrl = NSUrl.FromFilename (currentSegmentFile);

			// start recording
			movieFileOutput.StartRecordingToOutputFile (segmentUrl, movieSegmentWriter);
		}

		void StopMovieWriter()
		{
			if (movieFileOutput == null)
				return;

			movieFileOutput.StopRecording ();
		}

		bool SetupCaptureSessionInternal (out string errorMessage)
		{
			errorMessage = string.Empty;

			// create the capture session
			session = new AVCaptureSession ();
			switch (resolution) {
				case Resolution.Low:
					session.SessionPreset = AVCaptureSession.PresetLow;
					break;
				case Resolution.High:
					session.SessionPreset = AVCaptureSession.PresetHigh;
					break;
				default:
					session.SessionPreset = AVCaptureSession.PresetMedium;
					break;
			}

			// conditionally configure the camera input
			if ((captureVideo || captureImages) && AddCameraInput (out errorMessage) == false)
				return false;

			// conditionally configure the microphone input
			if ( captureAudio && AddAudioInput (out errorMessage) == false)
				return false;

			// conditionally configure the sample buffer output
			if (captureImages) {
				int minimumSampleIntervalInMilliSeconds = captureVideo ? 1000 : 100;
				if (AddImageSamplerOutput (out errorMessage, minimumSampleIntervalInMilliSeconds) == false) {
					return false;
				}
			}

			// conditionally configure the movie file output
			if (shouldRecord && AddMovieFileOutput( out errorMessage ) == false)
				return false;

			return true;
		}

		bool AddCameraInput (out string errorMessage)
		{
			errorMessage = string.Empty;
			videoCaptureDevice = cameraType == CameraType.FrontFacing ? MediaDevices.FrontCamera : MediaDevices.BackCamera;
			videoInput = AVCaptureDeviceInput.FromDevice(videoCaptureDevice);
			if (videoInput == null) {
				errorMessage = "No video capture device";
				return false;
			}

			session.AddInput (videoInput);
			return true;
		}

		bool AddAudioInput (out string errorMessage)
		{
			errorMessage = string.Empty;
			audioCaptureDevice = MediaDevices.Microphone;
			audioInput = AVCaptureDeviceInput.FromDevice (audioCaptureDevice);
			if (audioInput == null) {
				errorMessage = "No audio capture device";
				return false;
			}
			session.AddInput (audioInput);
			return true;
		}

		bool AddMovieFileOutput (out string errorMessage)
		{
			errorMessage = string.Empty;

			// create a movie file output and add it to the capture session
			movieFileOutput = new AVCaptureMovieFileOutput();
			if (movieSegmentDurationInMilliSeconds > 0)
				movieFileOutput.MaxRecordedDuration = new CMTime( movieSegmentDurationInMilliSeconds, 1000);

			// setup the delegate that handles the writing
			movieSegmentWriter = new MovieSegmentWriterDelegate();

			// subscribe to the delegate events
			movieSegmentWriter.MovieSegmentRecordingStarted += HandleMovieSegmentRecordingStarted;
			movieSegmentWriter.MovieSegmentRecordingComplete += HandleMovieSegmentRecordingComplete;
			movieSegmentWriter.CaptureError += HandleMovieCaptureError;

			session.AddOutput (movieFileOutput);

			return true;
		}

		bool AddImageSamplerOutput( out string errorMessage, int minimumSampleIntervalInMilliSeconds )
		{
			errorMessage = string.Empty;

			// create a VideoDataOutput and add it to the capture session
			frameGrabberOutput = new AVCaptureVideoDataOutput();
			frameGrabberOutput.WeakVideoSettings = new CVPixelBufferAttributes { PixelFormatType = CVPixelFormatType.CV32BGRA }.Dictionary;
			// set up the output queue and delegate
			queue = new DispatchQueue ("captureQueue");
			videoFrameSampler = new VideoFrameSamplerDelegate();
			frameGrabberOutput.SetSampleBufferDelegateQueue (videoFrameSampler, queue);

			// subscribe to from capture events
			videoFrameSampler.CaptureError += HandleImageCaptureError;
			videoFrameSampler.ImageCaptured += HandleImageCaptured;

			// add the output to the session
			session.AddOutput (frameGrabberOutput);

			// set minimum time interval between image samples (if possible).
			try {
				AVCaptureConnection connection = frameGrabberOutput.Connections[0];
				connection.VideoMinFrameDuration = new CMTime(minimumSampleIntervalInMilliSeconds, 1000);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			return true;
		}

		void HandleMovieSegmentRecordingStarted (object sender, MovieSegmentRecordingStartedEventArgs args)
		{
			currentSegmentStartedAt = DateTime.Now;
		}

		#region event handlers
		void HandleMovieSegmentRecordingComplete (object sender, MovieSegmentRecordingCompleteEventArgs args)
		{
			try {
				// grab the pertinent event data
				var captureInfo = new MovieSegmentCapturedEventArgs {
					StartedAt = currentSegmentStartedAt,
					DurationMilliSeconds = movieSegmentDurationInMilliSeconds,
					File = args.Path
				};

				// conditionally start recording the next segment
				if (args.ErrorOccured == false && breakMovieIntoSegments && isCapturing)
					StartRecordingNextMovieFilename ();

				// raise the capture event to external listeners
				OnMovieSegmentCaptured (captureInfo);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		void HandleMovieCaptureError (object sender, CaptureErrorEventArgs args)
		{
			// bubble up
			OnCaptureError (args);
		}

		void HandleImageCaptured (object sender, ImageCaptureEventArgs args)
		{
			// bubble up
			OnImageCaptured (args);
		}

		void HandleImageCaptureError (object sender, CaptureErrorEventArgs args)
		{
			// bubble up
			OnCaptureError (args);
		}
		#endregion

		void UnsubscribeDelegateEvents ()
		{
			try {
				if (videoFrameSampler != null) {
					videoFrameSampler.CaptureError -= HandleImageCaptureError;
					videoFrameSampler.ImageCaptured -= HandleImageCaptured;
				}
				if (movieSegmentWriter != null) {
					movieSegmentWriter.MovieSegmentRecordingStarted -= HandleMovieSegmentRecordingStarted;
					movieSegmentWriter.MovieSegmentRecordingComplete -= HandleMovieSegmentRecordingComplete;
					movieSegmentWriter.CaptureError -= HandleMovieCaptureError;
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		string GetDateTimeDirectoryName (DateTime dateTime)
		{
			return dateTime.ToString ().Replace (":","-").Replace ("/","-").Replace (" ","-").Replace ("\\","-");
		}
	}

	public enum Resolution {
		Low,
		Medium,
		High
	}

	public enum CameraType {
		FrontFacing,
		RearFacing
	}

}

