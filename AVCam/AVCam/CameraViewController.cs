using System;
using System.IO;

using UIKit;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using AVFoundation;
using Photos;

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

	[Register ("CameraViewController")]
	public class CameraViewController : UIViewController, IAVCaptureFileOutputRecordingDelegate
	{
		// TODO: ???
		[Outlet]
		PreviewView PreviewView { get; set; }

		// TODO: ???
		[Outlet]
		UILabel CameraUnavailableLabel  { get; set; }

		// TODO: ???
		[Outlet]
		UIButton ResumeButton { get; set; }

		// TODO: ???
		[Outlet]
		UIButton RecordButton { get; set; }

		// TODO: ???
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

		// Communicate with the session and other session objects on this queue.
		readonly DispatchQueue sessionQueue = new DispatchQueue ("session queue");

		readonly AVCaptureSession session = new AVCaptureSession ();

		AVCaptureDeviceInput videoDeviceInput;
		readonly AVCapturePhotoOutput photoOutput = new AVCapturePhotoOutput ();

		// TODO: ???
		AVCaptureMovieFileOutput MovieFileOutput { get; set; }

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


		#endregion

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

		[Export ("changeCamera:")]
		void ChangeCamera (CameraViewController sender)
		{
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			StillButton.Enabled = false;

			sessionQueue.DispatchAsync (() => {
				AVCaptureDevice currentVideoDevice = videoDeviceInput.Device;
				AVCaptureDevicePosition preferredPosition = AVCaptureDevicePosition.Unspecified;
				AVCaptureDevicePosition currentPosition = currentVideoDevice.Position;

				switch (currentPosition) {
					case AVCaptureDevicePosition.Unspecified:
					case AVCaptureDevicePosition.Front:
						preferredPosition = AVCaptureDevicePosition.Back;
						break;
					case AVCaptureDevicePosition.Back:
						preferredPosition = AVCaptureDevicePosition.Front;
						break;
				}
				AVCaptureDevice videoDevice = CreateDevice (AVMediaType.Video, preferredPosition);
				AVCaptureDeviceInput videoDeviceInput = AVCaptureDeviceInput.FromDevice (videoDevice);

				session.BeginConfiguration ();

				// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
				session.RemoveInput (videoDeviceInput);

				if (session.CanAddInput (videoDeviceInput)) {
					subjectSubscriber.Dispose ();

					SetFlashModeForDevice (AVCaptureFlashMode.Auto, videoDevice);
					subjectSubscriber = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, videoDevice);

					session.AddInput (videoDeviceInput);
					videoDeviceInput = videoDeviceInput;
				} else {
					session.AddInput (videoDeviceInput);
				}

				AVCaptureConnection connection = MovieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
				if (connection.SupportsVideoStabilization)
					connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;

				session.CommitConfiguration ();

				DispatchQueue.MainQueue.DispatchAsync (() => {
					CameraButton.Enabled = true;
					RecordButton.Enabled = true;
					StillButton.Enabled = true;
				});
			});
		}

		[Export ("snapStillImage:")]
		void SnapStillImage (CameraViewController sender)
		{
			sessionQueue.DispatchAsync (async () => {
				AVCaptureConnection connection = StillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
				var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;

				// Update the orientation on the still image output video connection before capturing.
				connection.VideoOrientation = previewLayer.Connection.VideoOrientation;

				// Flash set to Auto for Still Capture.
				SetFlashModeForDevice (AVCaptureFlashMode.Auto, videoDeviceInput.Device);

				// Capture a still image.
				try {
					var imageDataSampleBuffer = await StillImageOutput.CaptureStillImageTaskAsync (connection);

					// The sample buffer is not retained. Create image data before saving the still image to the photo library asynchronously.
					NSData imageData = AVCaptureStillImageOutput.JpegStillToNSData (imageDataSampleBuffer);

					PHPhotoLibrary.RequestAuthorization (status => {
						if (status == PHAuthorizationStatus.Authorized) {
							// To preserve the metadata, we create an asset from the JPEG NSData representation.
							// Note that creating an asset from a UIImage discards the metadata.

							// In iOS 9, we can use AddResource method on PHAssetCreationRequest class.
							// In iOS 8, we save the image to a temporary file and use +[PHAssetChangeRequest creationRequestForAssetFromImageAtFileURL:].

							if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
								PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
									var request = PHAssetCreationRequest.CreationRequestForAsset ();
									request.AddResource (PHAssetResourceType.Photo, imageData, null);
								}, (success, err) => {
									if (!success)
										Console.WriteLine ("Error occurred while saving image to photo library: {0}", err);
								});
							} else {
								var temporaryFileUrl = new NSUrl (GetTmpFilePath ("jpg"), false);
								PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
									NSError error = null;
									if (imageData.Save (temporaryFileUrl, NSDataWritingOptions.Atomic, out error))
										PHAssetChangeRequest.FromImage (temporaryFileUrl);
									else
										Console.WriteLine ("Error occured while writing image data to a temporary file: {0}", error);
								}, (success, error) => {
									if (!success)
										Console.WriteLine ("Error occurred while saving image to photo library: {0}", error);

									// Delete the temporary file.
									NSError deleteError;
									NSFileManager.DefaultManager.Remove (temporaryFileUrl, out deleteError);
								});
							}
						}
					});
				} catch (NSErrorException ex) {
					Console.WriteLine ("Could not capture still image: {0}", ex.Error);
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

		[Export ("focusAndExposeTap:")]
		void FocusAndExposeTap (UIGestureRecognizer gestureRecognizer)
		{
			var location = gestureRecognizer.LocationInView (gestureRecognizer.View);
			CGPoint devicePoint = ((AVCaptureVideoPreviewLayer)PreviewView.Layer).CaptureDevicePointOfInterestForPoint (location);
			UpdateDeviceFocus (AVCaptureFocusMode.AutoFocus, AVCaptureExposureMode.AutoExpose, devicePoint, true);
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

		void UpdateDeviceFocus (AVCaptureFocusMode focusMode, AVCaptureExposureMode exposureMode, CGPoint point, bool monitorSubjectAreaChange)
		{
			sessionQueue.DispatchAsync (() => {
				if (videoDeviceInput == null)
					return;

				AVCaptureDevice device = videoDeviceInput.Device;
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
					Console.WriteLine ("Could not lock device for configuration: {0}", error);
				}
			});
		}

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