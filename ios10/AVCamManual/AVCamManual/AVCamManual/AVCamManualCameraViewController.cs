using System;
using System.Globalization;
using System.IO;
using System.Linq;

using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using Photos;
using UIKit;

namespace AVCamManual
{
	public enum SetupResult
	{
		Success,
		CameraNotAuthorized,
		SessionConfigurationFailed
	};

	public enum CaptureMode
	{
		Photo,
		Movie
	}

	[Register ("AVCamManualCameraViewController")]
	public partial class AVCamManualCameraViewController : UIViewController, IAVCapturePhotoCaptureDelegate, IAVCaptureFileOutputRecordingDelegate
	{
		IDisposable focusModeToken;
		IDisposable lensPositionToken;
		IDisposable exposureModeToken;
		IDisposable exposureDurationToken;
		IDisposable isoToken;
		IDisposable runningToken;
		IDisposable exposureTargetBiasToken;
		IDisposable exposureTargetOffsetToken;
		IDisposable whiteBalanceModeToken;
		IDisposable deviceWhiteBalanceGainsToken;
		NSObject subjectAreaDidChangeToken;
		NSObject runtimeErrorToken;
		NSObject wasInterruptedToken;
		NSObject interruptionEndedToken;

		DispatchQueue sessionQueue;
		AVCaptureMovieFileOutput movieFileOutput;
		AVCapturePhotoOutput photoOutput;

		// Utilities
		SetupResult setupResult;
		bool sessionRunning;
		nint backgroundRecordingID;

		AVCaptureFocusMode [] focusModes;
		AVCaptureExposureMode [] exposureModes;
		AVCaptureWhiteBalanceMode [] whiteBalanceModes;

		public AVCamManualCameraViewController (IntPtr handle)
			: base (handle)
		{
		}

		#region Lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Disable UI until the session starts running
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			PhotoButton.Enabled = false;
			CaptureModeControl.Enabled = false;
			HUDButton.Enabled = false;

			ManualHUD.Hidden = true;
			ManualHUDPhotoView.Hidden = true;
			ManualHUDFocusView.Hidden = true;
			ManualHUDExposureView.Hidden = true;
			ManualHUDWhiteBalanceView.Hidden = true;
			ManualHUDLensStabilizationView.Hidden = true;

			// Create the AVCaptureSession
			Session = new AVCaptureSession ();

			// Set up preview
			PreviewView.Session = Session;

			sessionQueue = new DispatchQueue ("session queue");
			setupResult = SetupResult.Success;

			// Check video authorization status. Video access is required and audio access is optional.
			// If audio access is denied, audio is not recorded during movie recording.
			CheckDeviceAuthorizationStatus ();

			// Setup the capture session.
			// In general it is not safe to mutate an AVCaptureSession or any of its inputs, outputs, or connections from multiple threads at the same time.
			// Why not do all of this on the main queue?
			// Because AVCaptureSession.StartRunning is a blocking call which can take a long time. We dispatch session setup to the sessionQueue
			// so that the main queue isn't blocked, which keeps the UI responsive.
			sessionQueue.DispatchAsync (ConfigureSession);
		}

		void CheckDeviceAuthorizationStatus ()
		{
			var status = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);
			switch (status) {
			// The user has previously granted access to the camera
			case AVAuthorizationStatus.Authorized:
				break;

			// The user has not yet been presented with the option to grant video access.
			// We suspend the session queue to delay session running until the access request has completed.
			// Note that audio access will be implicitly requested when we create an AVCaptureDeviceInput for audio during session setup.
			case AVAuthorizationStatus.NotDetermined:
				sessionQueue.Suspend ();
				AVCaptureDevice.RequestAccessForMediaType (AVMediaType.Video, granted => {
					if (!granted)
						setupResult = SetupResult.CameraNotAuthorized;
					sessionQueue.Resume ();
				});
				break;

			default:
				// The user has previously denied access
				setupResult = SetupResult.CameraNotAuthorized;
				break;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			sessionQueue.DispatchAsync (() => {
				switch (setupResult) {
				// Only setup observers and start the session running if setup succeeded
				case SetupResult.Success:
					AddObservers ();
					Session.StartRunning ();
					sessionRunning = Session.Running;
					break;

				case SetupResult.CameraNotAuthorized:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "AVCamManual doesn't have permission to use the camera, please change privacy settings";
						UIAlertController alertController = UIAlertController.Create ("AVCamManual", message, UIAlertControllerStyle.Alert);
						UIAlertAction cancelAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
						alertController.AddAction (cancelAction);
						// Provide quick access to Settings
						UIAlertAction settingsAction = UIAlertAction.Create ("Settings", UIAlertActionStyle.Default, action => {
							UIApplication.SharedApplication.OpenUrl (NSUrl.FromString (UIApplication.OpenSettingsUrlString), new UIApplicationOpenUrlOptions (), null);
						});
						alertController.AddAction (settingsAction);
						PresentViewController (alertController, true, null);
					});
					break;
				case SetupResult.SessionConfigurationFailed:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "Unable to capture media";
						UIAlertController alertController = UIAlertController.Create ("AVCamManual", message, UIAlertControllerStyle.Alert);
						UIAlertAction cancelAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
						alertController.AddAction (cancelAction);
						PresentViewController (alertController, true, null);
					});
					break;
				}
			});
		}

		public override void ViewDidDisappear (bool animated)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				if (setupResult == SetupResult.Success) {
					Session.StopRunning ();
					RemoveObservers ();
				}
			});

			base.ViewDidDisappear (animated);
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			UIDeviceOrientation deviceOrientation = UIDevice.CurrentDevice.Orientation;
			if (deviceOrientation.IsPortrait () || deviceOrientation.IsLandscape ()) {
				var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
				var connection = PreviewLayer.Connection;
				if(connection != null)
					connection.VideoOrientation = (AVCaptureVideoOrientation)deviceOrientation;
			}
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		public override bool ShouldAutorotate ()
		{
			// Disable autorotation of the interface when recording is in progress
			return movieFileOutput == null || !movieFileOutput.Recording;
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		#endregion

		#region HUD

		void ConfigureManualHUD ()
		{
			// Manual focus controls
			focusModes = new AVCaptureFocusMode [] { AVCaptureFocusMode.ContinuousAutoFocus, AVCaptureFocusMode.Locked };

			FocusModeControl.Enabled = VideoDevice != null;
			FocusModeControl.SelectedSegment = Array.IndexOf (focusModes, VideoDevice.FocusMode);
			foreach (var mode in focusModes)
				FocusModeControl.SetEnabled (VideoDevice.IsFocusModeSupported (mode), Array.IndexOf (focusModes, mode));

			LensPositionSlider.MinValue = 0;
			LensPositionSlider.MaxValue = 1;
			LensPositionSlider.Value = VideoDevice.LensPosition;
			LensPositionSlider.Enabled = (VideoDevice != null && VideoDevice.FocusMode == AVCaptureFocusMode.Locked && VideoDevice.IsFocusModeSupported (AVCaptureFocusMode.Locked));

			// Manual exposure controls
			exposureModes = new AVCaptureExposureMode [] {
				AVCaptureExposureMode.AutoExpose,
				AVCaptureExposureMode.Locked,
				AVCaptureExposureMode.Custom
			};

			ExposureModeControl.Enabled = VideoDevice != null;
			ExposureModeControl.SelectedSegment = Array.IndexOf (exposureModes, VideoDevice.ExposureMode);
			foreach (var mode in exposureModes)
				ExposureModeControl.SetEnabled (VideoDevice.IsExposureModeSupported (mode), Array.IndexOf (exposureModes, mode));

			// Use 0-1 as the slider range and do a non-linear mapping from the slider value to the actual device exposure duration
			ExposureDurationSlider.MinValue = 0;
			ExposureDurationSlider.MaxValue = 1;
			double exposureDurationSeconds = VideoDevice.ExposureDuration.Seconds;
			double minExposureDurationSeconds = Math.Max (VideoDevice.ActiveFormat.MinExposureDuration.Seconds, ExposureMinDuration);
			double maxExposureDurationSeconds = VideoDevice.ActiveFormat.MaxExposureDuration.Seconds;
			// Map from duration to non-linear UI range 0-1
			double p = (exposureDurationSeconds - minExposureDurationSeconds) / (maxExposureDurationSeconds - minExposureDurationSeconds); // Scale to 0-1
			ExposureDurationSlider.Value = (float)Math.Pow (p, 1 / ExposureDurationPower); // Apply inverse power
			ExposureDurationSlider.Enabled = (VideoDevice != null && VideoDevice.ExposureMode == AVCaptureExposureMode.Custom);

			ISOSlider.MinValue = VideoDevice.ActiveFormat.MinISO;
			ISOSlider.MaxValue = VideoDevice.ActiveFormat.MaxISO;
			ISOSlider.Value = VideoDevice.ISO;
			ISOSlider.Enabled = (VideoDevice.ExposureMode == AVCaptureExposureMode.Custom);

			ExposureTargetBiasSlider.MinValue = VideoDevice.MinExposureTargetBias;
			ExposureTargetBiasSlider.MaxValue = VideoDevice.MaxExposureTargetBias;
			ExposureTargetBiasSlider.Value = VideoDevice.ExposureTargetBias;
			ExposureTargetBiasSlider.Enabled = (VideoDevice != null);

			ExposureTargetOffsetSlider.MinValue = VideoDevice.MinExposureTargetBias;
			ExposureTargetOffsetSlider.MaxValue = VideoDevice.MaxExposureTargetBias;
			ExposureTargetOffsetSlider.Value = VideoDevice.ExposureTargetOffset;
			ExposureTargetOffsetSlider.Enabled = false;

			// Manual white balance controls
			whiteBalanceModes = new AVCaptureWhiteBalanceMode [] {
				AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance,
				AVCaptureWhiteBalanceMode.Locked
			};

			WhiteBalanceModeControl.Enabled = (VideoDevice != null);
			WhiteBalanceModeControl.SelectedSegment = Array.IndexOf (whiteBalanceModes, VideoDevice.WhiteBalanceMode);
			foreach (var mode in whiteBalanceModes)
				WhiteBalanceModeControl.SetEnabled (VideoDevice.IsWhiteBalanceModeSupported (mode), Array.IndexOf (whiteBalanceModes, mode));

			AVCaptureWhiteBalanceGains whiteBalanceGains = VideoDevice.DeviceWhiteBalanceGains;
			AVCaptureWhiteBalanceTemperatureAndTintValues whiteBalanceTemperatureAndTint = VideoDevice.GetTemperatureAndTintValues (whiteBalanceGains);

			TemperatureSlider.MinValue = 3000;
			TemperatureSlider.MaxValue = 8000;
			TemperatureSlider.Value = whiteBalanceTemperatureAndTint.Temperature;
			TemperatureSlider.Enabled = (VideoDevice != null && VideoDevice.WhiteBalanceMode == AVCaptureWhiteBalanceMode.Locked);

			TintSlider.MinValue = -150;
			TintSlider.MaxValue = 150;
			TintSlider.Value = whiteBalanceTemperatureAndTint.Tint;
			TintSlider.Enabled = (VideoDevice != null && VideoDevice.WhiteBalanceMode == AVCaptureWhiteBalanceMode.Locked);

			LensStabilizationControl.Enabled = (VideoDevice != null);
			LensStabilizationControl.SelectedSegment = 0;
			LensStabilizationControl.SetEnabled (photoOutput.IsLensStabilizationDuringBracketedCaptureSupported, 1);

			RawControl.Enabled = (VideoDevice != null);
			RawControl.SelectedSegment = 0;
		}

		[Action ("toggleHUD:")]
		void ToggleHUD (NSObject sender)
		{
			ManualHUD.Hidden = !ManualHUD.Hidden;
		}

		[Action ("changeManualHUD:")]
		void ChangeManualHUD (NSObject sender)
		{
			var control = (UISegmentedControl)sender;

			ManualHUDPhotoView.Hidden = control.SelectedSegment != 0;
			ManualHUDFocusView.Hidden = control.SelectedSegment != 1;
			ManualHUDExposureView.Hidden = control.SelectedSegment != 2;
			ManualHUDWhiteBalanceView.Hidden = control.SelectedSegment != 3;
			ManualHUDLensStabilizationView.Hidden = control.SelectedSegment != 4;
		}

		void SetColorFor (UISlider slider, UIColor color)
		{
			slider.TintColor = color;

			if (slider == LensPositionSlider)
				LensPositionNameLabel.TextColor = LensPositionValueLabel.TextColor = color;
			else if (slider == ExposureDurationSlider)
				ExposureDurationNameLabel.TextColor = ExposureDurationValueLabel.TextColor = color;
			else if (slider == ISOSlider)
				IsoNameLabel.TextColor = ISOValueLabel.TextColor = color;
			else if (slider == ExposureTargetBiasSlider)
				ExposureTargetBiasNameLabel.TextColor = ExposureTargetBiasValueLabel.TextColor = color;
			else if (slider == TemperatureSlider)
				TemperatureNameLabel.TextColor = TemperatureValueLabel.TextColor = color;
			else if (slider == TintSlider)
				TintNameLabel.TextColor = TintValueLabel.TextColor = color;
		}

		[Action ("sliderTouchBegan:")]
		void SliderTouchBegan (NSObject sender)
		{
			var slider = (UISlider)sender;
			SetColorFor (slider, UIColor.FromRGBA (0, 122, 1, 1));
		}

		[Action ("sliderTouchEnded:")]
		void SliderTouchEnded (NSObject sender)
		{
			var slider = (UISlider)sender;
			SetColorFor (slider, UIColor.Yellow);
		}

		#endregion

		#region Session Management

		void ConfigureSession ()
		{
			if (setupResult != SetupResult.Success)
				return;

			NSError error = null;
			Session.BeginConfiguration ();
			Session.SessionPreset = AVCaptureSession.PresetPhoto;

			// Add video input
			AVCaptureDevice vDevice = GetDeviceFrom (AVMediaType.Video, AVCaptureDevicePosition.Back);
			AVCaptureDeviceInput vDeviceInput = AVCaptureDeviceInput.FromDevice (vDevice, out error);
			if (error != null) {
				Console.WriteLine ("Could not create video device input: {0}", error);
				setupResult = SetupResult.SessionConfigurationFailed;
				Session.CommitConfiguration ();
				return;
			}
			if (Session.CanAddInput (vDeviceInput)) {
				Session.AddInput (vDeviceInput);
				VideoDeviceInput = vDeviceInput;
				VideoDevice = vDeviceInput.Device;
			} else {
				Console.WriteLine ("Could not add video device input to the session");
				setupResult = SetupResult.SessionConfigurationFailed;
				Session.CommitConfiguration ();
				return;
			}

			// Add audio input
			AVCaptureDevice aDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Audio);
			AVCaptureDeviceInput aDeviceInput = AVCaptureDeviceInput.FromDevice (aDevice, out error);
			if (error != null)
				Console.WriteLine ("Could not create audio device input: {0}", error);
			if (Session.CanAddInput (aDeviceInput))
				Session.AddInput (aDeviceInput);
			else
				Console.WriteLine ("Could not add audio device input to the session");

			// Add photo output
			var po = new AVCapturePhotoOutput ();
			if (Session.CanAddOutput (po)) {
				Session.AddOutput (po);
				photoOutput = po;
				photoOutput.IsHighResolutionCaptureEnabled = true;
			} else {
				Console.WriteLine ("Could not add photo output to the session");
				setupResult = SetupResult.SessionConfigurationFailed;
				Session.CommitConfiguration ();
				return;
			}

			// We will not create an AVCaptureMovieFileOutput when configuring the session because the AVCaptureMovieFileOutput does not support movie recording with AVCaptureSessionPresetPhoto
			backgroundRecordingID = -1;

			Session.CommitConfiguration ();
			DispatchQueue.MainQueue.DispatchAsync (ConfigureManualHUD);
		}

		// Should be called on the main queue
		AVCapturePhotoSettings GetCurrentPhotoSettings ()
		{
			bool lensStabilizationEnabled = LensStabilizationControl.SelectedSegment == 1;
			bool rawEnabled = RawControl.SelectedSegment == 1;
			AVCapturePhotoSettings photoSettings = null;

			if (lensStabilizationEnabled && photoOutput.IsLensStabilizationDuringBracketedCaptureSupported) {
				AVCaptureBracketedStillImageSettings [] bracketedSettings = null;
				if (VideoDevice.ExposureMode == AVCaptureExposureMode.Custom) {
					bracketedSettings = new AVCaptureBracketedStillImageSettings [] {
						AVCaptureManualExposureBracketedStillImageSettings.Create(AVCaptureDevice.ExposureDurationCurrent, AVCaptureDevice.ISOCurrent)
					};
				} else {
					bracketedSettings = new AVCaptureBracketedStillImageSettings []{
						AVCaptureAutoExposureBracketedStillImageSettings.Create(AVCaptureDevice.ExposureTargetBiasCurrent)
					};
				}
				if (rawEnabled && photoOutput.AvailableRawPhotoPixelFormatTypes.Length > 0) {
					photoSettings = AVCapturePhotoBracketSettings.FromRawPixelFormatType (photoOutput.AvailableRawPhotoPixelFormatTypes [0].UInt32Value, null, bracketedSettings);
				} else {
					// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=44111
					photoSettings = AVCapturePhotoBracketSettings.FromRawPixelFormatType (0, new NSDictionary<NSString, NSObject> (AVVideo.CodecKey, new NSNumber ((int)AVVideoCodec.JPEG)), bracketedSettings);
				}

				((AVCapturePhotoBracketSettings)photoSettings).IsLensStabilizationEnabled = true;
			} else {
				if (rawEnabled && photoOutput.AvailableRawPhotoPixelFormatTypes.Length > 0) {
					photoSettings = AVCapturePhotoSettings.FromRawPixelFormatType (photoOutput.AvailableRawPhotoPixelFormatTypes [0].UInt32Value);
				} else {
					photoSettings = AVCapturePhotoSettings.Create ();
				}

				// We choose not to use flash when doing manual exposure
				if (VideoDevice.ExposureMode == AVCaptureExposureMode.Custom) {
					photoSettings.FlashMode = AVCaptureFlashMode.Off;
				} else {
					photoSettings.FlashMode = photoOutput.SupportedFlashModes.Contains (new NSNumber ((long)AVCaptureFlashMode.Auto)) ? AVCaptureFlashMode.Auto : AVCaptureFlashMode.Off;
				}
			}

			// The first format in the array is the preferred format
			if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Length > 0)
				photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject> (CVPixelBuffer.PixelFormatTypeKey, photoSettings.AvailablePreviewPhotoPixelFormatTypes [0]);

			if (VideoDevice.ExposureMode == AVCaptureExposureMode.Custom)
				photoSettings.IsAutoStillImageStabilizationEnabled = false;

			photoSettings.IsHighResolutionPhotoEnabled = true;
			return photoSettings;
		}

		[Action ("resumeInterruptedSession:")]
		void ResumeInterruptedSession (NSObject sender)
		{
			// The session might fail to start running, e.g. if a phone or FaceTime call is still using audio or video.
			// A failure to start the session will be communicated via a session runtime error notification.
			// To avoid repeatedly failing to start the session running, we only try to restart the session in the
			// session runtime error handler if we aren't trying to resume the session running.
			sessionQueue.DispatchAsync (() => {
				Session.StartRunning ();
				sessionRunning = Session.Running;
				if (!Session.Running) {
					DispatchQueue.MainQueue.DispatchAsync (() => {
						var message = "Unable to resume";
						UIAlertController alertController = UIAlertController.Create ("AVCamManual", message, UIAlertControllerStyle.Alert);
						UIAlertAction cancelAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
						alertController.AddAction (cancelAction);
						PresentViewController (alertController, true, null);
					});
				} else {
					DispatchQueue.MainQueue.DispatchAsync (() => ResumeButton.Hidden = true);
				}
			});
		}

		[Action ("changeCaptureMode:")]
		void ChangeCaptureMode (UISegmentedControl captureModeControl)
		{
			if (captureModeControl.SelectedSegment == (int)CaptureMode.Photo) {
				RecordButton.Enabled = false;

				// Remove the AVCaptureMovieFileOutput from the session because movie recording is not supported with AVCaptureSessionPresetPhoto. Additionally, Live Photo
				// capture is not supported when an AVCaptureMovieFileOutput is connected to the session.
				sessionQueue.DispatchAsync (() => {
					Session.BeginConfiguration ();
					Session.RemoveOutput (movieFileOutput);
					Session.SessionPreset = AVCaptureSession.PresetPhoto;
					Session.CommitConfiguration ();

					movieFileOutput = null;
				});
			} else if (captureModeControl.SelectedSegment == (int)CaptureMode.Movie) {
				sessionQueue.DispatchAsync (() => {
					var mfo = new AVCaptureMovieFileOutput ();
					if (Session.CanAddOutput (mfo)) {
						Session.BeginConfiguration ();
						Session.AddOutput (mfo);
						Session.SessionPreset = AVCaptureSession.PresetHigh;
						AVCaptureConnection connection = mfo.ConnectionFromMediaType (AVMediaType.Video);
						if (connection.SupportsVideoStabilization)
							connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
						Session.CommitConfiguration ();
						movieFileOutput = mfo;

						DispatchQueue.MainQueue.DispatchAsync (() => {
							RecordButton.Enabled = true;
						});
					}
				});
			}
		}

		#endregion

		#region Device Configuration

		[Export ("changeCamera:")]
		void OnCameraChangeClicked (NSObject sender)
		{
			ManualHUD.UserInteractionEnabled = false;
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			PhotoButton.Enabled = false;
			CaptureModeControl.Enabled = false;
			HUDButton.Enabled = false;

			sessionQueue.DispatchAsync (() => {
				AVCaptureDevicePosition preferredPosition = AVCaptureDevicePosition.Unspecified;
				switch (VideoDevice.Position) {
				case AVCaptureDevicePosition.Back:
					preferredPosition = AVCaptureDevicePosition.Front;
					break;

				case AVCaptureDevicePosition.Unspecified:
				case AVCaptureDevicePosition.Front:
					preferredPosition = AVCaptureDevicePosition.Back;
					break;
				}

				AVCaptureDevice newVideoDevice = GetDeviceFrom (AVMediaType.Video, preferredPosition);
				AVCaptureDeviceInput newVideoDeviceInput = AVCaptureDeviceInput.FromDevice (newVideoDevice);

				Session.BeginConfiguration ();

				// Remove the existing device input first, since using the front and back camera simultaneously is not supported
				Session.RemoveInput (VideoDeviceInput);
				if (Session.CanAddInput (newVideoDeviceInput)) {
					subjectAreaDidChangeToken?.Dispose ();
					subjectAreaDidChangeToken = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, newVideoDevice);

					Session.AddInput (newVideoDeviceInput);
					VideoDeviceInput = newVideoDeviceInput;
					VideoDevice = newVideoDeviceInput.Device;
				} else {
					Session.AddInput (VideoDeviceInput);
				}

				if (movieFileOutput != null) {
					AVCaptureConnection connection = movieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
					if (connection.SupportsVideoStabilization)
						connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
				}

				Session.CommitConfiguration ();

				DispatchQueue.MainQueue.DispatchAsync (() => {
					ConfigureManualHUD ();
					CameraButton.Enabled = true;
					RecordButton.Enabled = (CaptureModeControl.SelectedSegment == (int)CaptureMode.Movie);
					PhotoButton.Enabled = true;
					CaptureModeControl.Enabled = true;
					HUDButton.Enabled = true;
					ManualHUD.UserInteractionEnabled = true;
				});
			});
		}

		static AVCaptureDevice GetDeviceFrom (string mediaType, AVCaptureDevicePosition position)
		{
			AVCaptureDevice [] devices = AVCaptureDevice.DevicesWithMediaType (mediaType);
			AVCaptureDevice captureDevice = devices.FirstOrDefault (d => d.Position == position);
			return captureDevice;
		}

		[Export ("changeFocusMode:")]
		public void OnChangeFocusModeClicked (NSObject sender)
		{
			var control = (UISegmentedControl)sender;
			AVCaptureFocusMode mode = focusModes [control.SelectedSegment];

			NSError error = null;

			bool success = VideoDevice.LockForConfiguration (out error);
			if (!success) {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
				return;
			}

			if (VideoDevice.IsFocusModeSupported (mode)) {
				VideoDevice.FocusMode = mode;
			} else {
				Console.WriteLine ($"Focus mode {StringFromFocusMode (mode)} is not supported. Focus mode is {StringFromFocusMode (VideoDevice.FocusMode)}.");
				FocusModeControl.SelectedSegment = Array.IndexOf (focusModes, VideoDevice.FocusMode);
			}

			VideoDevice.UnlockForConfiguration ();
		}

		[Export ("changeLensPosition:")]
		public void OnChangeLensPositionClicked (NSObject sender)
		{
			var control = (UISlider)sender;
			NSError error = null;

			if (VideoDevice.LockForConfiguration (out error)) {
				VideoDevice.SetFocusModeLocked (control.Value, null);
				VideoDevice.UnlockForConfiguration ();
			} else {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
			}
		}

		void SetFocusAndMode (AVCaptureFocusMode focusMode, AVCaptureExposureMode exposureMode, CGPoint point, bool monitorSubjectAreaChange)
		{
			sessionQueue.DispatchAsync (() => {
				AVCaptureDevice device = VideoDevice;
				NSError error = null;
				if (device.LockForConfiguration (out error)) {
					// Setting (Focus|Exposure)PointOfInterest alone does not initiate a (focus/exposure) operation
					// Set (Focus|Exposure)Mode to apply the new point of interest
					if (focusMode != AVCaptureFocusMode.Locked && device.FocusPointOfInterestSupported && device.IsFocusModeSupported (focusMode)) {
						device.FocusMode = focusMode;
						device.FocusPointOfInterest = point;
					}
					if (exposureMode != AVCaptureExposureMode.Custom && device.ExposurePointOfInterestSupported && device.IsExposureModeSupported (exposureMode)) {
						device.ExposureMode = exposureMode;
						device.ExposurePointOfInterest = point;
					}
					device.SubjectAreaChangeMonitoringEnabled = monitorSubjectAreaChange;
					device.UnlockForConfiguration ();
				} else {
					Console.WriteLine ($"Could not lock device for configuration: {error}");
				}
			});
		}

		[Export ("focusAndExposeTap:")]
		void OnFocusAndExposeClicked (UIGestureRecognizer gestureRecognizer)
		{
			CGPoint devicePoint = ((AVCaptureVideoPreviewLayer)PreviewView.Layer).CaptureDevicePointOfInterestForPoint (gestureRecognizer.LocationInView (gestureRecognizer.View));
			SetFocusAndMode (VideoDevice.FocusMode, VideoDevice.ExposureMode, devicePoint, true);
		}

		[Export ("changeExposureMode:")]
		public void OnChangeExposureModeClicked (NSObject sender)
		{
			var control = (UISegmentedControl)sender;
			AVCaptureExposureMode mode = exposureModes [control.SelectedSegment];
			NSError error = null;

			bool success = VideoDevice.LockForConfiguration (out error);
			if (!success) {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
				return;
			}

			if (VideoDevice.IsExposureModeSupported (mode)) {
				VideoDevice.ExposureMode = mode;
			} else {
				Console.WriteLine ($"Exposure mode {StringFromExposureMode (mode)} is not supported. Exposure mode is {StringFromExposureMode (VideoDevice.ExposureMode)}.");
				ExposureModeControl.SelectedSegment = Array.IndexOf (exposureModes, VideoDevice.ExposureMode);
			}
			VideoDevice.UnlockForConfiguration ();
		}

		[Export ("changeExposureDuration:")]
		void OnChangeExposureDurationClicked (NSObject sender)
		{
			var control = (UISlider)sender;
			NSError error = null;

			double p = Math.Pow (control.Value, ExposureDurationPower); // Apply power function to expand slider's low-end range
			double minDurationSeconds = Math.Max (VideoDevice.ActiveFormat.MinExposureDuration.Seconds, ExposureMinDuration);
			double maxDurationSeconds = VideoDevice.ActiveFormat.MaxExposureDuration.Seconds;
			double newDurationSeconds = p * (maxDurationSeconds - minDurationSeconds) + minDurationSeconds; // Scale from 0-1 slider range to actual duration

			if (VideoDevice.LockForConfiguration (out error)) {
				VideoDevice.LockExposure (CMTime.FromSeconds (newDurationSeconds, 1000 * 1000 * 1000), AVCaptureDevice.ISOCurrent, null);
				VideoDevice.UnlockForConfiguration ();
			} else {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
			}
		}

		[Export ("changeISO:")]
		public void OnChangeISOClicked (NSObject sender)
		{
			var control = (UISlider)sender;
			NSError error = null;

			if (VideoDevice.LockForConfiguration (out error)) {
				VideoDevice.LockExposure (AVCaptureDevice.ExposureDurationCurrent, control.Value, null);
				VideoDevice.UnlockForConfiguration ();
			} else {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
			}
		}

		[Export ("changeExposureTargetBias:")]
		public void OnChangeExposureTargetBiasClicked (NSObject sender)
		{
			var control = (UISlider)sender;
			NSError error = null;

			if (VideoDevice.LockForConfiguration (out error)) {
				VideoDevice.SetExposureTargetBias (control.Value, null);
				VideoDevice.UnlockForConfiguration ();
			} else {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
			}
		}

		[Export ("changeWhiteBalanceMode:")]
		public void OnChangeWhiteBalanceModeClicked (NSObject sender)
		{
			var control = (UISegmentedControl)sender;
			AVCaptureWhiteBalanceMode mode = whiteBalanceModes [control.SelectedSegment];
			NSError error = null;

			var success = VideoDevice.LockForConfiguration (out error);
			if (!success) {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
				return;
			}

			if (VideoDevice.IsWhiteBalanceModeSupported (mode)) {
				VideoDevice.WhiteBalanceMode = mode;
			} else {
				Console.WriteLine ($"White balance mode {StringFromWhiteBalanceMode (mode)} is not supported. White balance mode is {StringFromWhiteBalanceMode (VideoDevice.WhiteBalanceMode)}.");
				WhiteBalanceModeControl.SelectedSegment = Array.IndexOf (whiteBalanceModes, VideoDevice.WhiteBalanceMode);
			}
			VideoDevice.UnlockForConfiguration ();
		}

		void SetWhiteBalanceGains (AVCaptureWhiteBalanceGains gains)
		{
			NSError error = null;

			if (VideoDevice.LockForConfiguration (out error)) {
				AVCaptureWhiteBalanceGains newGains = NormalizeGains (gains); // Conversion can yield out-of-bound values, cap to limits
				VideoDevice.SetWhiteBalanceModeLockedWithDeviceWhiteBalanceGains (newGains, null);
				VideoDevice.UnlockForConfiguration ();
			} else {
				Console.WriteLine ($"Could not lock device for configuration: {error}");
			}
		}

		[Export ("changeTemperature:")]
		public void OnChangeTemperatureClicked (NSObject sender)
		{
			var temperatureAndTint = new AVCaptureWhiteBalanceTemperatureAndTintValues {
				Temperature = TemperatureSlider.Value,
				Tint = TintSlider.Value
			};

			SetWhiteBalanceGains (VideoDevice.GetDeviceWhiteBalanceGains (temperatureAndTint));
		}

		[Export ("changeTint:")]
		public void OnChangeTintClicked (NSObject sender)
		{
			var temperatureAndTint = new AVCaptureWhiteBalanceTemperatureAndTintValues {
				Temperature = TemperatureSlider.Value,
				Tint = TintSlider.Value
			};

			SetWhiteBalanceGains (VideoDevice.GetDeviceWhiteBalanceGains (temperatureAndTint));
		}

		[Export ("lockWithGrayWorld:")]
		public void OnLockWithGrayWorldClicked (NSObject sender)
		{
			SetWhiteBalanceGains (VideoDevice.GrayWorldDeviceWhiteBalanceGains);
		}

		AVCaptureWhiteBalanceGains NormalizeGains (AVCaptureWhiteBalanceGains gains)
		{
			gains.RedGain = Math.Max (1, gains.RedGain);
			gains.GreenGain = Math.Max (1, gains.GreenGain);
			gains.BlueGain = Math.Max (1, gains.BlueGain);

			float maxGain = VideoDevice.MaxWhiteBalanceGain;
			gains.RedGain = Math.Min (maxGain, gains.RedGain);
			gains.GreenGain = Math.Min (maxGain, gains.GreenGain);
			gains.BlueGain = Math.Min (maxGain, gains.BlueGain);

			return gains;
		}

		#endregion

		#region Capturing Photos

		[Action ("capturePhoto:")]
		void CapturePhoto (NSObject sender)
		{
			// Retrieve the video preview layer's video orientation on the main queue before entering the session queue
			// We do this to ensure UI elements are accessed on the main thread and session configuration is done on the session queue
			var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
			AVCaptureVideoOrientation videoPreviewLayerVideoOrientation = previewLayer.Connection.VideoOrientation;

			AVCapturePhotoSettings settings = GetCurrentPhotoSettings ();
			sessionQueue.DispatchAsync (() => {
				// Update the orientation on the photo output video connection before capturing
				AVCaptureConnection photoOutputConnection = photoOutput.ConnectionFromMediaType (AVMediaType.Video);
				photoOutputConnection.VideoOrientation = videoPreviewLayerVideoOrientation;
				photoOutput.CapturePhoto (settings, this);
			});
		}

		[Export ("captureOutput:willCapturePhotoForResolvedSettings:")]
		void WillCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				PreviewView.Layer.Opacity = 0;
				UIView.Animate (0.25, () => {
					PreviewView.Layer.Opacity = 1;
				});
			});
		}

		[Export ("captureOutput:didFinishProcessingPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput,
									   CMSampleBuffer photoSampleBuffer, CMSampleBuffer previewPhotoSampleBuffer,
									   AVCaptureResolvedPhotoSettings resolvedSettings, AVCaptureBracketedStillImageSettings bracketSettings,
									   NSError error)
		{
			if (photoSampleBuffer == null) {
				Console.WriteLine ($"Error occurred while capturing photo: {error}");
				return;
			}

			NSData imageData = AVCapturePhotoOutput.GetJpegPhotoDataRepresentation (photoSampleBuffer, previewPhotoSampleBuffer);
			PHPhotoLibrary.RequestAuthorization (status => {
				if (status == PHAuthorizationStatus.Authorized) {
					PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
						PHAssetCreationRequest.CreationRequestForAsset ().AddResource (PHAssetResourceType.Photo, imageData, null);
					}, (success, err) => {
						if (!success) {
							Console.WriteLine ($"Error occurred while saving photo to photo library: {err}");
						} else {
							Console.WriteLine ("Photo was saved to photo library");
						}
					});
				} else {
					Console.WriteLine ("Not authorized to save photo");
				}
			});
		}

		[Export ("captureOutput:didFinishProcessingRawPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingRawPhoto (AVCapturePhotoOutput captureOutput,
										  CMSampleBuffer rawSampleBuffer, CMSampleBuffer previewPhotoSampleBuffer,
										  AVCaptureResolvedPhotoSettings resolvedSettings, AVCaptureBracketedStillImageSettings bracketSettings,
										  NSError error)
		{
			if (rawSampleBuffer == null) {
				Console.WriteLine ($"Error occurred while capturing photo: {error}");
				return;
			}

			var filePath = Path.Combine (Path.GetTempPath (), $"{resolvedSettings.UniqueID}.dng");
			NSData imageData = AVCapturePhotoOutput.GetDngPhotoDataRepresentation (rawSampleBuffer, previewPhotoSampleBuffer);
			imageData.Save (filePath, true);

			PHPhotoLibrary.RequestAuthorization (status => {
				if (status == PHAuthorizationStatus.Authorized) {
					PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
						// In iOS 9 and later, it's possible to move the file into the photo library without duplicating the file data.
						// This avoids using double the disk space during save, which can make a difference on devices with limited free disk space.
						var options = new PHAssetResourceCreationOptions ();
						options.ShouldMoveFile = true;
						PHAssetCreationRequest.CreationRequestForAsset ().AddResource (PHAssetResourceType.Photo, new NSUrl(filePath), options); // Add move (not copy) option
					}, (success, err) => {
						if (!success)
							Console.WriteLine ($"Error occurred while saving raw photo to photo library: {err}");
						else
							Console.WriteLine ("Raw photo was saved to photo library");

						NSError rErr;
						if (NSFileManager.DefaultManager.FileExists (filePath))
							NSFileManager.DefaultManager.Remove (filePath, out rErr);
					});
				} else {
					Console.WriteLine ("Not authorized to save photo");
				}
			});
		}

		#endregion

		#region Recording Movies

		[Export ("toggleMovieRecording:")]
		public void OnRecordClicked (NSObject sender)
		{
			// Disable the Camera button until recording finishes, and disable the Record button until recording starts or finishes (see the AVCaptureFileOutputRecordingDelegate methods)
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			CaptureModeControl.Enabled = false;

			// Retrieve the video preview layer's video orientation on the main queue before entering the session queue. We do this to ensure UI
			// elements are accessed on the main thread and session configuration is done on the session queue.
			var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
			AVCaptureVideoOrientation previewLayerVideoOrientation = previewLayer.Connection.VideoOrientation;

			sessionQueue.DispatchAsync (() => {
				if (!movieFileOutput.Recording) {
					// tODO: fix comment
					// Setup background task. This is needed because the -[captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:]
					// callback is not received until AVCamManual returns to the foreground unless you request background execution time.
					// This also ensures that there will be time to write the file to the photo library when AVCamManual is backgrounded.
					// To conclude this background execution, -endBackgroundTask is called in
					// -[captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:] after the recorded file has been saved.
					if (UIDevice.CurrentDevice.IsMultitaskingSupported)
						backgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask (null);

					var connection = movieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
					connection.VideoOrientation = PreviewLayer.Connection.VideoOrientation;

					// Start recording to a temporary file.
					string movFile = Path.ChangeExtension (Path.GetRandomFileName (), "mov");
					string outputFilePath = Path.Combine (Path.GetTempPath (), movFile);
					movieFileOutput.StartRecordingToOutputFile (NSUrl.FromFilename (outputFilePath), this);
				} else {
					movieFileOutput.StopRecording ();
				}
			});
		}

		[Export ("captureOutput:didStartRecordingToOutputFileAtURL:fromConnections:")]
		public void DidStartRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections)
		{
			// Enable the Record button to let the user stop the recording
			DispatchQueue.MainQueue.DispatchAsync (() => {
				RecordButton.Enabled = true;
				RecordButton.SetTitle ("Stop", UIControlState.Normal);
			});
		}

		public void FinishedRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections, NSError error)
		{
			// Note that currentBackgroundRecordingID is used to end the background task associated with this recording.
			// This allows a new recording to be started, associated with a new UIBackgroundTaskIdentifier, once the movie file output's Recording property
			// is back to false — which happens sometime after this method returns.
			// Note: Since we use a unique file path for each recording, a new recording will not overwrite a recording currently being saved.
			var currentBackgroundRecordingID = backgroundRecordingID;
			backgroundRecordingID = -1;

			Action cleanup = () => {
				NSError err;
				if (NSFileManager.DefaultManager.FileExists (outputFileUrl.Path))
					NSFileManager.DefaultManager.Remove (outputFileUrl, out err);

				if (currentBackgroundRecordingID != -1)
					UIApplication.SharedApplication.EndBackgroundTask (currentBackgroundRecordingID);
			};

			var success = true;

			if (error != null) {
				Console.WriteLine ($"Error occurred while capturing movie: {error}");
				success = error.UserInfo [AVErrorKeys.RecordingSuccessfullyFinished].AsBool ();
			}
			if (success) {
				PHPhotoLibrary.RequestAuthorization (status => {
					if (status == PHAuthorizationStatus.Authorized) {
						// Save the movie file to the photo library and cleanup
						PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
							// In iOS 9 and later, it's possible to move the file into the photo library without duplicating the file data.
							// This avoids using double the disk space during save, which can make a difference on devices with limited free disk space.
							var options = new PHAssetResourceCreationOptions { ShouldMoveFile = true };
							PHAssetCreationRequest changeRequest = PHAssetCreationRequest.CreationRequestForAsset ();
							changeRequest.AddResource (PHAssetResourceType.Video, outputFileUrl, options);
						}, (result, err) => {
							if (!result)
								Console.WriteLine ($"Could not save movie to photo library: {err}");
							cleanup ();
						});
					} else {
						cleanup ();
					}
				});
			} else {
				cleanup ();
			}

			// Enable the Camera and Record buttons to let the user switch camera and start another recording
			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Only enable the ability to change camera if the device has more than one camera
				CameraButton.Enabled = (AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video).Length > 1);
				RecordButton.Enabled = CaptureModeControl.SelectedSegment == (int)CaptureMode.Movie;
				RecordButton.SetTitle ("Record", UIControlState.Normal);
				CaptureModeControl.Enabled = true;
			});
		}

		#endregion

		#region KVO

		void AddObservers ()
		{
			// To learn more about KVO visit:
			// http://tirania.org/monomac/archive/2012/Apr-19.html?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed%3A+MiguelsOsxAndIosBlog+(Miguel%27s+OSX+and+iOS+blog)
			runningToken = AddObserver ("session.running", NSKeyValueObservingOptions.New, SessionRunningChanged);
			focusModeToken = AddObserver ("videoDevice.focusMode", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, FocusModeChanged);
			lensPositionToken = AddObserver ("videoDevice.lensPosition", NSKeyValueObservingOptions.New, LensPositionChanged);
			exposureModeToken = AddObserver ("videoDevice.exposureMode", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, ExposureModeChanged);
			exposureDurationToken = AddObserver ("videoDevice.exposureDuration", NSKeyValueObservingOptions.New, ExposureDurationChanged); 
			isoToken = AddObserver ("videoDevice.ISO", NSKeyValueObservingOptions.New, ISOChanged);
			exposureTargetBiasToken = AddObserver ("videoDevice.exposureTargetBias", NSKeyValueObservingOptions.New, ExposureTargetBiasChanged);
			exposureTargetOffsetToken = AddObserver ("videoDevice.exposureTargetOffset", NSKeyValueObservingOptions.New, ExposureTargetOffsetChanged);
			whiteBalanceModeToken = AddObserver ("videoDevice.whiteBalanceMode", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, WhiteBalanceModeChange);
			deviceWhiteBalanceGainsToken = AddObserver ("videoDevice.deviceWhiteBalanceGains", NSKeyValueObservingOptions.New, DeviceWhiteBalanceGainsChange);


			subjectAreaDidChangeToken = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, VideoDevice);
			runtimeErrorToken = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.RuntimeErrorNotification, SessionRuntimeError, Session);
			// A session can only run when the app is full screen. It will be interrupted in a multi-app layout, introduced in iOS 9,
			// see also the documentation of AVCaptureSessionInterruptionReason. Add observers to handle these session interruptions
			// and show a preview is paused message. See the documentation of AVCaptureSessionWasInterruptedNotification for other
			// interruption reasons.
			wasInterruptedToken =  NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.WasInterruptedNotification, SessionWasInterrupted, Session);
			interruptionEndedToken =  NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.InterruptionEndedNotification, SessionInterruptionEnded, Session);
		}

		void RemoveObservers ()
		{
			subjectAreaDidChangeToken.Dispose ();
			runtimeErrorToken.Dispose ();
			wasInterruptedToken.Dispose ();
			interruptionEndedToken.Dispose ();

			runningToken.Dispose ();
			focusModeToken.Dispose ();
			lensPositionToken.Dispose ();
			exposureModeToken.Dispose ();
			exposureDurationToken.Dispose ();
			isoToken.Dispose ();
			exposureTargetBiasToken.Dispose ();
			exposureTargetOffsetToken.Dispose ();
			whiteBalanceModeToken.Dispose ();
			deviceWhiteBalanceGainsToken.Dispose ();
		}

		void SessionRunningChanged (NSObservedChange obj)
		{
			var isRunning = false;
			if (obj.NewValue != null && obj.NewValue != NSNull.Null)
				isRunning = obj.NewValue.AsBool ();

			DispatchQueue.MainQueue.DispatchAsync (() => {
				CameraButton.Enabled = isRunning && (AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video).Length > 1);
				RecordButton.Enabled = isRunning && (CaptureModeControl.SelectedSegment == (int)CaptureMode.Movie);
				PhotoButton.Enabled = isRunning;
				HUDButton.Enabled = isRunning;
				CaptureModeControl.Enabled = isRunning;
			});
		}

		void FocusModeChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			var oldValue = obj.OldValue;

			if (newValue != null && newValue != NSNull.Null) {
				var newMode = (AVCaptureFocusMode)newValue.AsInt ();
				DispatchQueue.MainQueue.DispatchAsync (() => {
					FocusModeControl.SelectedSegment = Array.IndexOf (focusModes, newMode);
					LensPositionSlider.Enabled = (newMode == AVCaptureFocusMode.Locked);

					if (oldValue != null && oldValue != NSNull.Null) {
						var oldMode = (AVCaptureFocusMode)oldValue.AsInt ();
						Console.WriteLine ($"focus mode: {StringFromFocusMode (oldMode)} -> {StringFromFocusMode (newMode)}");
					} else {
						Console.WriteLine ($"focus mode: {StringFromFocusMode (newMode)}");
					}
				});
			}
		}

		void LensPositionChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			if (newValue != null && newValue != NSNull.Null) {
				AVCaptureFocusMode focusMode = VideoDevice.FocusMode;
				float newLensPosition = newValue.AsFloat ();
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (focusMode != AVCaptureFocusMode.Locked)
						LensPositionSlider.Value = newLensPosition;
					LensPositionValueLabel.Text = newLensPosition.ToString ("F1", CultureInfo.InvariantCulture);
				});
			}
		}

		void ExposureModeChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			var oldValue = obj.OldValue;

			if (newValue != null && newValue != NSNull.Null) {
				var newMode = (AVCaptureExposureMode)newValue.AsInt ();
				if (oldValue != null && oldValue != NSNull.Null) {
					var oldMode = (AVCaptureExposureMode)oldValue.AsInt ();

					// It’s important to understand the relationship between ExposureDuration and the minimum frame rate as represented by ActiveVideoMaxFrameDuration.
					// In manual mode, if ExposureDuration is set to a value that's greater than ActiveVideoMaxFrameDuration, then ActiveVideoMaxFrameDuration will
					// increase to match it, thus lowering the minimum frame rate. If ExposureMode is then changed to automatic mode, the minimum frame rate will
					// remain lower than its default. If this is not the desired behavior, the min and max frameRates can be reset to their default values for the
					// current ActiveFormat by setting ActiveVideoMaxFrameDuration and ActiveVideoMinFrameDuration to CMTime.Invalid.
					if (oldMode != newMode && oldMode == AVCaptureExposureMode.Custom) {
						NSError error = null;
						if (VideoDevice.LockForConfiguration (out error)) {
							VideoDevice.ActiveVideoMaxFrameDuration = CMTime.Invalid;
							VideoDevice.ActiveVideoMinFrameDuration = CMTime.Invalid;
							VideoDevice.UnlockForConfiguration ();
						} else {
							Console.WriteLine ($"Could not lock device for configuration: {error}");
						}
					}
				}
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ExposureModeControl.SelectedSegment = Array.IndexOf (exposureModes, newMode);
					ExposureDurationSlider.Enabled = (newMode == AVCaptureExposureMode.Custom);
					ISOSlider.Enabled = (newMode == AVCaptureExposureMode.Custom);

					if (oldValue != null && oldValue != NSNull.Null) {
						var oldMode = (AVCaptureExposureMode)oldValue.AsInt ();
						Console.WriteLine ($"exposure mode: {StringFromExposureMode (oldMode)} -> {StringFromExposureMode (newMode)}");
					} else {
						Console.WriteLine ($"exposure mode: {StringFromExposureMode (newMode)}");
					}
				});
			}
		}

		void ExposureDurationChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			var oldValue = obj.OldValue;

			if (newValue != null && newValue != NSNull.Null) {
				double newDurationSeconds = newValue.AsCMTime ().Seconds;
				AVCaptureExposureMode exposureMode = VideoDevice.ExposureMode;

				double minDurationSeconds = Math.Max (VideoDevice.ActiveFormat.MinExposureDuration.Seconds, ExposureMinDuration);
				double maxDurationSeconds = VideoDevice.ActiveFormat.MaxExposureDuration.Seconds;
				// Map from duration to non-linear UI range 0-1
				double p = (newDurationSeconds - minDurationSeconds) / (maxDurationSeconds - minDurationSeconds); // Scale to 0-1
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (exposureMode != AVCaptureExposureMode.Custom)
						ExposureDurationSlider.Value = (float)Math.Pow(p ,1 / ExposureDurationPower); // Apply inverse power
					ExposureDurationValueLabel.Text = FormatDuration (newDurationSeconds);
				});
			}
		}

		void ISOChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			if (newValue != null && newValue != NSNull.Null) {
				float newISO = newValue.AsFloat ();
				AVCaptureExposureMode exposureMode = VideoDevice.ExposureMode;

				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (exposureMode != AVCaptureExposureMode.Custom)
						ISOSlider.Value = newISO;
					ISOValueLabel.Text = ((int)newISO).ToString (CultureInfo.InvariantCulture);
				});
			}
		}

		void ExposureTargetBiasChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			if (newValue != null && newValue != NSNull.Null) {
				float newExposureTargetBias = newValue.AsFloat ();
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ExposureTargetBiasValueLabel.Text = newExposureTargetBias.ToString ("F1", CultureInfo.InvariantCulture);
				});
			}
		}

		void ExposureTargetOffsetChanged (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			if (newValue != null && newValue != NSNull.Null) {
				float newExposureTargetOffset = newValue.AsFloat ();
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ExposureTargetOffsetSlider.Value = newExposureTargetOffset;
					ExposureTargetOffsetValueLabel.Text = newExposureTargetOffset.ToString ("F1", CultureInfo.InvariantCulture);
				});
			}
		}

		void WhiteBalanceModeChange (NSObservedChange obj)
		{
			var newValue = obj.NewValue;
			var oldValue = obj.OldValue;

			if (newValue != null && newValue != NSNull.Null) {
				var newMode = (AVCaptureWhiteBalanceMode)newValue.AsInt ();
				DispatchQueue.MainQueue.DispatchAsync (() => {
					WhiteBalanceModeControl.SelectedSegment = Array.IndexOf (whiteBalanceModes, newMode);
					TemperatureSlider.Enabled = (newMode == AVCaptureWhiteBalanceMode.Locked);
					TintSlider.Enabled = (newMode == AVCaptureWhiteBalanceMode.Locked);

					if (oldValue != null && oldValue != NSNull.Null) {
						var oldMode = (AVCaptureWhiteBalanceMode)oldValue.AsInt ();
						Console.WriteLine ($"white balance mode: {StringFromWhiteBalanceMode (oldMode)} -> {StringFromWhiteBalanceMode (newMode)}");
					}
				});
			}
		}

		unsafe void DeviceWhiteBalanceGainsChange (NSObservedChange obj)
		{
			var gains = (NSValue)obj.NewValue;
			if (gains != null) {
				AVCaptureWhiteBalanceGains newGains;
				gains.StoreValueAtAddress ((IntPtr)(void*)&newGains);

				AVCaptureWhiteBalanceTemperatureAndTintValues newTemperatureAndTint = VideoDevice.GetTemperatureAndTintValues (newGains);
				AVCaptureWhiteBalanceMode whiteBalanceMode = VideoDevice.WhiteBalanceMode;
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (whiteBalanceMode != AVCaptureWhiteBalanceMode.Locked) {
						TemperatureSlider.Value = newTemperatureAndTint.Temperature;
						TintSlider.Value = newTemperatureAndTint.Tint;
					}

					var ci = CultureInfo.InvariantCulture;
					TemperatureValueLabel.Text = ((int)newTemperatureAndTint.Temperature).ToString (ci);
					TintValueLabel.Text = ((int)newTemperatureAndTint.Tint).ToString (ci);
				});
			}
		}

		void SubjectAreaDidChange (NSNotification obj)
		{
			var devicePoint = new CGPoint (0.5f, 0.5f);
			SetFocusAndMode (VideoDevice.FocusMode, VideoDevice.ExposureMode, devicePoint, false);
		}

		void SessionRuntimeError (NSNotification notification)
		{
			var error = (NSError)notification.UserInfo [AVCaptureSession.ErrorKey];
			Console.WriteLine ($"Capture session runtime error: {error}");

			if (error.Code == (long)AVError.MediaServicesWereReset) {
				sessionQueue.DispatchAsync (() => {
					// If we aren't trying to resume the session, try to restart it, since it must have been stopped due to an error (see -[resumeInterruptedSession:])
					if (sessionRunning) {
						Session.StartRunning ();
						sessionRunning = Session.Running;
					} else {
						DispatchQueue.MainQueue.DispatchAsync (() => {
							ResumeButton.Hidden = false;
						});
					}
				});
			} else {
				ResumeButton.Hidden = false;
			}
		}

		void SessionWasInterrupted (NSNotification notification)
		{
			// In some scenarios we want to enable the user to restart the capture session.
			// For example, if music playback is initiated via Control Center while using AVCamManual,
			// then the user can let AVCamManual resume the session running, which will stop music playback.
			// Note that stopping music playback in Control Center will not automatically resume the session.
			// Also note that it is not always possible to resume, see ResumeInterruptedSession method.
			// In iOS 9 and later, the notification's UserInfo dictionary contains information about why the session was interrupted
			var reason = (AVCaptureSessionInterruptionReason)notification.UserInfo [AVCaptureSession.InterruptionReasonKey].AsInt ();
			Console.WriteLine ($"Capture session was interrupted with reason {reason}");

			if (reason == AVCaptureSessionInterruptionReason.AudioDeviceInUseByAnotherClient ||
				reason == AVCaptureSessionInterruptionReason.VideoDeviceInUseByAnotherClient) {
				// Simply fade-in a button to enable the user to try to resume the session running
				ResumeButton.Hidden = false;
				ResumeButton.Alpha = 0;
				UIView.Animate (0.25, () => ResumeButton.Alpha = 1);
			} else if (reason == AVCaptureSessionInterruptionReason.VideoDeviceNotAvailableWithMultipleForegroundApps) {
				// Simply fade-in a label to inform the user that the camera is unavailable
				CameraUnavailableLabel.Hidden = false;
				CameraUnavailableLabel.Alpha = 0;
				UIView.Animate (0.25, () => CameraUnavailableLabel.Alpha = 1);
			}
		}

		void SessionInterruptionEnded (NSNotification obj)
		{
			Console.WriteLine ("Capture session interruption ended");

			if (!ResumeButton.Hidden)
				UIView.AnimateNotify (0.25, () => ResumeButton.Alpha = 0, (finished) => ResumeButton.Hidden = true);
			if (!CameraUnavailableLabel.Hidden)
				UIView.AnimateNotify (0.25, () => CameraUnavailableLabel.Alpha = 0, (finished) => CameraUnavailableLabel.Hidden = true);
		}

		string FormatDuration (double duration)
		{
			var ci = CultureInfo.InvariantCulture;

			if (duration <= 0)
				throw new ArgumentOutOfRangeException (nameof (duration));

			if (duration < 1) {
				// e.x. 1/1000 1/350 etc 
				var digits = (int)Math.Max (0, 2 + Math.Floor (Math.Log10 (duration)));
				string pattern = "1/{0:####." + new string ('0', digits) + "}";
				return string.Format (pattern, 1.0 / duration, ci);
			}

			return duration.ToString ("F2", ci);
		}

		#endregion

		#region Utilities

		static string StringFromFocusMode (AVCaptureFocusMode focusMode)
		{
			switch (focusMode) {
			case AVCaptureFocusMode.Locked:
				return "Locked";
			case AVCaptureFocusMode.AutoFocus:
				return "Auto";
			case AVCaptureFocusMode.ContinuousAutoFocus:
				return "ContinuousAuto";
			default:
				return "INVALID FOCUS MODE";
			}
		}

		string StringFromExposureMode (AVCaptureExposureMode exposureMode)
		{
			switch (exposureMode) {
			case AVCaptureExposureMode.Locked:
				return "Locked";
			case AVCaptureExposureMode.AutoExpose:
				return "Auto";
			case AVCaptureExposureMode.ContinuousAutoExposure:
				return "ContinuousAuto";
			case AVCaptureExposureMode.Custom:
				return "Custom";
			default:
				return "INVALID EXPOSURE MODE";
			}
		}

		string StringFromWhiteBalanceMode (AVCaptureWhiteBalanceMode whiteBalanceMode)
		{
			switch (whiteBalanceMode) {
			case AVCaptureWhiteBalanceMode.Locked:
				return "Locked";
			case AVCaptureWhiteBalanceMode.AutoWhiteBalance:
				return "Auto";
			case AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance:
				return "ContinuousAuto";
			default:
				return "INVALID WHITE BALANCE MODE";
			}
		}

		#endregion

	}
}
