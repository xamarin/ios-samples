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
using System.IO;
using Foundation;

namespace MediaCapture
{
	public class Settings
	{
		private enum SettingsNames
		{
			HaveSettingsBeenLoadedBefore,
			Camera,
			ImageCaptureEnabled,
			AudioCaptureEnabled,
			VideoCaptureEnabled,
			CaptureResolution,
			MaxMovieDurationInSeconds,
			AutoRecordNextMovie,
			SaveCapturedImagesToPhotoLibrary,
			SaveCapturedImagesToMyDocuments,
		}
	
		private CameraType camera = CameraType.FrontFacing;
		public CameraType Camera 
		{
			get
			{
				return camera;	
			}
			set
			{
				camera = value;	
			}
		}
		
		
		private Resolution captureResolution = Resolution.Medium;
		public Resolution CaptureResolution 
		{
			get
			{
				return captureResolution;	
			}
			set
			{
				captureResolution = value;	
			}
		}
		
		private bool imageCaptureEnabled = true;
		public bool ImageCaptureEnabled 
		{ 
			get
			{
				return imageCaptureEnabled;	
			}
			set
			{
				imageCaptureEnabled = value;	
			}
		}
		
		public bool AudioCaptureEnabled { get; set; }
		public bool VideoCaptureEnabled { get; set; }
		public bool SaveCapturedImagesToPhotoLibrary { get; set; }
		public bool SaveCapturedImagesToMyDocuments { get; set; }
	
		private int imageSaveIntervalInSeconds = 5;
		public int ImageSaveIntervalInSeconds
		{
			get
			{
				return imageSaveIntervalInSeconds;
			}
		}
		
		// seconds (0 or negative number means no limit)
		private int maxMovieDurationInSeconds = 60;
		public int MaxMovieDurationInSeconds
		{
			get
			{
				return maxMovieDurationInSeconds;	
			}
			set
			{
				maxMovieDurationInSeconds = value;	
			}
		}
		
		// whether or not to automatically start recording a new movie once the max duration is reached and recording is forcibly stopped
		private bool autoRecordNextMovie = false;
		public bool AutoRecordNextMovie
		{
			get
			{
				return autoRecordNextMovie;	
			}
			set
			{
				autoRecordNextMovie = value;	
			}
		}
			
		public void Load()
		{
			bool isFirstSettingsLoad = (NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.HaveSettingsBeenLoadedBefore.ToString() ) == false);
			if ( isFirstSettingsLoad )
			{
				// this forces the defaults to be written and flag that this has happened for future loads
				NSUserDefaults.StandardUserDefaults.SetBool( true, SettingsNames.HaveSettingsBeenLoadedBefore.ToString() );
				Save ();
			}

			camera = NSUserDefaults.StandardUserDefaults.IntForKey( SettingsNames.Camera.ToString()) == 0 ? CameraType.FrontFacing : CameraType.RearFacing;
			//camera = CameraType.FrontFacing;
			ImageCaptureEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.ImageCaptureEnabled.ToString() );
			AudioCaptureEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.AudioCaptureEnabled.ToString() );
			VideoCaptureEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.VideoCaptureEnabled.ToString() );
			//CaptureResolution = (Resolution) NSUserDefaults.StandardUserDefaults.IntForKey( SettingsNames.CaptureResolution.ToString() );
			CaptureResolution = Resolution.High;
			//MaxMovieDurationInSeconds = NSUserDefaults.StandardUserDefaults.IntForKey( SettingsNames.MaxMovieDurationInSeconds.ToString() );
			MaxMovieDurationInSeconds = 60;
			AutoRecordNextMovie = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.AutoRecordNextMovie.ToString() );
			SaveCapturedImagesToPhotoLibrary = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.SaveCapturedImagesToPhotoLibrary.ToString() );
			SaveCapturedImagesToMyDocuments = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsNames.SaveCapturedImagesToMyDocuments.ToString() );
		}
		
		public void Save()
		{
			NSUserDefaults.StandardUserDefaults.SetInt( (int)camera, SettingsNames.Camera.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( ImageCaptureEnabled, SettingsNames.ImageCaptureEnabled.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( AudioCaptureEnabled, SettingsNames.AudioCaptureEnabled.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( VideoCaptureEnabled, SettingsNames.VideoCaptureEnabled.ToString() );
			NSUserDefaults.StandardUserDefaults.SetInt( (int)CaptureResolution, SettingsNames.CaptureResolution.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( AutoRecordNextMovie, SettingsNames.AutoRecordNextMovie.ToString() );
			NSUserDefaults.StandardUserDefaults.SetInt( MaxMovieDurationInSeconds, SettingsNames.MaxMovieDurationInSeconds.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( SaveCapturedImagesToPhotoLibrary, SettingsNames.SaveCapturedImagesToPhotoLibrary.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( SaveCapturedImagesToMyDocuments, SettingsNames.SaveCapturedImagesToMyDocuments.ToString() );

			NSUserDefaults.StandardUserDefaults.Synchronize();
		}
		
		private static string createDirectoryIfNeeded( string directory )
		{
			if (Directory.Exists(directory) == false)
			{
				Directory.CreateDirectory( directory );
			}
			return directory;
		}
		
		private static string myDocuments = null;
		public static string MyDocuments
		{
			get
			{
				if ( myDocuments == null )
				{
					myDocuments = createDirectoryIfNeeded( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) );
				}
				return myDocuments;
			}
		}
		
		private static string configDirectory = null;
		public static string ConfigDirectory
		{
			get
			{
				if ( configDirectory == null )
				{
					configDirectory = createDirectoryIfNeeded( Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Config" ) );
				}
				return configDirectory;
			}
		}

		private static string videoDataPath = null;
		public static string VideoDataPath
		{
			get
			{
				if ( videoDataPath == null )
				{
					videoDataPath = createDirectoryIfNeeded( Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VideoData" ) );
				}
				return videoDataPath;
			}
		}

		private static string imageDataPath = null;
		public static string ImageDataPath
		{
			get
			{
				if ( imageDataPath == null )
				{
					imageDataPath = createDirectoryIfNeeded( Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ImageData" ) );
				}
				return imageDataPath;
			}
		}

	
		
		
	}
}

