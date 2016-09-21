using System;
using System.IO;

using UIKit;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using AVFoundation;
using Photos;
using System.Linq;
using System.Collections.Generic;
using CoreVideo;

namespace AVCam
{
	public enum AVCamSetupResult
	{
		Success,
		CameraNotAuthorized,
		SessionConfigurationFailed
	}

	public enum LivePhotoMode
	{
		On,
		Off
	}

	public enum CaptureMode
	{
		Photo,
		Movie
	}

	[Register ("CameraViewController")]
	public class CameraViewController : UIViewController, IAVCaptureFileOutputRecordingDelegate
	{
		// TODO: ???
		[Outlet]
		PreviewView PreviewView { get; set; }

		[Outlet]
		UILabel CameraUnavailableLabel  { get; set; }

		// TODO: ???
		[Outlet]
		UIButton ResumeButton { get; set; }

		// TODO: ???
		[Outlet]
		UIButton RecordButton { get; set; }

		[Outlet]
		UIButton CameraButton { get; set; }

		// TODO: ???
		[Outlet]
		UIButton StillButton { get; set; }

		[Outlet]
		UIButton PhotoButton { get; set; }

		[Outlet]
		UIButton LivePhotoModeButton { get; set; }

		[Outlet]
		UISegmentedControl CaptureModeControl { get; set; }

		[Outlet]
		UILabel CapturingLivePhotoLabel { get; set; }

		// Communicate with the session and other session objects on this queue.
		readonly DispatchQueue sessionQueue = new DispatchQueue ("session queue");
		readonly AVCaptureSession session = new AVCaptureSession ();

		AVCaptureDeviceInput videoDeviceInput;
		readonly AVCapturePhotoOutput photoOutput = new AVCapturePhotoOutput ();
		AVCaptureMovieFileOutput MovieFileOutput;

		int inProgressLivePhotoCapturesCount;

		readonly Dictionary<long, PhotoCaptureDelegate> inProgressPhotoCaptureDelegates = new Dictionary<long, PhotoCaptureDelegate> ();

		readonly AVCaptureDeviceDiscoverySession videoDeviceDiscoverySession = AVCaptureDeviceDiscoverySession.Create (
			new AVCaptureDeviceType [] { AVCaptureDeviceType.BuiltInWideAngleCamera, AVCaptureDeviceType.BuiltInDuoCamera },
			AVMediaType.Video, AVCaptureDevicePosition.Unspecified);

		// TODO: ???
		AVCaptureStillImageOutput StillImageOutput { get; set; }

		AVCamSetupResult setupResult;
		LivePhotoMode livePhotoMode = LivePhotoMode.Off;

		bool sessionRunning;

		// TODO: ???
		nint backgroundRecordingID;

		IDisposable subjectSubscriber;
		IDisposable runningObserver;
		IDisposable capturingStillObserver;
		IDisposable recordingObserver;
		IDisposable runtimeErrorObserver;
		IDisposable interuptionObserver;
		IDisposable interuptionEndedObserver;

		public CameraViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Disable UI. The UI is enabled if and only if the session starts running.
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			PhotoButton.Enabled = false;
			LivePhotoModeButton.Enabled = false;
			CaptureModeControl.Enabled = false;

			// Setup the preview view.
			PreviewView.Session = session;

			// Check video authorization status. Video access is required and audio access is optional.
			// If audio access is denied, audio is not recorded during movie recording.
			switch (AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video)) {
				// The user has previously granted access to the camera.
				case AVAuthorizationStatus.Authorized:
					break;

				// The user has not yet been presented with the option to grant video access.
				// We suspend the session queue to delay session setup until the access request has completed to avoid
				// asking the user for audio access if video access is denied.
				// Note that audio access will be implicitly requested when we create an AVCaptureDeviceInput for audio during session setup.
				case AVAuthorizationStatus.NotDetermined:
					sessionQueue.Suspend ();
					AVCaptureDevice.RequestAccessForMediaType (AVMediaType.Video, granted => {
						if (!granted)
							setupResult = AVCamSetupResult.CameraNotAuthorized;
						sessionQueue.Resume ();
					});
					break;

				// The user has previously denied access.
				default:
					setupResult = AVCamSetupResult.CameraNotAuthorized;
					break;
			}

			// Setup the capture session.
			// In general it is not safe to mutate an AVCaptureSession or any of its inputs, outputs, or connections from multiple threads at the same time.
			// Why not do all of this on the main queue?
			// Because AVCaptureSession.StartRunning is a blocking call which can take a long time. We dispatch session setup to the sessionQueue
			// so that the main queue isn't blocked, which keeps the UI responsive.
			sessionQueue.DispatchAsync (ConfigureSession);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			sessionQueue.DispatchAsync (() => {
				switch (setupResult) {
				// Only setup observers and start the session running if setup succeeded.
				case AVCamSetupResult.Success:
					AddObservers ();
					session.StartRunning ();
					sessionRunning = session.Running;
					break;

				case AVCamSetupResult.CameraNotAuthorized:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "AVCam doesn't have permission to use the camera, please change privacy settings";
						UIAlertController alertController = UIAlertController.Create ("AVCam", message, UIAlertControllerStyle.Alert);
						UIAlertAction cancelAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
						alertController.AddAction (cancelAction);
						// Provide quick access to Settings.
						alertController.AddAction (UIAlertAction.Create ("Settings", UIAlertActionStyle.Default, action => {
							UIApplication.SharedApplication.OpenUrl (new NSUrl (UIApplication.OpenSettingsUrlString), new NSDictionary (), null);
						}));
						PresentViewController (alertController, true, null);
					});
					break;

				case AVCamSetupResult.SessionConfigurationFailed:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "Unable to capture media";
						UIAlertController alertController = UIAlertController.Create ("AVCam", message, UIAlertControllerStyle.Alert);
						alertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null));
						PresentViewController (alertController, true, null);
					});
					break;
				}
			});
		}

		public override void ViewDidDisappear (bool animated)
		{
			sessionQueue.DispatchAsync (() => {
				if (setupResult == AVCamSetupResult.Success) {
					session.StopRunning ();
					sessionRunning = session.Running;
					RemoveObservers ();
				}
			});
			base.ViewDidDisappear (animated);
		}

		public override bool ShouldAutorotate ()
		{
			// Disable autorotation of the interface when recording is in progress.
			return (MovieFileOutput == null) || !MovieFileOutput.Recording;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			var videoPreviewLayerConnection = PreviewView.VideoPreviewLayer.Connection;
			if (videoPreviewLayerConnection != null) {
				var deviceOrientation = UIDevice.CurrentDevice.Orientation;

				AVCaptureVideoOrientation newVideoOrientation;
				if (!TryConvertToVideoOrientation (deviceOrientation, out newVideoOrientation))
					return;
				if (!deviceOrientation.IsPortrait () && !deviceOrientation.IsLandscape ())
					return;

				videoPreviewLayerConnection.VideoOrientation = newVideoOrientation;
			}
		}

		#region Session Management

		void ConfigureSession ()
		{
			if (setupResult != AVCamSetupResult.Success)
				return;

			session.BeginConfiguration ();

			// We do not create an AVCaptureMovieFileOutput when setting up the session because the
			// AVCaptureMovieFileOutput does not support movie recording with AVCaptureSessionPresetPhoto.
			session.SessionPreset = AVCaptureSession.PresetPhoto;

			// Add video input.
			// Choose the back dual camera if available, otherwise default to a wide angle camera.
			AVCaptureDevice defaultVideoDevice = AVCaptureDevice.GetDefaultDevice (AVCaptureDeviceType.BuiltInDuoCamera, AVMediaType.Video, AVCaptureDevicePosition.Back)
				?? AVCaptureDevice.GetDefaultDevice (AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Back)
				?? AVCaptureDevice.GetDefaultDevice (AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Front);

			NSError error;
			var input = AVCaptureDeviceInput.FromDevice (defaultVideoDevice, out error);
			if (error != null) {
				Console.WriteLine ($"Could not create video device input: {error.LocalizedDescription}");
				setupResult = AVCamSetupResult.SessionConfigurationFailed;
				session.CommitConfiguration ();
				return;
			}

			if (session.CanAddInput (input)) {
				session.AddInput (input);
				videoDeviceInput = input;

				DispatchQueue.MainQueue.DispatchAsync (() => {
					// Why are we dispatching this to the main queue?
					// Because AVCaptureVideoPreviewLayer is the backing layer for PreviewView and UIView
					// can only be manipulated on the main thread.
					// Note: As an exception to the above rule, it is not necessary to serialize video orientation changes
					// on the AVCaptureVideoPreviewLayer’s connection with other session manipulation.
					// Use the status bar orientation as the initial video orientation. Subsequent orientation changes are handled by
					// ViewWillTransitionToSize method.
					var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
					var initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
					AVCaptureVideoOrientation videoOrientation;
					if (statusBarOrientation != UIInterfaceOrientation.Unknown && TryConvertToVideoOrientation(statusBarOrientation, out videoOrientation))
						initialVideoOrientation = videoOrientation;

					PreviewView.VideoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
				});
			} else {
				Console.WriteLine ("Could not add video device input to the session");
				setupResult = AVCamSetupResult.SessionConfigurationFailed;
				session.CommitConfiguration ();
				return;
			}

			// Add audio input.
			var audioDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Audio);
			var audioDeviceInput = AVCaptureDeviceInput.FromDevice (audioDevice, out error);
			if (error != null)
				Console.WriteLine ($"Could not create audio device input: {error.LocalizedDescription}");
			if (session.CanAddInput (audioDeviceInput))
				session.AddInput (audioDeviceInput);
			else
				Console.WriteLine ("Could not add audio device input to the session");

			// Add photo output.
			if (session.CanAddOutput (photoOutput)) {
				session.AddOutput (photoOutput);
				photoOutput.IsHighResolutionCaptureEnabled = true;
				photoOutput.IsLivePhotoCaptureEnabled = photoOutput.IsLivePhotoCaptureSupported;
				livePhotoMode = photoOutput.IsLivePhotoCaptureSupported ? LivePhotoMode.On : LivePhotoMode.Off;
			} else {
				Console.WriteLine ("Could not add photo output to the session");
				setupResult = AVCamSetupResult.SessionConfigurationFailed;
				session.CommitConfiguration ();
				return;
			}
			session.CommitConfiguration ();
		}

		static bool TryGetDefaultVideoCamera (AVCaptureDeviceType type, AVCaptureDevicePosition position, out AVCaptureDevice device)
		{
			device = AVCaptureDevice.GetDefaultDevice (type, AVMediaType.Video, position);
			return device != null;
		}

		[Export ("resumeInterruptedSession:")]
		void ResumeInterruptedSession (CameraViewController sender)
		{
			sessionQueue.DispatchAsync (() => {
				// The session might fail to start running, e.g., if a phone or FaceTime call is still using audio or video.
				// A failure to start the session running will be communicated via a session runtime error notification.
				// To avoid repeatedly failing to start the session running, we only try to restart the session running in the
				// session runtime error handler if we aren't trying to resume the session running.

				session.StartRunning ();
				sessionRunning = session.Running;
				if (!session.Running) {
					DispatchQueue.MainQueue.DispatchAsync (() => {
						const string message = "Unable to resume";
						UIAlertController alertController = UIAlertController.Create ("AVCam", message, UIAlertControllerStyle.Alert);
						UIAlertAction cancelAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
						alertController.AddAction (cancelAction);
						PresentViewController (alertController, true, null);
					});
				} else {
					DispatchQueue.MainQueue.DispatchAsync (() => {
						ResumeButton.Hidden = true;
					});
				}
			});
		}

		[Export ("toggleCaptureMode:")]
		void ToggleCaptureMode (UISegmentedControl captureModeControl)
		{
			if (captureModeControl.SelectedSegment == (int)CaptureMode.Photo) {
				RecordButton.Enabled = false;

				sessionQueue.DispatchAsync (() => {
					// Remove the AVCaptureMovieFileOutput from the session because movie recording is
					// not supported with AVCaptureSessionPresetPhoto. Additionally, Live Photo
					// capture is not supported when an AVCaptureMovieFileOutput is connected to the session.
					session.BeginConfiguration ();
					session.RemoveOutput (MovieFileOutput);
					session.SessionPreset = AVCaptureSession.PresetPhoto;
					session.CommitConfiguration ();

					MovieFileOutput = null;

					if (photoOutput.IsLivePhotoCaptureSupported) {
						photoOutput.IsLivePhotoCaptureEnabled = true;
						DispatchQueue.MainQueue.DispatchAsync (() => {
							LivePhotoModeButton.Enabled = true;
							LivePhotoModeButton.Enabled = false;
						});
					}
				});
			} else if (captureModeControl.SelectedSegment == (int)CaptureMode.Movie) {
				LivePhotoModeButton.Hidden = true;

				sessionQueue.DispatchAsync (() => {
					var output = new AVCaptureMovieFileOutput ();
					if (session.CanAddOutput (output)) {
						session.BeginConfiguration ();
						session.AddOutput (output);
						session.SessionPreset = AVCaptureSession.PresetHigh;
						var connection = output.ConnectionFromMediaType (AVMediaType.Video);
						if (connection != null) {
							if (connection.SupportsVideoStabilization)
								connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
						}
						session.CommitConfiguration ();
						MovieFileOutput = output;

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
		void ChangeCamera (UIButton cameraButton)
		{
			cameraButton.Enabled = false;
			RecordButton.Enabled = false;
			PhotoButton.Enabled = false;
			LivePhotoModeButton.Enabled = false;
			CaptureModeControl.Enabled = false;

			sessionQueue.DispatchAsync (() => {
				AVCaptureDevice currentVideoDevice = videoDeviceInput.Device;
				AVCaptureDevicePosition currentPosition = currentVideoDevice.Position;

			AVCaptureDevicePosition preferredPosition = 0;
			AVCaptureDeviceType preferredDeviceType = 0;

				switch (currentPosition) {
				case AVCaptureDevicePosition.Unspecified:
				case AVCaptureDevicePosition.Front:
					preferredPosition = AVCaptureDevicePosition.Back;
					preferredDeviceType = AVCaptureDeviceType.BuiltInDuoCamera;
					break;

				case AVCaptureDevicePosition.Back:
					preferredPosition = AVCaptureDevicePosition.Front;
					preferredDeviceType = AVCaptureDeviceType.BuiltInWideAngleCamera;
					break;
				}

				var devices = videoDeviceDiscoverySession.Devices;
				AVCaptureDevice newVideoDevice = null;

				// First, look for a device with both the preferred position and device type. Otherwise, look for a device with only the preferred position.
				newVideoDevice = devices.FirstOrDefault (d => d.Position == preferredPosition && d.DeviceType == preferredDeviceType)
							  ?? devices.FirstOrDefault (d => d.Position == preferredPosition);

				if (newVideoDevice != null) {
					NSError error;
					var input = AVCaptureDeviceInput.FromDevice (newVideoDevice, out error);
					if (error == null) {
						session.BeginConfiguration ();

						// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
						session.RemoveInput (input);

						if (session.CanAddInput (input)) {
							subjectSubscriber?.Dispose ();
							subjectSubscriber = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, input.Device);
							session.AddInput (input);
							videoDeviceInput = input;
						} else {
							session.AddInput (videoDeviceInput);
						}

						var connection = MovieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
						if (connection != null) {
							if (connection.SupportsVideoStabilization)
								connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
						}

						// Set Live Photo capture enabled if it is supported.When changing cameras, the
						// IsLivePhotoCaptureEnabled property of the AVCapturePhotoOutput gets set to false when
						// a video device is disconnected from the session.After the new video device is
						// added to the session, re - enable Live Photo capture on the AVCapturePhotoOutput if it is supported.
						photoOutput.IsLivePhotoCaptureEnabled = photoOutput.IsLivePhotoCaptureSupported;
						session.CommitConfiguration ();
					}
				}

				DispatchQueue.MainQueue.DispatchAsync (() => {
					CameraButton.Enabled = true;
					RecordButton.Enabled = MovieFileOutput != null;
					PhotoButton.Enabled = true;
					LivePhotoModeButton.Enabled = true;
					CaptureModeControl.Enabled = true;
				});
			});
		}

		[Export ("focusAndExposeTap:")]
		void FocusAndExposeTap (UIGestureRecognizer gestureRecognizer)
		{
			var location = gestureRecognizer.LocationInView (gestureRecognizer.View);
			CGPoint devicePoint = PreviewView.VideoPreviewLayer.CaptureDevicePointOfInterestForPoint (location);
			UpdateDeviceFocus (AVCaptureFocusMode.AutoFocus, AVCaptureExposureMode.AutoExpose, devicePoint, true);
		}

		void UpdateDeviceFocus (AVCaptureFocusMode focusMode, AVCaptureExposureMode exposureMode, CGPoint point, bool monitorSubjectAreaChange)
		{
			sessionQueue.DispatchAsync (() => {
				var device = videoDeviceInput?.Device;
				if (device == null)
					return;

				NSError error;
				if (device.LockForConfiguration (out error)) {
					// Setting (Focus/Exposure)PointOfInterest alone does not initiate a (focus/exposure) operation.
					// Set (Focus/Exposure)Mode to apply the new point of interest.
					if (device.FocusPointOfInterestSupported && device.IsFocusModeSupported (focusMode)) {
						device.FocusPointOfInterest = point;
						device.FocusMode = focusMode;
					}
					if (device.ExposurePointOfInterestSupported && device.IsExposureModeSupported (exposureMode)) {
						device.ExposurePointOfInterest = point;
						device.ExposureMode = exposureMode;
					}
					device.SubjectAreaChangeMonitoringEnabled = monitorSubjectAreaChange;
					device.UnlockForConfiguration ();
				} else {
					Console.WriteLine ($"Could not lock device for configuration: {error.LocalizedDescription}");
				}
			});
		}

		#endregion

		#region Capturing Photos

		[Export ("capturePhoto:")]
		void CapturePhoto (UIButton photoButton)
		{
			// Retrieve the video preview layer's video orientation on the main queue before
			// entering the session queue. We do this to ensure UI elements are accessed on
			// the main thread and session configuration is done on the session queue.
			var videoPreviewLayerOrientation = PreviewView.VideoPreviewLayer.Connection.VideoOrientation;

			sessionQueue.DispatchAsync (() => {
				// Update the photo output's connection to match the video orientation of the video preview layer.
				var photoOutputConnection = photoOutput.ConnectionFromMediaType (AVMediaType.Video);
				if (photoOutputConnection != null)
					photoOutputConnection.VideoOrientation = videoPreviewLayerOrientation;

				// Capture a JPEG photo with flash set to auto and high resolution photo enabled.
				var photoSettings = AVCapturePhotoSettings.Create ();
				photoSettings.FlashMode = AVCaptureFlashMode.Auto;
				photoSettings.IsHighResolutionPhotoEnabled = true;
				// TODO: request strong typed API
				if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Length > 0)
					photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject> (CVPixelBuffer.PixelFormatTypeKey, photoOutput.AvailablePhotoPixelFormatTypes [0]);
				// Live Photo capture is not supported in movie mode.
				if (livePhotoMode == LivePhotoMode.On && photoOutput.IsLivePhotoCaptureSupported) {
					var livePhotoMovieFileName = new NSUuid ().AsString ();
					var livePhotoMovieFilePath = Path.Combine (Path.GetTempPath (), Path.ChangeExtension (livePhotoMovieFileName, "mov"));
					photoSettings.LivePhotoMovieFileUrl = NSUrl.FromString (livePhotoMovieFilePath);
				}

				// Use a separate object for the photo capture delegate to isolate each capture life cycle.
				var photoCaptureDelegate = new PhotoCaptureDelegate (photoSettings, () => DispatchQueue.MainQueue.DispatchAsync (() => {
					PreviewView.VideoPreviewLayer.Opacity = 0;
					UIView.Animate (0.25, () => PreviewView.VideoPreviewLayer.Opacity = 1);
				}), capturing => {
					// Because Live Photo captures can overlap, we need to keep track of the
					// number of in progress Live Photo captures to ensure that the
					// Live Photo label stays visible during these captures.
					sessionQueue.DispatchAsync (() => {
						if (capturing)
							inProgressLivePhotoCapturesCount += 1;
						else
							inProgressLivePhotoCapturesCount -= 1;

						var count = inProgressLivePhotoCapturesCount;
						DispatchQueue.MainQueue.DispatchAsync (() => {
							if (count > 0)
								CapturingLivePhotoLabel.Hidden = false;
							else if (count == 0)
								CapturingLivePhotoLabel.Hidden = true;
							else
								Console.WriteLine ("Error: In progress live photo capture count is less than 0");
						});
					});
				}, photoDelegate => {
					// When the capture is complete, remove a reference to the photo capture delegate so it can be deallocated.
					sessionQueue.DispatchAsync (() => inProgressPhotoCaptureDelegates.Remove (photoDelegate.RequestedPhotoSettings.UniqueID));
				});

				// The Photo Output keeps a weak reference to the photo capture delegate so
				// we store it in an array to maintain a strong reference to this object
				// until the capture is completed.
				inProgressPhotoCaptureDelegates [photoCaptureDelegate.RequestedPhotoSettings.UniqueID] = photoCaptureDelegate;
				photoOutput.CapturePhoto (photoSettings, photoCaptureDelegate);
			});
		}

		#endregion


		void AddObservers ()
		{
			runningObserver = session.AddObserver ("running", NSKeyValueObservingOptions.New, OnSessionRunningChanged);
			capturingStillObserver = StillImageOutput.AddObserver ("capturingStillImage", NSKeyValueObservingOptions.New, OnCapturingStillImageChanged);
			recordingObserver = MovieFileOutput.AddObserver ("recording", NSKeyValueObservingOptions.New, OnRecordingChanged);

			subjectSubscriber = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, videoDeviceInput.Device);
			runtimeErrorObserver = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.RuntimeErrorNotification, SessionRuntimeError, session);

			// A session can only run when the app is full screen. It will be interrupted in a multi-app layout, introduced in iOS 9.
			// Add observers to handle these session interruptions
			// and show a preview is paused message. See the documentation of AVCaptureSession.WasInterruptedNotification for other
			// interruption reasons.
			interuptionObserver = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.WasInterruptedNotification, SessionWasInterrupted, session);
			interuptionEndedObserver = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.InterruptionEndedNotification, SessionInterruptionEnded, session);
		}

		void RemoveObservers ()
		{
			subjectSubscriber.Dispose ();
			runningObserver.Dispose ();
			capturingStillObserver.Dispose ();
			recordingObserver.Dispose ();
			runtimeErrorObserver.Dispose ();
			interuptionObserver.Dispose ();
			interuptionEndedObserver.Dispose ();
		}

		void OnCapturingStillImageChanged (NSObservedChange change)
		{
			bool isCapturingStillImage = ((NSNumber)change.NewValue).BoolValue;
			if (isCapturingStillImage) {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					PreviewView.Layer.Opacity = 0;
					UIView.Animate (0.25, () => {
						PreviewView.Layer.Opacity = 1;
					});
				});
			}
		}

		void OnRecordingChanged (NSObservedChange change)
		{
			bool isRecording = ((NSNumber)change.NewValue).BoolValue;
			DispatchQueue.MainQueue.DispatchAsync (() => {
				if (isRecording) {
					CameraButton.Enabled = false;
					RecordButton.Enabled = true;
					RecordButton.SetTitle ("Stop", UIControlState.Normal);
				} else {
					// Only enable the ability to change camera if the device has more than one camera.
					CameraButton.Enabled = NumberOfVideoCameras () > 1;
					RecordButton.Enabled = true;
					RecordButton.SetTitle ("Record", UIControlState.Normal);
				}
			});
		}

		void OnSessionRunningChanged (NSObservedChange change)
		{
			bool isSessionRunning = ((NSNumber)change.NewValue).BoolValue;
			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Only enable the ability to change camera if the device has more than one camera.
				CameraButton.Enabled = isSessionRunning && NumberOfVideoCameras () > 1;
				RecordButton.Enabled = isSessionRunning;
				StillButton.Enabled = isSessionRunning;
			});
		}

		void SubjectAreaDidChange (NSNotification notification)
		{
			var devicePoint = new CGPoint (0.5, 0.5);
			UpdateDeviceFocus (AVCaptureFocusMode.ContinuousAutoFocus, AVCaptureExposureMode.ContinuousAutoExposure, devicePoint, false);
		}

		void SessionRuntimeError (NSNotification notification)
		{
			var error = (NSError)notification.UserInfo [AVCaptureSession.ErrorKey];
			Console.WriteLine ("Capture session runtime error: {0}", error);

			// Automatically try to restart the session running if media services were reset and the last start running succeeded.
			// Otherwise, enable the user to try to resume the session running.
			if (error.Code == (int)AVError.MediaServicesWereReset) {
				sessionQueue.DispatchAsync (() => {
					if (sessionRunning) {
						session.StartRunning ();
						sessionRunning = session.Running;
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
			// In some scenarios we want to enable the user to resume the session running.
			// For example, if music playback is initiated via control center while using AVCam,
			// then the user can let AVCam resume the session running, which will stop music playback.
			// Note that stopping music playback in control center will not automatically resume the session running.
			// Also note that it is not always possible to resume, see ResumeInterruptedSession.
			bool showResumeButton = false;

			// In iOS 9 and later, the userInfo dictionary contains information on why the session was interrupted.
			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				var reason = (AVCaptureSessionInterruptionReason)((NSNumber)notification.UserInfo [AVCaptureSession.InterruptionReasonKey]).Int32Value;
				Console.WriteLine ("Capture session was interrupted with reason {0}", reason);
				if (reason == AVCaptureSessionInterruptionReason.AudioDeviceInUseByAnotherClient ||
					reason == AVCaptureSessionInterruptionReason.VideoDeviceInUseByAnotherClient) {
					showResumeButton = true;
				} else if (reason == AVCaptureSessionInterruptionReason.VideoDeviceNotAvailableWithMultipleForegroundApps) {
					// Simply fade-in a label to inform the user that the camera is unavailable.
					CameraUnavailableLabel.Hidden = false;
					CameraUnavailableLabel.Alpha = 0;
					UIView.Animate (0.25, () => {
						CameraUnavailableLabel.Alpha = 1;
					});
				}
			} else {
				Console.WriteLine ("Capture session was interrupted");
				showResumeButton = UIApplication.SharedApplication.ApplicationState == UIApplicationState.Inactive;
			}
			if (showResumeButton) {
				// Simply fade-in a button to enable the user to try to resume the session running.
				ResumeButton.Hidden = false;
				ResumeButton.Alpha = 0;
				UIView.Animate (0.25, () => {
					ResumeButton.Alpha = 1;
				});
			}
		}

		void SessionInterruptionEnded (NSNotification notification)
		{
			Console.WriteLine ("Capture session interruption ended");
			if (!ResumeButton.Hidden) {
				UIView.AnimateNotify (0.25,
					() => ResumeButton.Alpha = 0,
					success => ResumeButton.Hidden = true);
			}
			if (!CameraUnavailableLabel.Hidden) {
				UIView.AnimateNotify (0.25,
					() => CameraUnavailableLabel.Alpha = 0,
					success => CameraUnavailableLabel.Hidden = true);
			}
		}

		#region Actions

		[Export ("toggleMovieRecording:")]
		void ToggleMovieRecording (CameraViewController sender)
		{
			// Disable the Camera button until recording finishes, and disable the Record button until recording starts or finishes.
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;

			sessionQueue.DispatchAsync (() => {
				if (!MovieFileOutput.Recording) {
					if (UIDevice.CurrentDevice.IsMultitaskingSupported) {
						// Setup background task. This is needed because the IAVCaptureFileOutputRecordingDelegate.FinishedRecording
						// callback is not received until AVCam returns to the foreground unless you request background execution time.
						// This also ensures that there will be time to write the file to the photo library when AVCam is backgrounded.
						// To conclude this background execution, UIApplication.SharedApplication.EndBackgroundTask is called in
						// IAVCaptureFileOutputRecordingDelegate.FinishedRecording after the recorded file has been saved.
						backgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask (null);
					}

					// Update the orientation on the movie file output video connection before starting recording.
					AVCaptureConnection connection = MovieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
					var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
					connection.VideoOrientation = previewLayer.Connection.VideoOrientation;

					// Turn OFF flash for video recording.
					SetFlashModeForDevice (AVCaptureFlashMode.Off, videoDeviceInput.Device);

					// Start recording to a temporary file.
					MovieFileOutput.StartRecordingToOutputFile (new NSUrl(GetTmpFilePath ("mov"), false), this);
				} else {
					MovieFileOutput.StopRecording ();
				}
			});
		}


		static string GetTmpFilePath (string extension)
		{
			// Start recording to a temporary file.
			string outputFileName = NSProcessInfo.ProcessInfo.GloballyUniqueString;
			string tmpDir = Path.GetTempPath ();
			string outputFilePath = Path.Combine (tmpDir, outputFileName);
			return Path.ChangeExtension (outputFilePath, extension);
		}

		static int NumberOfVideoCameras ()
		{
			return AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video).Length;
		}

		#endregion

		#region IAVCaptureFileOutputRecordingDelegate

		public void FinishedRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			// Note that currentBackgroundRecordingID is used to end the background task associated with this recording.
			// This allows a new recording to be started, associated with a new UIBackgroundTaskIdentifier, once the movie file output's isRecording property
			// is back to NO — which happens sometime after this method returns.
			// Note: Since we use a unique file path for each recording, a new recording will not overwrite a recording currently being saved.

			var currentBackgroundRecordingID = backgroundRecordingID;
			backgroundRecordingID = -1;
			Action cleanup = () => {
				NSError err;
				NSFileManager.DefaultManager.Remove (outputFileUrl, out err);
				if (currentBackgroundRecordingID != -1)
					UIApplication.SharedApplication.EndBackgroundTask (currentBackgroundRecordingID);
			};

			bool success = true;
			if (error != null) {
				Console.WriteLine ("Movie file finishing error: {0}", error);
				success = ((NSNumber)error.UserInfo [AVErrorKeys.RecordingSuccessfullyFinished]).BoolValue;
			}

			if (!success) {
				cleanup ();
				return;
			}
			// Check authorization status.
			PHPhotoLibrary.RequestAuthorization (status => {
				if (status == PHAuthorizationStatus.Authorized) {
					// Save the movie file to the photo library and cleanup.
					PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
						// In iOS 9 and later, it's possible to move the file into the photo library without duplicating the file data.
						// This avoids using double the disk space during save, which can make a difference on devices with limited free disk space.
						if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
							var options = new PHAssetResourceCreationOptions {
								ShouldMoveFile = true
							};
							var changeRequest = PHAssetCreationRequest.CreationRequestForAsset ();
							changeRequest.AddResource (PHAssetResourceType.Video, outputFileUrl, options);
						} else {
							PHAssetChangeRequest.FromVideo (outputFileUrl);
						}
					}, (success2, error2) => {
						if (!success2)
							Console.WriteLine ("Could not save movie to photo library: {0}", error2);
						cleanup ();
					});
				} else {
					cleanup ();
				}
			});
		}

		#endregion

		#region Device Configuration


		static void SetFlashModeForDevice (AVCaptureFlashMode flashMode, AVCaptureDevice device)
		{
			if (device.HasFlash && device.IsFlashModeSupported (flashMode)) {
				NSError error;
				if (device.LockForConfiguration (out error)) {
					device.FlashMode = flashMode;
					device.UnlockForConfiguration ();
				} else {
					Console.WriteLine ("Could not lock device for configuration: {0}", error);
				}
			}
		}

		static AVCaptureDevice CreateDevice (string mediaType, AVCaptureDevicePosition position)
		{
			AVCaptureDevice[] devices = AVCaptureDevice.DevicesWithMediaType (mediaType);
			AVCaptureDevice captureDevice = devices [0];
			foreach (var device in devices) {
				if (device.Position == position) {
					captureDevice = device;
					break;
				}
			}
			return captureDevice;
		}

		#endregion

		static bool TryConvertToVideoOrientation (UIDeviceOrientation orientation, out AVCaptureVideoOrientation result)
		{
			switch (orientation) {
			case UIDeviceOrientation.Portrait:
				result = AVCaptureVideoOrientation.Portrait;
				return true;

			case UIDeviceOrientation.PortraitUpsideDown:
				result = AVCaptureVideoOrientation.PortraitUpsideDown;
				return true;

			case UIDeviceOrientation.LandscapeLeft:
				result = AVCaptureVideoOrientation.LandscapeRight;
				return true;

			case UIDeviceOrientation.LandscapeRight:
				result = AVCaptureVideoOrientation.LandscapeLeft;
				return true;

			default:
				result = 0;
				return false;
			}
		}

		static bool TryConvertToVideoOrientation (UIInterfaceOrientation orientation, out AVCaptureVideoOrientation result)
		{
			switch (orientation) {
			case UIInterfaceOrientation.Portrait:
				result = AVCaptureVideoOrientation.Portrait;
				return true;

			case UIInterfaceOrientation.PortraitUpsideDown:
				result = AVCaptureVideoOrientation.PortraitUpsideDown;
				return true;

			case UIInterfaceOrientation.LandscapeLeft:
				result = AVCaptureVideoOrientation.LandscapeRight;
				return true;

			case UIInterfaceOrientation.LandscapeRight:
				result = AVCaptureVideoOrientation.LandscapeLeft;
				return true;

			default:
				result = 0;
				return false;
			}
		}
	}
}