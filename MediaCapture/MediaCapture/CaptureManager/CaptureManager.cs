//
// how to capture still images, video and audio using iOS AVFoundation and the AVCAptureSession
//
// This sample handles all of the low-level AVFoundation and capture graph setup required to capture and save media.  This code also exposes the
// capture, configuration and notification capabilities in a more '.Netish' way of programming.  The client code will not need to deal with threads, delegate classes
// buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  The underlying iOS concepts and classes are detailed in
// the iOS developer online help (TP40010188-CH5-SW2).
//
// https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2
//
// Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com
//
using System;
using System.Collections.Concurrent;
using CoreGraphics;
using System.IO;
using Foundation;
using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreFoundation;
using System.Runtime.InteropServices;

namespace MediaCapture
{
	public class CaptureManager
	{
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

		private CaptureManager(){}

		public CaptureManager(
			Resolution resolution,
			bool captureImages,
			bool captureAudio,
			bool captureVideo,
			CameraType cameraType,
			string movieRecordingDirectory,
			int movieSegmentDurationInMilliSeconds,
			bool breakMovieIntoSegments )
		{
			this.resolution = resolution;
			this.captureImages = captureImages;
			this.captureAudio = captureAudio;
			this.captureVideo = captureVideo;
			this.cameraType = cameraType;
			this.movieSegmentDurationInMilliSeconds = movieSegmentDurationInMilliSeconds;
			this.breakMovieIntoSegments = breakMovieIntoSegments;
			if ( captureAudio || captureVideo )
			{
				this.movieRecordingDirectory =  Path.Combine( movieRecordingDirectory, GetDateTimeDirectoryName( DateTime.Now ) );
				if ( Directory.Exists( this.movieRecordingDirectory ) == false )
					Directory.CreateDirectory( this.movieRecordingDirectory );
			}
		}

		#region events

		public EventHandler<MovieSegmentCapturedEventArgs> MovieSegmentCaptured;
		void onMovieSegmentCaptured( MovieSegmentCapturedEventArgs args )
		{
			if (MovieSegmentCaptured != null)
				MovieSegmentCaptured( this, args );
		}

		public EventHandler<CaptureErrorEventArgs> CaptureError;
		void OnCaptureError( CaptureErrorEventArgs args )
		{
			if (CaptureError != null )
				CaptureError( this, args );
		}

		public EventHandler<ImageCaptureEventArgs> ImageCaptured;
		void OnImageCaptured( ImageCaptureEventArgs args )
		{
			if (  ImageCaptured != null )
			{
				ImageCaptured( this, args );
			}
		}
		#endregion

		public int MovieSegmentDurationInMilliSeconds {
			get {
				return movieSegmentDurationInMilliSeconds;
			}
		}

		public bool IsCapturing {
			get {
				return this.isCapturing;
			}
			set {
				isCapturing = value;
			}
		}

		bool ShouldRecord {
			get {
				return ( ( this.captureAudio || this.captureVideo ) && ( string.IsNullOrEmpty(this.movieRecordingDirectory) == false ) );
			}
		}

		public bool StartCapture( out string message )
		{
			message = "";
			if ( isCapturing == true )
			{
				message = "already capturing";
				return true;
			}

			isCapturing = true;
			if (!SetupCaptureSessionInternal(out message))
				return false;

			// start the capture
			session.StartRunning();

			// start recording (if configured)
			if ( ShouldRecord )
				StartMovieWriter();

			return true;
		}

		public void StopCapture()
		{
			if ( isCapturing == false )
				return;

			isCapturing = false;

			// stop recording
			if ( ShouldRecord )
				StopMovieWriter();

			// stop the capture session
			session.StopRunning();

			UnsubscribeDelegateEvents();
		}

		void StartMovieWriter()
		{
			if ( movieFileOutput == null )
			{
				return;
			}

			StartRecordingNextMovieFilename();
		}

		void StartRecordingNextMovieFilename()
		{
			// generate file name
			currentSegmentFile = System.IO.Path.Combine( this.movieRecordingDirectory, string.Format("video_{0}.mov", nextMovieIndex++) );
			NSUrl segmentUrl = NSUrl.FromFilename( currentSegmentFile );

			// start recording
			movieFileOutput.StartRecordingToOutputFile( segmentUrl, movieSegmentWriter);
		}

		void StopMovieWriter()
		{
			if ( movieFileOutput == null )
				return;
			movieFileOutput.StopRecording();
		}

		bool SetupCaptureSessionInternal( out string errorMessage )
		{
			errorMessage = "";

			// create the capture session
			session = new AVCaptureSession();
			switch ( resolution )
			{
				case Resolution.Low:
					session.SessionPreset = AVCaptureSession.PresetLow;
					break;

				case Resolution.High:
					session.SessionPreset = AVCaptureSession.PresetHigh;
					break;

				case Resolution.Medium:
				default:
					session.SessionPreset = AVCaptureSession.PresetMedium;
					break;
			}

			// conditionally configure the camera input
			if ( captureVideo || captureImages)
			{
				if (!AddCameraInput(out errorMessage))
					return false;
			}

			// conditionally configure the microphone input
			if ( captureAudio )
			{
				if (!AddAudioInput( out errorMessage ))
					return false;
			}

			// conditionally configure the sample buffer output
			if ( captureImages )
			{
				int minimumSampleIntervalInMilliSeconds = captureVideo ? 1000 : 100;
				if (!AddImageSamplerOutput( out errorMessage, minimumSampleIntervalInMilliSeconds ))
					return false;
			}

			// conditionally configure the movie file output
			if ( ShouldRecord )
			{
				if (!AddMovieFileOutput( out errorMessage ))
					return false;
			}

			return true;
		}

		bool AddCameraInput( out string errorMessage )
		{
			errorMessage = string.Empty;
			videoCaptureDevice = this.cameraType == CameraType.FrontFacing ? MediaDevices.FrontCamera : MediaDevices.BackCamera;
			videoInput = AVCaptureDeviceInput.FromDevice(videoCaptureDevice);
			if (videoInput == null)
			{
				errorMessage = "No video capture device";
				return false;
			}
			session.AddInput (videoInput);
			return true;
		}

		bool AddAudioInput( out string errorMessage )
		{
			errorMessage = string.Empty;
			audioCaptureDevice = MediaDevices.Microphone;
			audioInput = AVCaptureDeviceInput.FromDevice(audioCaptureDevice);
			if (audioInput == null)
			{
				errorMessage = "No audio capture device";
				return false;
			}
			session.AddInput (audioInput);
			return true;
		}

		bool AddMovieFileOutput( out string errorMessage )
		{
			errorMessage = "";

			// create a movie file output and add it to the capture session
			movieFileOutput = new AVCaptureMovieFileOutput();
			if ( movieSegmentDurationInMilliSeconds > 0 )
				movieFileOutput.MaxRecordedDuration = new CMTime( movieSegmentDurationInMilliSeconds, 1000 );

			// setup the delegate that handles the writing
			movieSegmentWriter = new MovieSegmentWriterDelegate();

			// subscribe to the delegate events
			movieSegmentWriter.MovieSegmentRecordingStarted += new EventHandler<MovieSegmentRecordingStartedEventArgs>( HandleMovieSegmentRecordingStarted );
			movieSegmentWriter.MovieSegmentRecordingComplete += new EventHandler<MovieSegmentRecordingCompleteEventArgs>( handleMovieSegmentRecordingComplete );
			movieSegmentWriter.CaptureError += new EventHandler<CaptureErrorEventArgs>( HandleMovieCaptureError );

			session.AddOutput (movieFileOutput);

			return true;
		}

		bool AddImageSamplerOutput( out string errorMessage, int minimumSampleIntervalInMilliSeconds )
		{
			errorMessage = "";

			// create a VideoDataOutput and add it to the capture session
			frameGrabberOutput = new AVCaptureVideoDataOutput();
			frameGrabberOutput.WeakVideoSettings = new CVPixelBufferAttributes () { PixelFormatType = CVPixelFormatType.CV32BGRA }.Dictionary;
			// set up the output queue and delegate
			queue = new CoreFoundation.DispatchQueue ("captureQueue");
			videoFrameSampler = new VideoFrameSamplerDelegate();
			frameGrabberOutput.SetSampleBufferDelegateQueue (videoFrameSampler, queue);

			// subscribe to from capture events
			videoFrameSampler.CaptureError += new EventHandler<CaptureErrorEventArgs>( HandleImageCaptureError );
			videoFrameSampler.ImageCaptured += new EventHandler<ImageCaptureEventArgs>( HandleImageCaptured );

			// add the output to the session
			session.AddOutput (frameGrabberOutput);

			// set minimum time interval between image samples (if possible).
			try
			{
				AVCaptureConnection connection = (AVCaptureConnection)frameGrabberOutput.Connections[0];
				connection.VideoMinFrameDuration = new CMTime(minimumSampleIntervalInMilliSeconds, 1000);
			}
			catch
			{
			}

			return true;
		}

		void HandleMovieSegmentRecordingStarted( object sender, MovieSegmentRecordingStartedEventArgs args )
		{
			currentSegmentStartedAt = DateTime.Now;
		}

		#region event handlers
		private void handleMovieSegmentRecordingComplete(object sender, MovieSegmentRecordingCompleteEventArgs args )
		{
			try
			{
				// grab the pertinent event data
				MovieSegmentCapturedEventArgs captureInfo = new MovieSegmentCapturedEventArgs();
				captureInfo.StartedAt = currentSegmentStartedAt;
				captureInfo.DurationMilliSeconds = movieSegmentDurationInMilliSeconds;
				captureInfo.File = args.Path;

				// conditionally start recording the next segment
				if ( !args.ErrorOccured && breakMovieIntoSegments && isCapturing)
					StartRecordingNextMovieFilename();

				// raise the capture event to external listeners
				onMovieSegmentCaptured( captureInfo );
			}
			catch
			{
			}
		}

		void HandleMovieCaptureError(object sender, CaptureErrorEventArgs args )
		{
			// bubble up
			OnCaptureError( args );
		}

		void HandleImageCaptured( object sender, ImageCaptureEventArgs args)
		{
			// bubble up
			OnImageCaptured( args);
		}

		void HandleImageCaptureError( object sender, CaptureErrorEventArgs args )
		{
			// bubble up
			OnCaptureError( args );
		}
		#endregion

		void UnsubscribeDelegateEvents()
		{
			try
			{
				if ( videoFrameSampler != null )
				{
					videoFrameSampler.CaptureError -= new EventHandler<CaptureErrorEventArgs>( HandleImageCaptureError );
					videoFrameSampler.ImageCaptured -= new EventHandler<ImageCaptureEventArgs>( HandleImageCaptured );
				}
				if ( movieSegmentWriter != null )
				{
					movieSegmentWriter.MovieSegmentRecordingStarted -= new EventHandler<MovieSegmentRecordingStartedEventArgs>( HandleMovieSegmentRecordingStarted );
					movieSegmentWriter.MovieSegmentRecordingComplete -= new EventHandler<MovieSegmentRecordingCompleteEventArgs>( handleMovieSegmentRecordingComplete );
					movieSegmentWriter.CaptureError -= new EventHandler<CaptureErrorEventArgs>( HandleMovieCaptureError );
				}
			}
			catch
			{
			}
		}

		string GetDateTimeDirectoryName( DateTime dateTime )
		{
			return dateTime.ToString().Replace(":","-").Replace ("/","-").Replace (" ","-").Replace ("\\","-");
		}

	}

	public enum Resolution
	{
		Low,
		Medium,
		High
	}

	public enum CameraType
	{
		FrontFacing,
		RearFacing
	}
}
