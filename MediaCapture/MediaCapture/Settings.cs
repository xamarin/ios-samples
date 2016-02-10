using System;
using System.IO;

using Foundation;

namespace MediaCapture {
	public class Settings {
		enum SettingsNames {
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

		CameraType camera = CameraType.FrontFacing;
		public CameraType Camera {
			get {
				return camera;
			}
			set {
				camera = value;
			}
		}

		Resolution captureResolution = Resolution.Medium;
		public Resolution CaptureResolution {
			get {
				return captureResolution;
			}
			set {
				captureResolution = value;
			}
		}

		bool imageCaptureEnabled = true;
		public bool ImageCaptureEnabled {
			get {
				return imageCaptureEnabled;
			}
			set {
				imageCaptureEnabled = value;
			}
		}

		public bool AudioCaptureEnabled { get; set; }

		public bool VideoCaptureEnabled { get; set; }

		public bool SaveCapturedImagesToPhotoLibrary { get; set; }

		public bool SaveCapturedImagesToMyDocuments { get; set; }

		int imageSaveIntervalInSeconds = 5;
		public int ImageSaveIntervalInSeconds {
			get {
				return imageSaveIntervalInSeconds;
			}
		}

		// seconds (0 or negative number means no limit)
		int maxMovieDurationInSeconds = 60;
		public int MaxMovieDurationInSeconds {
			get {
				return maxMovieDurationInSeconds;
			}
			set {
				maxMovieDurationInSeconds = value;
			}
		}

		// whether or not to automatically start recording a new movie once the max duration is reached and recording is forcibly stopped
		bool autoRecordNextMovie = false;
		public bool AutoRecordNextMovie {
			get {
				return autoRecordNextMovie;
			}
			set {
				autoRecordNextMovie = value;
			}
		}

		public void Load ()
		{
			bool isFirstSettingsLoad = (NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.HaveSettingsBeenLoadedBefore.ToString()) == false);
			if (isFirstSettingsLoad) {
				// this forces the defaults to be written and flag that this has happened for future loads
				NSUserDefaults.StandardUserDefaults.SetBool (true, SettingsNames.HaveSettingsBeenLoadedBefore.ToString());
				Save ();
			}

			camera =  NSUserDefaults.StandardUserDefaults.IntForKey (SettingsNames.Camera.ToString()) == 0 ? CameraType.FrontFacing : CameraType.RearFacing ;
			ImageCaptureEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.ImageCaptureEnabled.ToString ());
			AudioCaptureEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.AudioCaptureEnabled.ToString ());
			VideoCaptureEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.VideoCaptureEnabled.ToString ());
			CaptureResolution = Resolution.High;
			MaxMovieDurationInSeconds = 60;
			AutoRecordNextMovie = NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.AutoRecordNextMovie.ToString());
			SaveCapturedImagesToPhotoLibrary = NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.SaveCapturedImagesToPhotoLibrary.ToString ());
			SaveCapturedImagesToMyDocuments = NSUserDefaults.StandardUserDefaults.BoolForKey (SettingsNames.SaveCapturedImagesToMyDocuments.ToString ());
		}

		public void Save ()
		{
			NSUserDefaults.StandardUserDefaults.SetInt ((int)camera, SettingsNames.Camera.ToString ());
			NSUserDefaults.StandardUserDefaults.SetBool (ImageCaptureEnabled, SettingsNames.ImageCaptureEnabled.ToString ());
			NSUserDefaults.StandardUserDefaults.SetBool (AudioCaptureEnabled, SettingsNames.AudioCaptureEnabled.ToString ());
			NSUserDefaults.StandardUserDefaults.SetBool (VideoCaptureEnabled, SettingsNames.VideoCaptureEnabled.ToString ());
			NSUserDefaults.StandardUserDefaults.SetInt ((int)CaptureResolution, SettingsNames.CaptureResolution.ToString ());
			NSUserDefaults.StandardUserDefaults.SetBool (AutoRecordNextMovie, SettingsNames.AutoRecordNextMovie.ToString ());
			NSUserDefaults.StandardUserDefaults.SetInt (MaxMovieDurationInSeconds, SettingsNames.MaxMovieDurationInSeconds.ToString ());
			NSUserDefaults.StandardUserDefaults.SetBool (SaveCapturedImagesToPhotoLibrary, SettingsNames.SaveCapturedImagesToPhotoLibrary.ToString ());
			NSUserDefaults.StandardUserDefaults.SetBool (SaveCapturedImagesToMyDocuments, SettingsNames.SaveCapturedImagesToMyDocuments.ToString ());

			NSUserDefaults.StandardUserDefaults.Synchronize ();
		}

		static string CreateDirectoryIfNeeded (string directory)
		{
			if (Directory.Exists(directory) == false)
				Directory.CreateDirectory (directory);
			return directory;
		}

		static string myDocuments = null;
		public static string MyDocuments {
			get {
				if (myDocuments == null)
					myDocuments = CreateDirectoryIfNeeded (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
				return myDocuments;
			}
		}

		static string configDirectory = null;
		public static string ConfigDirectory {
			get {
				if (configDirectory == null)
					configDirectory = CreateDirectoryIfNeeded (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "Config"));
				return configDirectory;
			}
		}

		static string videoDataPath = null;
		public static string VideoDataPath {
			get {
				if (videoDataPath == null)
					videoDataPath = CreateDirectoryIfNeeded (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "VideoData"));
				return videoDataPath;
			}
		}

		static string imageDataPath = null;
		public static string ImageDataPath {
			get {
				if (imageDataPath == null)
					imageDataPath = CreateDirectoryIfNeeded (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "ImageData"));

				return imageDataPath;
			}
		}

	}
}

