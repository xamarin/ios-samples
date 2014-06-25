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
		private AVCaptureSession session = null;
		private bool isCapturing = false;
		private bool captureImages = false;
		private bool captureAudio = false;
		private bool captureVideo = false;
		private string movieRecordingDirectory;
		private CameraType cameraType = CameraType.RearFacing;
		private Resolution resolution = Resolution.Medium;

		// camera input objects
		private AVCaptureDevice videoCaptureDevice = null;
		private AVCaptureDeviceInput videoInput = null;
		
		// microphone input objects
		private AVCaptureDevice audioCaptureDevice = null;
		private AVCaptureDeviceInput audioInput = null;
		
		// frame grabber objects
		private AVCaptureVideoDataOutput frameGrabberOutput = null;
		private VideoFrameSamplerDelegate videoFrameSampler = null;
		private DispatchQueue queue = null;
		
		// movie recorder objects
		private AVCaptureMovieFileOutput movieFileOutput = null;
		private MovieSegmentWriterDelegate movieSegmentWriter = null;
		private string currentSegmentFile = null;
		private DateTime currentSegmentStartedAt;
		private uint nextMovieIndex = 1;
		private int movieSegmentDurationInMilliSeconds = 20000;
		private bool breakMovieIntoSegments = true; 
		
		
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
			    this.movieRecordingDirectory =  Path.Combine( movieRecordingDirectory, getDateTimeDirectoryName( DateTime.Now ) );
				if ( Directory.Exists( this.movieRecordingDirectory ) == false )
				{
					Directory.CreateDirectory( this.movieRecordingDirectory );
				}
			}
		}
		
		#region events
		public EventHandler<MovieSegmentCapturedEventArgs> MovieSegmentCaptured;
		private void onMovieSegmentCaptured( MovieSegmentCapturedEventArgs args )
		{
			if (  MovieSegmentCaptured != null )
			{
				MovieSegmentCaptured( this, args );
			}
		}

		public EventHandler<CaptureErrorEventArgs> CaptureError;
		private void onCaptureError( CaptureErrorEventArgs args )
		{
			if (  CaptureError != null )
			{
				CaptureError( this, args );
			}
		}

		public EventHandler<ImageCaptureEventArgs> ImageCaptured;
		private void onImageCaptured( ImageCaptureEventArgs args )
		{
			if (  ImageCaptured != null )
			{
				ImageCaptured( this, args );
			}
		}
		#endregion

		public int MovieSegmentDurationInMilliSeconds 
		{
			get 
			{
				return movieSegmentDurationInMilliSeconds;
			}
		}

		public bool IsCapturing 
		{
			get 
			{
				return this.isCapturing;
			}
			set 
			{
				isCapturing = value;
			}
		}		
		
		private bool shouldRecord
		{
			get
			{
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
			if ( setupCaptureSessionInternal( out message ) == false )
			{
				return false;	
			}
			
			// start the capture
			session.StartRunning();
			
			// start recording (if configured)
			if ( shouldRecord )
			{
				startMovieWriter();
			}
			
			return true;
		}

		public void StopCapture()
		{
			if ( isCapturing == false )
			{
				return;
			}
			
			isCapturing = false;
			
			// stop recording
			if ( shouldRecord )
			{
				stopMovieWriter();
			}
			
			// stop the capture session
			session.StopRunning();
			
			unsubscribeDelegateEvents();
		}

		private void startMovieWriter()
		{
			if ( movieFileOutput == null )
			{
				return;
			}
			
			startRecordingNextMovieFilename();
		}

		private void startRecordingNextMovieFilename()
		{
			// generate file name
			currentSegmentFile = System.IO.Path.Combine( this.movieRecordingDirectory, string.Format("video_{0}.mov", nextMovieIndex++) );
			NSUrl segmentUrl = NSUrl.FromFilename( currentSegmentFile );
			
			// start recording
			movieFileOutput.StartRecordingToOutputFile( segmentUrl, movieSegmentWriter);
		}
		
		private void stopMovieWriter()
		{
			if ( movieFileOutput == null )
			{
				return;
			}
			movieFileOutput.StopRecording();
		}
		
		private bool setupCaptureSessionInternal( out string errorMessage )
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
				if ( addCameraInput( out errorMessage ) == false )
				{
					return false;	
				}
			}

			// conditionally configure the microphone input
			if ( captureAudio )
			{
				if ( addAudioInput( out errorMessage ) == false )
				{
					return false;	
				}
			}

			// conditionally configure the sample buffer output
			if ( captureImages )
			{
				int minimumSampleIntervalInMilliSeconds = captureVideo ? 1000 : 100;
				if ( addImageSamplerOutput( out errorMessage, minimumSampleIntervalInMilliSeconds ) == false )
				{
					return false;	
				}
			}
			
			// conditionally configure the movie file output
			if ( shouldRecord )
			{
				if ( addMovieFileOutput( out errorMessage ) == false )
				{
					return false;	
				}
			}
			
			return true;
		}

		private bool addCameraInput( out string errorMessage )
		{
			errorMessage = "";
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

		private bool addAudioInput( out string errorMessage )
		{
			errorMessage = "";
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
		
		private bool addMovieFileOutput( out string errorMessage )
		{
			errorMessage = "";
			
			// create a movie file output and add it to the capture session
			movieFileOutput = new AVCaptureMovieFileOutput();
			if ( movieSegmentDurationInMilliSeconds > 0 )
			{
				movieFileOutput.MaxRecordedDuration = new CMTime( movieSegmentDurationInMilliSeconds, 1000 );
			}

			// setup the delegate that handles the writing
			movieSegmentWriter = new MovieSegmentWriterDelegate();
			
			// subscribe to the delegate events
			movieSegmentWriter.MovieSegmentRecordingStarted += new EventHandler<MovieSegmentRecordingStartedEventArgs>( handleMovieSegmentRecordingStarted );
			movieSegmentWriter.MovieSegmentRecordingComplete += new EventHandler<MovieSegmentRecordingCompleteEventArgs>( handleMovieSegmentRecordingComplete );
			movieSegmentWriter.CaptureError += new EventHandler<CaptureErrorEventArgs>( handleMovieCaptureError );
			
			session.AddOutput (movieFileOutput);

			return true;
		}
		
		private bool addImageSamplerOutput( out string errorMessage, int minimumSampleIntervalInMilliSeconds )
		{
			errorMessage = "";
			
			// create a VideoDataOutput and add it to the capture session
			frameGrabberOutput = new AVCaptureVideoDataOutput();
			frameGrabberOutput.CompressedVideoSetting = new AVVideoSettingsCompressed ();
			
			// set up the output queue and delegate
			queue = new CoreFoundation.DispatchQueue ("captureQueue");
			videoFrameSampler = new VideoFrameSamplerDelegate();
			frameGrabberOutput.SetSampleBufferDelegateQueue (videoFrameSampler, queue);

			// subscribe to from capture events
			videoFrameSampler.CaptureError += new EventHandler<CaptureErrorEventArgs>( handleImageCaptureError );
			videoFrameSampler.ImageCaptured += new EventHandler<ImageCaptureEventArgs>( handleImageCaptured );
			
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
		
		private void handleMovieSegmentRecordingStarted( object sender, MovieSegmentRecordingStartedEventArgs args )
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
				if ( args.ErrorOccured == false && breakMovieIntoSegments && isCapturing)
				{
					startRecordingNextMovieFilename();
				}
				
				// raise the capture event to external listeners
				onMovieSegmentCaptured( captureInfo );
			}
			catch
			{
			}
		}
		
		private void handleMovieCaptureError(object sender, CaptureErrorEventArgs args )
		{
			// bubble up
			onCaptureError( args );
		}
		
		private void handleImageCaptured( object sender, ImageCaptureEventArgs args)
		{
			// bubble up
			onImageCaptured( args);			
		}

		private void handleImageCaptureError( object sender, CaptureErrorEventArgs args )
		{
			// bubble up
			onCaptureError( args );
		}
		#endregion
		
		private void unsubscribeDelegateEvents()
		{
			try
			{
				if ( videoFrameSampler != null )
				{
					videoFrameSampler.CaptureError -= new EventHandler<CaptureErrorEventArgs>( handleImageCaptureError );
					videoFrameSampler.ImageCaptured -= new EventHandler<ImageCaptureEventArgs>( handleImageCaptured );
				}
				if ( movieSegmentWriter != null )
				{
					movieSegmentWriter.MovieSegmentRecordingStarted -= new EventHandler<MovieSegmentRecordingStartedEventArgs>( handleMovieSegmentRecordingStarted );
					movieSegmentWriter.MovieSegmentRecordingComplete -= new EventHandler<MovieSegmentRecordingCompleteEventArgs>( handleMovieSegmentRecordingComplete );
					movieSegmentWriter.CaptureError -= new EventHandler<CaptureErrorEventArgs>( handleMovieCaptureError );
				}
			}
			catch
			{
			}
		}
		
		private string getDateTimeDirectoryName( DateTime dateTime )
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

