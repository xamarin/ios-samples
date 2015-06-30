using System;
using UIKit;
using Foundation;
using CoreGraphics;
using AVFoundation;
using CoreFoundation;
using System.Collections.Generic;
using System.IO;
using Photos;

namespace AVCam
{
	public enum AVCamSetupResult
	{
		Success,
		CameraNotAuthorized,
		SessionConfigurationFailed
	}

	[Register ("AAPLCameraViewController")]
	public class CameraViewController : UIViewController, IAVCaptureFileOutputRecordingDelegate
	{
		[Outlet]
		PreviewView PreviewView { get; set; }

		[Outlet]
		UILabel CameraUnavailableLabel  { get; set; }

		[Outlet]
		UIButton ResumeButton { get; set; }

		[Outlet]
		UIButton RecordButton { get; set; }

		[Outlet]
		UIButton CameraButton { get; set; }

		[Outlet]
		UIButton StillButton { get; set; }

		DispatchQueue SessionQueue { get; set; }
		AVCaptureSession Session  { get; set; }
		AVCaptureDeviceInput VideoDeviceInput  { get; set; }
		AVCaptureMovieFileOutput MovieFileOutput { get; set; }
		AVCaptureStillImageOutput StillImageOutput { get; set; }

		AVCamSetupResult SetupResult { get; set; }
		bool SessionRunning { get; set; }
		nint BackgroundRecordingID { get; set; }

		IDisposable subjectSubscriber;
		IDisposable runningObserver;
		IDisposable capturingStillObserver;
		IDisposable recordingObserver;
		IDisposable runtimeErrorObserver;
		IDisposable interuptionObserver;
		IDisposable interuptionEndedObserver;

		public CameraViewController(IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Disable UI. The UI is enabled if and only if the session starts running.
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			StillButton.Enabled = false;

			// Create the AVCaptureSession.
			Session = new AVCaptureSession ();

			// Setup the preview view.
			PreviewView.Session = Session;

			// Communicate with the session and other session objects on this queue.
			SessionQueue = new DispatchQueue ("session queue");
			SetupResult = AVCamSetupResult.Success;

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
				SessionQueue.Suspend ();
				AVCaptureDevice.RequestAccessForMediaType (AVMediaType.Video, granted => {
					if (!granted)
						SetupResult = AVCamSetupResult.CameraNotAuthorized;
					SessionQueue.Resume ();
				});
				break;

			// The user has previously denied access.
			default:
				SetupResult = AVCamSetupResult.CameraNotAuthorized;
				break;
			}

			// Setup the capture session.
			// In general it is not safe to mutate an AVCaptureSession or any of its inputs, outputs, or connections from multiple threads at the same time.
			// Why not do all of this on the main queue?
			// Because AVCaptureSession.StartRunning is a blocking call which can take a long time. We dispatch session setup to the sessionQueue
			// so that the main queue isn't blocked, which keeps the UI responsive.
			SessionQueue.DispatchAsync (() => {
				if (SetupResult != AVCamSetupResult.Success)
					return;

				BackgroundRecordingID = -1;
				NSError error = null;
				AVCaptureDevice videoDevice = CameraViewController.CreateDevice (AVMediaType.Video, AVCaptureDevicePosition.Back);
				AVCaptureDeviceInput videoDeviceInput = AVCaptureDeviceInput.FromDevice (videoDevice, out error);
				if (videoDeviceInput == null)
					Console.WriteLine ("Could not create video device input: {0}", error);

				Session.BeginConfiguration ();
				if (Session.CanAddInput (videoDeviceInput)) {
					Session.AddInput (videoDeviceInput);
					VideoDeviceInput = videoDeviceInput;
					DispatchQueue.MainQueue.DispatchAsync (() => {
						// Why are we dispatching this to the main queue?
						// Because AVCaptureVideoPreviewLayer is the backing layer for PreviewView and UIView
						// can only be manipulated on the main thread.
						// Note: As an exception to the above rule, it is not necessary to serialize video orientation changes
						// on the AVCaptureVideoPreviewLayer’s connection with other session manipulation.
						// Use the status bar orientation as the initial video orientation. Subsequent orientation changes are handled by
						// ViewWillTransitionToSize method.
						UIInterfaceOrientation statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
						AVCaptureVideoOrientation initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
						if (statusBarOrientation != UIInterfaceOrientation.Unknown)
							initialVideoOrientation = (AVCaptureVideoOrientation)(long)statusBarOrientation;

						var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
						previewLayer.Connection.VideoOrientation = initialVideoOrientation;
					});
				} else {
					Console.WriteLine ("Could not add video device input to the session");
					SetupResult = AVCamSetupResult.SessionConfigurationFailed;
				}

				AVCaptureDevice audioDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Audio);
				AVCaptureDeviceInput audioDeviceInput = AVCaptureDeviceInput.FromDevice (audioDevice, out error);
				if (audioDeviceInput == null)
					Console.WriteLine ("Could not create audio device input: {0}", error);

				if (Session.CanAddInput (audioDeviceInput))
					Session.AddInput (audioDeviceInput);
				else
					Console.WriteLine ("Could not add audio device input to the session");

				var movieFileOutput = new AVCaptureMovieFileOutput ();
				if (Session.CanAddOutput (movieFileOutput)) {
					Session.AddOutput (movieFileOutput);
					AVCaptureConnection connection = movieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
					if (connection.SupportsVideoStabilization)
						connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto; // TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=31518
					MovieFileOutput = movieFileOutput;
				} else {
					Console.WriteLine ("Could not add movie file output to the session");
					SetupResult = AVCamSetupResult.SessionConfigurationFailed;
				}

				var stillImageOutput = new AVCaptureStillImageOutput ();
				if (Session.CanAddOutput (stillImageOutput)) {
					stillImageOutput.CompressedVideoSetting = new AVVideoSettingsCompressed {
						Codec = AVVideoCodec.JPEG
					};
					Session.AddOutput (stillImageOutput);
					StillImageOutput = stillImageOutput;
				} else {
					Console.WriteLine ("Could not add still image output to the session");
					SetupResult = AVCamSetupResult.SessionConfigurationFailed;
				}

				Session.CommitConfiguration ();
			});
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SessionQueue.DispatchAsync (() => {
				switch (SetupResult) {
				// Only setup observers and start the session running if setup succeeded.
				case AVCamSetupResult.Success:
					AddObservers ();
					Session.StartRunning ();
					SessionRunning = Session.Running;
					break;

				case AVCamSetupResult.CameraNotAuthorized:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "AVCam doesn't have permission to use the camera, please change privacy settings";
						UIAlertController alertController = UIAlertController.Create ("AVCam", message, UIAlertControllerStyle.Alert);
						UIAlertAction cancelAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
						alertController.AddAction (cancelAction);
						// Provide quick access to Settings.
						UIAlertAction settingsAction = UIAlertAction.Create ("Settings", UIAlertActionStyle.Default, action => {
							UIApplication.SharedApplication.OpenUrl (new NSUrl (UIApplication.OpenSettingsUrlString));
						});
						alertController.AddAction (settingsAction);
						PresentViewController (alertController, true, null);
					});
					break;

				case AVCamSetupResult.SessionConfigurationFailed:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "Unable to capture media";
						UIAlertController alertController = UIAlertController.Create ("AVCam", message, UIAlertControllerStyle.Alert);
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
			SessionQueue.DispatchAsync (() => {
				if (SetupResult == AVCamSetupResult.Success) {
					Session.StopRunning ();
					RemoveObservers ();
				}
			});
			base.ViewDidDisappear (animated);
		}

		public override bool ShouldAutorotate ()
		{
			if (MovieFileOutput == null)
				return true;

			return !MovieFileOutput.Recording;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		public override void ViewWillTransitionToSize (CGSize size, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (size, coordinator);
			// Note that the app delegate controls the device orientation notifications required to use the device orientation.
			UIDeviceOrientation deviceOrientation = UIDevice.CurrentDevice.Orientation;
			if (deviceOrientation.IsPortrait () || deviceOrientation.IsLandscape ()) {
				var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
				previewLayer.Connection.VideoOrientation = (AVCaptureVideoOrientation)(long)deviceOrientation;
			}
		}

		void AddObservers ()
		{
			runningObserver = Session.AddObserver ("running", NSKeyValueObservingOptions.New, OnSessionRunningChanged);
			capturingStillObserver = StillImageOutput.AddObserver ("capturingStillImage", NSKeyValueObservingOptions.New, OnCapturingStillImageChanged);
			recordingObserver = MovieFileOutput.AddObserver ("recording", NSKeyValueObservingOptions.New, OnRecordingChanged);

			subjectSubscriber = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, VideoDeviceInput.Device);
			runtimeErrorObserver = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.RuntimeErrorNotification, SessionRuntimeError, Session);

			// A session can only run when the app is full screen. It will be interrupted in a multi-app layout, introduced in iOS 9.
			// Add observers to handle these session interruptions
			// and show a preview is paused message. See the documentation of AVCaptureSession.WasInterruptedNotification for other
			// interruption reasons.
			interuptionObserver = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.WasInterruptedNotification, SessionWasInterrupted, Session);
			interuptionEndedObserver = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.InterruptionEndedNotification, SessionInterruptionEnded, Session);
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
					CameraButton.Enabled = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video).Length > 1;
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
				CameraButton.Enabled = isSessionRunning && AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video).Length > 1;
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
				SessionQueue.DispatchAsync (() => {
					if (SessionRunning) {
						Session.StartRunning ();
						SessionRunning = Session.Running;
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
			// Also note that it is not always possible to resume, see -[resumeInterruptedSession:].
			bool showResumeButton = false;

			// In iOS 9 and later, the userInfo dictionary contains information on why the session was interrupted.
			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
//					AVCaptureSessionInterruptionReason reason = [notification.userInfo[AVCaptureSessionInterruptionReasonKey] integerValue];
//				Console.WriteLine ("Capture session was interrupted with reason {0}", (long)reason );
//					if ( reason == AVCaptureSessionInterruptionReasonAudioDeviceInUseByAnotherClient ||
				//			 reason == AVCaptureSessionInterruptionReasonVideoDeviceInUseByAnotherClient ) {
				//			showResumeButton = YES;
				//		}
				//		else if ( reason == AVCaptureSessionInterruptionReasonVideoDeviceNotAvailableWithMultipleForegroundApps ) {
				// Simply fade-in a label to inform the user that the camera is unavailable.
				CameraUnavailableLabel.Hidden = false;
				CameraUnavailableLabel.Alpha = 0;
				UIView.Animate (0.25, () => {
					CameraUnavailableLabel.Alpha = 1;
				});
//					}
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
				UIView.Animate (0.25, () => {
					ResumeButton.Alpha = 0;
				}, () => {
					ResumeButton.Hidden = true;
				});
			}
			if (!CameraUnavailableLabel.Hidden) {
				UIView.Animate (0.25, () => {
					CameraUnavailableLabel.Alpha = 0;
				}, () => {
					CameraUnavailableLabel.Hidden = true;
				});
			}
		}

		#region Actions

		[Export("resumeInterruptedSession:")]
		void ResumeInterruptedSession (CameraViewController sender)
		{
			SessionQueue.DispatchAsync (() => {
				// The session might fail to start running, e.g., if a phone or FaceTime call is still using audio or video.
				// A failure to start the session running will be communicated via a session runtime error notification.
				// To avoid repeatedly failing to start the session running, we only try to restart the session running in the
				// session runtime error handler if we aren't trying to resume the session running.

				Session.StartRunning ();
				SessionRunning = Session.Running;
				if (!Session.Running) {
					DispatchQueue.MainQueue.DispatchAsync (() => {
						string message = "Unable to resume";
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

		[Export("toggleMovieRecording:")]
		void ToggleMovieRecording (CameraViewController sender)
		{
			RecordButton.Enabled = false;
			SessionQueue.DispatchAsync (() => {
				if (!MovieFileOutput.Recording) {
					if (UIDevice.CurrentDevice.IsMultitaskingSupported) {
						// TODO: fix comments
						// Setup background task. This is needed because the -[captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:]
						// callback is not received until AVCam returns to the foreground unless you request background execution time.
						// This also ensures that there will be time to write the file to the photo library when AVCam is backgrounded.
						// To conclude this background execution, -endBackgroundTask is called in
						// -[captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:] after the recorded file has been saved.
						BackgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask (null);
					}
					// Update the orientation on the movie file output video connection before starting recording.
					AVCaptureConnection connection = MovieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
					AVCaptureVideoPreviewLayer previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
					connection.VideoOrientation = previewLayer.Connection.VideoOrientation;
					// Turn OFF flash for video recording.
					SetFlashModeForDevice (AVCaptureFlashMode.Off, VideoDeviceInput.Device);
					// Start recording to a temporary file.
					string outputFileName = NSProcessInfo.ProcessInfo.GloballyUniqueString;
					string tmpDir = Path.GetTempPath ();
					string outputFilePath = Path.Combine (tmpDir, outputFileName);
					outputFileName = Path.ChangeExtension (outputFileName, "mov");
					MovieFileOutput.StartRecordingToOutputFile (new NSUrl (outputFilePath), this);
				} else {
					MovieFileOutput.StopRecording ();
				}
			});
		}

		[Export("changeCamera:")]
		void ChangeCamera (CameraViewController sender)
		{
			CameraButton.Enabled = false;
			RecordButton.Enabled = false;
			StillButton.Enabled = false;
			SessionQueue.DispatchAsync (() => {
				AVCaptureDevice currentVideoDevice = VideoDeviceInput.Device;
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
				Session.BeginConfiguration ();

				// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
				Session.RemoveInput (videoDeviceInput);
				if (Session.CanAddInput (videoDeviceInput)) {
					subjectSubscriber.Dispose ();
					SetFlashModeForDevice (AVCaptureFlashMode.Auto, videoDevice);
					subjectSubscriber = NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, videoDevice);
					Session.AddInput (videoDeviceInput);
					VideoDeviceInput = videoDeviceInput;
				} else {
					Session.AddInput (VideoDeviceInput);
				}
				AVCaptureConnection connection = MovieFileOutput.ConnectionFromMediaType (AVMediaType.Video);
				if (connection.SupportsVideoStabilization) {
					connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
				}
				Session.CommitConfiguration ();
				DispatchQueue.MainQueue.DispatchAsync (() => {
					CameraButton.Enabled = true;
					RecordButton.Enabled = true;
					StillButton.Enabled = true;
				});
			});
		}

		[Export("snapStillImage:")]
		void SnapStillImage (CameraViewController sender)
		{
			SessionQueue.DispatchAsync (() => {
				AVCaptureConnection connection = StillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
				var previewLayer = (AVCaptureVideoPreviewLayer)PreviewView.Layer;
				// Update the orientation on the still image output video connection before capturing.
				connection.VideoOrientation = previewLayer.Connection.VideoOrientation;
				// Flash set to Auto for Still Capture.
				SetFlashModeForDevice (AVCaptureFlashMode.Auto, VideoDeviceInput.Device);
				// Capture a still image.
				StillImageOutput.CaptureStillImageAsynchronously (connection, (imageDataSampleBuffer, error) => {
					if (imageDataSampleBuffer != null) {
						// The sample buffer is not retained. Create image data before saving the still image to the photo library asynchronously.
						NSData imageData = AVCaptureStillImageOutput.JpegStillToNSData (imageDataSampleBuffer);
						PHPhotoLibrary.RequestAuthorization (status => {
							if (status == PHAuthorizationStatus.Authorized) {
								PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
									// TODO: not bounded
									// [[PHAssetCreationRequest creationRequestForAsset] addResourceWithType:PHAssetResourceTypePhoto data:imageData options:nil];
								}, (success, err) => {
									if (!success)
										Console.WriteLine ("Error occurred while saving image to photo library: {0}", err);
								});
							}
						});
					} else {
						Console.WriteLine ("Could not capture still image: {0}", error);
					}
				});
			});
		}

		[Export("focusAndExposeTap:")]
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
			var currentBackgroundRecordingID = BackgroundRecordingID;
			BackgroundRecordingID = -1;
			Action cleanup = () => {
				NSError err;
				NSFileManager.DefaultManager.Remove (outputFileUrl, out err);
				if (currentBackgroundRecordingID != -1)
					UIApplication.SharedApplication.EndBackgroundTask (currentBackgroundRecordingID);
			};

			bool success = true;
			if (error != null) {
				Console.WriteLine ("Movie file finishing error: {0}", error);
				// TODO: not bound
//				success = error.UserInfo[AVErrorRecordingSuccessfullyFinishedKey] boolValue];
			}
			if (success) {
				// Check authorization status.
				PHPhotoLibrary.RequestAuthorization (status => {
					if (status == PHAuthorizationStatus.Authorized) {
						// Save the movie file to the photo library and cleanup.
						PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
							// In iOS 9 and later, it's possible to move the file into the photo library without duplicating the file data.
							// This avoids using double the disk space during save, which can make a difference on devices with limited free disk space.
							if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
								throw new NotImplementedException ();
//								PHAssetResourceCreationOptions *options = [[PHAssetResourceCreationOptions alloc] init];
//								options.shouldMoveFile = YES;
//								PHAssetCreationRequest *changeRequest = [PHAssetCreationRequest creationRequestForAsset];
//								[changeRequest addResourceWithType:PHAssetResourceTypeVideo fileURL:outputFileURL options:options];
							} else {
								PHAssetChangeRequest.FromVideo (outputFileUrl);
							}
						}, (success2, error2) => {
							if (!success2) {
								Console.WriteLine ("Could not save movie to photo library: {0}", error);
							}
							cleanup ();
						});
					} else {
						cleanup ();
					}
				});
			} else {
				cleanup ();
			}
		}

		#endregion

		#region Device Configuration

		void UpdateDeviceFocus (AVCaptureFocusMode focusMode, AVCaptureExposureMode exposureMode, CGPoint point, bool monitorSubjectAreaChange)
		{
			SessionQueue.DispatchAsync (() => {
				AVCaptureDevice device = VideoDeviceInput.Device;
				NSError error = null;
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
				NSError error = null;
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
	}
}