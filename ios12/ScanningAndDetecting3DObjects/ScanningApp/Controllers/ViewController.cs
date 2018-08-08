using System;
using Foundation;
using UIKit;
using ARKit;
using CoreGraphics;
using System.IO;
using HealthKit;
using CoreFoundation;
using OpenTK;

namespace ScanningAndDetecting3DObjects
{
	internal partial class ViewController : UIViewController, IUIGestureRecognizerDelegate
	{

		private ViewControllerApplicationState state;
		internal ViewControllerApplicationState State { get => state; }

		ViewControllerSessionInfo sessionInfo;

		private Scan scan;
		internal Scan CurrentScan { get => scan; }

		private TestRun testRun;

		internal void ShowSessionInfo(string message, bool visible)
		{
			sessionInfoLabel.Text = message;
			sessionInfoView.Hidden = !visible;
		}

		internal TestRun ActiveTestRun { get => testRun; }

		internal ARSCNView SceneView { get => sceneView;  }

		NSTimer limitedTrackingTimer;
		NSTimer maxScanTimeTimer;

		internal CGPoint ScreenCenter { get; private set;  }

		// ViewController helper instances
		ViewControllerNavigationBar navigationBarController;

		internal static ViewController Instance { get; private set; }

		private NSUrl modelUrl;

		internal NSUrl ModelUrl
		{
			get => modelUrl;

			set
			{
				if (value != null)
				{
					modelUrl = value;
					sessionInfo.DisplayMessage($"3D Model \"{modelUrl.LastPathComponent}\" received.", 3.0);
				}
				scan?.ScannedObject?.Set3DModel(modelUrl);
				testRun?.DetectedObject?.Set3DModel(modelUrl);
			}
		}

		// Note that it's critical to Dispose() ASAP of any managed references to the session frame.
		// This is a common cause of "frame freeze" in AR apps. (Symptom: app seems to run, but image
		// is frozen. This is because the iOS system sees there is still a reference to the current frame
		// and declines to provide a new frame because it figures "current frame is still being used"! (see README.MD)
		internal ARFrame SessionFrame()
		{
			return sceneView?.Session?.CurrentFrame;
		}


		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		internal void EnterStateStartARSession()
		{
			scan?.Dispose();
			scan = null;
			testRun?.Dispose();
			testRun = null;
			modelUrl?.Dispose();
			modelUrl = null;
			navigationBarController.SetNavigationBarTitle("");
			navigationBarController.ShowBackButton(false);
			instructionView.Hidden = true;
			nextButton.Enabled = false;
			loadModelButton.Hidden = true;
			flashlightButton.Hidden = true;

			// Make sure the SCNScene is cleared of any SCNNodes from previous scans.
			sceneView.Scene = new SceneKit.SCNScene();

			var configuration = new ARObjectScanningConfiguration();
			configuration.PlaneDetection = ARPlaneDetection.Horizontal;
			sceneView.Session.Run(configuration, ARSessionRunOptions.ResetTracking);
			CancelMaxScanTimeTimer();
			sessionInfo.CancelMessageExpirationTimer();

		}

		internal void EnterStateTesting()
		{
			navigationBarController.SetNavigationBarTitle("Testing");
			navigationBarController.ShowBackButton(false);
			loadModelButton.Hidden = true;
			flashlightButton.Hidden = false;
			nextButton.Enabled = true;
			nextButton.SetTitle("Share", UIControlState.Normal);

			testRun = new TestRun(sessionInfo, sceneView);
			TestObjectDetection();
			CancelMaxScanTimeTimer();
		}

		internal void EnterStateScanning()
		{
			if (scan == null)
			{
				scan = new Scan(sceneView);
				scan.State = Scan.ScanState.Ready;
			}
			testRun?.Dispose();
			testRun = null;
			StartMaxScanTimeTimer();
		}

		internal void EnterStateNotReady()
		{
			scan?.Dispose();
			scan = null;
			testRun?.Dispose();
			testRun = null;
			navigationBarController.SetNavigationBarTitle("");
			navigationBarController.ShowBackButton(false);
			loadModelButton.Hidden = true;
			flashlightButton.Hidden = true;
			nextButton.Enabled = false;
			nextButton.SetTitle("Next", UIControlState.Normal);
			DisplayInstruction(new Message("Please wait for stable tracking"));
			CancelMaxScanTimeTimer();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Instance = this;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			sceneView.Session.Pause();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Member initialization order is important! Primirily because there is logic assoc'd w changing the state. (see EnterState* functions)
			navigationBarController = new ViewControllerNavigationBar(this.navigationBar, PreviousButtonTapped, RestartButtonTapped);
			sessionInfo = new ViewControllerSessionInfo(this);

			state = new ViewControllerApplicationState(this);


			sceneView.Delegate = new ViewControllerSCNViewDelegate(this, sessionInfo);
			sceneView.Session.Delegate = new ViewControllerSessionDelegate(this, State, sessionInfo);

			// Prevent the screen from being dimmed after a while
			UIApplication.SharedApplication.IdleTimerDisabled = true;


			// Configure notifications for application state changes
			var notificationCenter = NSNotificationCenter.DefaultCenter;

			notificationCenter.AddObserver(Scan.ScanningStateChangedNotificationName, State.ScanningStateChanged);
			notificationCenter.AddObserver(ScannedObject.GhostBoundingBoxCreatedNotificationName, State.GhostBoundingBoxWasCreated);
			notificationCenter.AddObserver(ScannedObject.GhostBoundingBoxRemovedNotificationName, State.GhostBoundingBoxWasRemoved);
			notificationCenter.AddObserver(ScannedObject.BoundingBoxCreatedNotificationName, State.BoundingBoxWasCreated);
			notificationCenter.AddObserver(BoundingBox.ScanPercentageChangedNotificationName, ScanPercentageChanged);
			notificationCenter.AddObserver(BoundingBox.ExtentChangedNotificationName, BoundingBoxExtentChanged);
			notificationCenter.AddObserver(BoundingBox.PositionChangedNotificationName, BoundingBoxPositionChanged);
			notificationCenter.AddObserver(ObjectOrigin.PositionChangedNotificationName, ObjectOriginPositionChanged);
			notificationCenter.AddObserver(NSProcessInfo.PowerStateDidChangeNotification, DisplayWarningIfInLowPowerMode);


			DisplayWarningIfInLowPowerMode(null);

			// Make sure the application launches in AppStat.StartARSession state
			// Entering this state launches the ARSession
			State.CurrentState = AppState.StartARSession;
		}

		internal void EnableNextButton(bool b)
		{
			nextButton.Enabled = b;
		}

		internal void EnterStateAdjustingOrigin()
		{
			DisplayInstruction(new Message("Adjust origin using gestures.\nYou can load a *.usdz 3D model overlay"));
			navigationBarController.SetNavigationBarTitle("Adjust origin");
			navigationBarController.ShowBackButton(true);
			nextButton.Enabled = true;
			loadModelButton.Hidden = false;
			flashlightButton.Hidden = true;
			nextButton.SetTitle("Test", UIControlState.Normal);
		}

		internal void EnterStateScanningContinue()
		{
			DisplayInstruction(new Message("Scan the object from all sides in which you are interested. Do not move the object while scanning!"));
			var boundingBox = scan.ScannedObject.BoundingBox;
			if (boundingBox != null)
			{
				navigationBarController.SetNavigationBarTitle($"Scan ({boundingBox.ProgressPercentage}%)");
			}
			else
			{
				navigationBarController.SetNavigationBarTitle("Scan 0%");
			}
			navigationBarController.ShowBackButton(true);
			nextButton.Enabled = true;
			loadModelButton.Hidden = true;
			flashlightButton.Hidden = true;
			nextButton.SetTitle("Finish", UIControlState.Normal);
		}

		internal void EnterStateDefineBoundingBox()
		{
			DisplayInstruction(new Message("Position and resize bounding box using gestures.\nLong press sides to push/pull them in or out."));
			navigationBarController.SetNavigationBarTitle("Define bounding box");
			navigationBarController.ShowBackButton(true);
			nextButton.Enabled = scan.BoundingBoxExists;
			loadModelButton.Hidden = true;
			flashlightButton.Hidden = true;
			nextButton.SetTitle("Scan", UIControlState.Normal);
		}

		internal void EnterStateScanReady()
		{
			navigationBarController.SetNavigationBarTitle("Ready to scan");
			navigationBarController.ShowBackButton(false);
			nextButton.SetTitle("Next", UIControlState.Normal);
			loadModelButton.Hidden = true;
			flashlightButton.Hidden = true;
			if (scan.GhostBoundingBoxExists)
			{
				DisplayInstruction(new Message("Tap 'Next' to create an approximate bounding box around the object you want to scan."));
				nextButton.Enabled = true;
			}
			else
			{
				DisplayInstruction(new Message("Point at a nearby object to scan."));
				nextButton.Enabled = false;
			}
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			// Store the screen center location after the view's bounds did change,
			// so it can be retrieved from outside the main thread
			ScreenCenter = sceneView.Center;
		}

		internal void RestartButtonTapped(Object sender, EventArgs args)
		{
			Action startOverAlert = () =>
			{
				var title = "Start over?";
				var message = "Discard the current scan and start over?";
				ShowAlert(title, message, "Yes", true, (o) => State.CurrentState = AppState.StartARSession);
			};

			if (scan != null && scan.BoundingBoxExists)
			{
				startOverAlert();
			}
			else
			{
				if (testRun != null)
				{
					startOverAlert();
				}
				else
				{
					State.CurrentState = AppState.StartARSession;
				}
			}
		}

		internal void PreviousButtonTapped(Object sender, EventArgs args)
		{
			State.SwitchToPreviousState();
		}


		partial void NextButtonTapped(RoundedButton sender)
		{
			if (nextButton.Hidden || !nextButton.Enabled)
			{
				return;
			}

			State.SwitchToNextState();
		}

		partial void LoadModelButtonTouched(NSObject sender)
		{
			if (loadModelButton.Hidden || !loadModelButton.Enabled)
			{
				return;
			}

			var documentPicker = new UIDocumentPickerViewController(new[] { "com.pixar.universal-scene-description-mobile" }, UIDocumentPickerMode.Import);
			documentPicker.Delegate = new ViewControllerDocumentPickerDelegate((url) => modelUrl = url);

			documentPicker.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
			if (documentPicker.PopoverPresentationController != null)
			{
				documentPicker.PopoverPresentationController.SourceView = loadModelButton;
				documentPicker.PopoverPresentationController.SourceRect = loadModelButton.Bounds;
			}
			DispatchQueue.MainQueue.DispatchAsync(() =>
		   {
			   PresentViewController(documentPicker, true, () => { });
		   });
		}

		partial void LeftButtonTouchAreaTapped(NSObject sender)
		{
			// A tap in the extended hit area on the lower left should cause a tap
			//  on the button that is currently visible at that location.
			if (!loadModelButton.Hidden)
			{
				LoadModelButtonTouched(this);
			}
			else
			{
				if (!flashlightButton.Hidden)
				{
					ToggleFlashlightButtonTapped(flashlightButton);
				}
			}
		}

		partial void ToggleFlashlightButtonTapped(FlashlightButton sender)
		{
			if (flashlightButton.Hidden || !flashlightButton.Enabled)
			{
				return;
			}
			flashlightButton.ToggledOn = !flashlightButton.ToggledOn;
		}

		partial void toggleInstructionsButtonTapped(RoundedButton sender)
		{
			if (toggleInstructionsButton.Hidden || !toggleInstructionsButton.Enabled)
			{
				return;
			}

			if (toggleInstructionsButton.ToggledOn)
			{
				HideInstructions();
			}
			else
			{
				ShowInstructions();
			}
		}

		internal void DisplayInstruction(Message msg)
		{
			instructionLabel.Display(msg);
			ShowInstructions();
		}

		void HideInstructions()
		{
			instructionView.Hidden = true;
			toggleInstructionsButton.ToggledOn = false;
		}

		void ShowInstructions()
		{
			instructionView.Hidden = false;
			toggleInstructionsButton.ToggledOn = true;
		}

		void TestObjectDetection()
		{
			if (scan == null || scan.BoundingBoxExists == false)
			{
				Console.WriteLine("Error: Bounding Box not yet created");
				return;
			}

			scan.CreateReferenceObject((scannedObject) =>
			{
				var localRef = scannedObject;
				if (localRef != null)
				{
					testRun?.SetReferenceObject(localRef, scan.Screenshot);

					// Delete the scan to make sure that users cannot go back from
					// testing to scanning, because:
					// 1. Testing and scanning require running the ARSession with different configurations,
					//    thus the scanned environment is lost when starting a test.
					// 2. We encourage users to move the scanned object during testing, which invalidates
					//    the feature point cloud which was captured during scanning.
					scan?.Dispose();
					scan = null;
					DisplayInstruction(new Message("Test detection of the object from different angles. Consider moving the object to different environments and test there."));
				}
				else
				{
					var title = "Scan failed";
					var message = "Saving the scan failed.";
					var buttonTitle = "Restart Scan";
					ShowAlert(title, message, buttonTitle, false, (_) => { State.CurrentState = AppState.StartARSession; });
				}
			});
		}

		internal void CreateAndShareReferenceObject()
		{
			if (testRun == null || testRun.ReferenceObject == null || testRun.ReferenceObject.Name == null)
			{
				Console.WriteLine("Error: Missing scanned object.");
				return;
			}

			var referenceObject = testRun.ReferenceObject;
			var name = referenceObject.Name;

			var documentUrl = NSFileManager.DefaultManager.GetTemporaryDirectory().Append($"{name}.arobject", false);

			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				try
				{
					NSError err = null;
					referenceObject.Export(documentUrl, testRun.PreviewImage, out err);
					if (err != null)
					{
						Console.WriteLine($"Error writing reference object : ({err})");
					}
				}
				catch (Exception x)
				{
					AppDelegate.FatalError($"Failed to save the file to '{documentUrl}'");
				}

				// Initiate a share sheet for the scanned object
				var airdropShareSheet = new ShareScanViewController(nextButton, documentUrl);
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					PresentViewController(airdropShareSheet, true, () => { });
				});
			});
		}

		internal void StartLimitedTrackingTimer()
		{
			if (limitedTrackingTimer != null)
			{
				// Cannot have more than one going
				return;
			}

			limitedTrackingTimer = NSTimer.CreateScheduledTimer(2.0, (_) =>
			{
				CancelLimitedTrackingTimer();
				if (scan == null)
				{
					return;
				}
				if (scan.State == Scan.ScanState.DefineBoundingBox
					|| scan.State == Scan.ScanState.Scanning
					|| scan.State == Scan.ScanState.AdjustingOrigin
				   )
				{
					var title = "Limited Tracking";
					var message = "Low tracking quality - it is unlikely that a good reference object can be generated from this scan.";
					var buttonTitle = "Restart Scan";

					ShowAlert(title, message, buttonTitle, true, (x) => State.CurrentState = AppState.StartARSession);
				}
			});
		}

		internal void CancelLimitedTrackingTimer()
		{
			limitedTrackingTimer?.Invalidate();
			limitedTrackingTimer?.Dispose();
			limitedTrackingTimer = null;
		}

		void StartMaxScanTimeTimer()
		{
			if (maxScanTimeTimer != null)
			{
				// Cannot have more than one
				return;
			}

			var timeout = 60.0 * 5;

			maxScanTimeTimer = NSTimer.CreateScheduledTimer(timeout, (_) =>
			{
				CancelMaxScanTimeTimer();
				if (state.CurrentState != AppState.Scanning)
				{
					return;
				}

				var title = "Scan is taking too long";
				var message = $"Scanning consumes a lot of resources. This scan has been running for {(int)timeout}s. " +
					"Consider closing the app and letting the device rest for a few minutes.";
				var buttonTitle = "OK";
				ShowAlert(title, message, buttonTitle, true, (x) => { });
			});
		}

		private void CancelMaxScanTimeTimer()
		{
			maxScanTimeTimer?.Invalidate();
			maxScanTimeTimer?.Dispose();
			maxScanTimeTimer = null;
		}



		private T TryGet<T>(NSDictionary dict, NSObject key) where T : NSObject
		{
			if (dict.ContainsKey(key))
			{
				return dict[key] as T;
			}
			return null;
		}

		private void DisplayWarningIfInLowPowerMode(NSNotification notification)
		{
			if (NSProcessInfo.ProcessInfo.LowPowerModeEnabled)
			{
				var title = "Low Power Mode is enabled";
				var message = "Performance may be impacted. For best results, disable " +
					"Low Power Mode in Settings > Battery, and restart the scan.";
				ShowAlert(title, message);
			}
		}

		private void ObjectOriginPositionChanged(NSNotification notification)
		{
			var origin = notification.Object as ObjectOrigin;
			if (origin == null)
			{
				return;
			}
			var pos = origin.Position;
			var message = $"Current local origin position in meters: [{pos.X:F2}, {pos.Y:F2}, {pos.Z:F2}]";
			sessionInfo.DisplayMessage(message, 1.5);
		}

		private void BoundingBoxPositionChanged(NSNotification notification)
		{

			var boundingBox = notification.Object as BoundingBox;
			var cameraPos = sceneView.PointOfView?.WorldPosition;
			if (boundingBox == null || !cameraPos.HasValue)
			{
				return;
			}

			var distanceFromCamera = $"{boundingBox.WorldPosition.Distance(cameraPos.Value):2F}";
			sessionInfo.DisplayMessage($"Current bounding box distance: ({distanceFromCamera}) m", 1.5);
		}

		private void BoundingBoxExtentChanged(NSNotification notification)
		{
			var boxedExtent = TryGet<SimpleBox<NVector3>>(notification.UserInfo, BoundingBox.BoxExtentUserInfoKey);
			if (boxedExtent == null)
			{
				return;
			}
			var extent = boxedExtent.Value;

			var message = $"Current bounding box in meters:\n (x : {extent.X:F2}) (y : {extent.Y:F2}) ( z : {extent.Z:F2})";
			sessionInfo.DisplayMessage(message, 1.5);
		}

		private void ScanPercentageChanged(NSNotification notification)
		{
			var pctNum = TryGet<NSNumber>(notification.UserInfo, BoundingBox.ScanPercentageUserKey);
			if (pctNum == null)
			{
				return;
			}
			double percentage = pctNum.DoubleValue;
			// Switch to the next state if scan is complete
			if (percentage >= 100.0)
			{
				State.SwitchToNextState();
			}
			else
			{
				DispatchQueue.MainQueue.DispatchAsync(() => navigationBarController.SetNavigationBarTitle($"Scan ({percentage})"));
			}
		}




		internal void BackFromBackground()
		{
			if (state.CurrentState == AppState.Scanning)
			{
				var title = "Warning: Scan may be broken";
				var message = "The scan was interrupted. It is recommended to restart the scan.";
				var buttonTitle = "Restart scan";
				ShowAlert(title, message, buttonTitle, true, (_) => State.CurrentState = AppState.NotReady);
			}
		}

		internal void ShowBlurView(bool visible)
		{
			this.blurView.Hidden = visible;
		}

		internal void ShowAlert(string title, string message, string buttonTitle = "OK", bool showCancel = false, Action<UIAlertAction> handler = null)
		{
			Console.WriteLine($"{title}\n{message}");
			InvokeOnMainThread(() =>
			{
				var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
				alertController.AddAction(UIAlertAction.Create(buttonTitle, UIAlertActionStyle.Default, handler));
				if (showCancel)
				{
					alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (_) => { }));
				}
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					PresentViewController(alertController, true, () => { });
				});
			});
		}

		internal ARCamera CurrentFrameCamera()
		{
			return sceneView?.Session?.CurrentFrame?.Camera;
		}

		partial void didTap(UITapGestureRecognizer sender)
		{
			if (State.CurrentState == AppState.Scanning)
			{
				scan?.DidTap(sender);
			}

			HideInstructions();
		}

		partial void didOneFingerPan(UIPanGestureRecognizer sender)
		{
			if (State.CurrentState == AppState.Scanning)
			{
				scan?.DidOneFingerPan(sender);
			}

			HideInstructions();
		}

		partial void didTwoFingerPan(ThresholdPanGestureRecognizer sender)
		{
			if (State.CurrentState == AppState.Scanning)
			{
				scan?.DidTwoFingerPan(sender);
			}

			HideInstructions();
		}

		partial void didRotate(ThresholdRotationGestureRecognizer sender)
		{
			if (State.CurrentState == AppState.Scanning)
			{
				scan?.DidRotate(sender);
			}

			HideInstructions();
		}

		partial void didLongPress(UILongPressGestureRecognizer sender)
		{
			if (State.CurrentState == AppState.Scanning)
			{
				scan?.DidLongPress(sender);
			}

			HideInstructions();
		}

		partial void didPinch(ThresholdPinchGestureRecognizer sender)
		{
			if (State.CurrentState == AppState.Scanning)
			{
				scan?.DidPinch(sender);
			}

			HideInstructions();
		}

		// IUIGestureRecognizerDelegate
		[Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
		internal bool ShouldRecognizeSimultaneously(UIGestureRecognizer first, UIGestureRecognizer second)
		{
			if (first is UIRotationGestureRecognizer && second is UIPinchGestureRecognizer)
			{
				return true;
			}
			if (first is UIRotationGestureRecognizer && second is UIPanGestureRecognizer)
			{
				return true;
			}
			if (first is UIPinchGestureRecognizer && second is UIRotationGestureRecognizer)
			{
				return true;
			}
			if (first is UIPinchGestureRecognizer && second is UIPanGestureRecognizer)
			{
				return true;
			}
			if (first is UIPanGestureRecognizer && second is UIPinchGestureRecognizer)
			{
				return true;
			}
			if (first is UIPanGestureRecognizer && second is UIRotationGestureRecognizer)
			{
				return true;
			}
			return false;
		}
	}
}
