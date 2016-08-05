using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using UIKit;
using Foundation;
using AVFoundation;
using CoreGraphics;
using CoreAnimation;
using CoreFoundation;
using CoreText;

using static AVFoundation.AVCaptureVideoOrientation;

namespace AVCamBarcode
{
	enum SessionSetupResult
	{
		success,
		notAuthorized,
		configurationFailed
	}

	class MetadataObjectLayer : CAShapeLayer
	{
		public AVMetadataObject MetadataObject { get; set; }

		bool PathContaints (CGPoint point)
		{
			var path = Path;
			return path != null && path.ContainsPoint (point, false);
		}
	}

	public class CameraViewController : UIViewController, IAVCaptureMetadataOutputObjectsDelegate, ItemSelectionViewControllerDelegate
	{
		const string metadataObjectTypeItemSelectionIdentifier = "MetadataObjectTypes";
		const string sessionPresetItemSelectionIdentifier = "SessionPreset";

		[Outlet ("metadataObjectTypesButton")]
		UIButton metadataObjectTypesButton { get; set; }

		[Outlet ("sessionPresetsButton")]
		UIButton sessionPresetsButton { get; set; }

		[Outlet ("cameraButton")]
		UIButton cameraButton { get; set; }

		[Outlet ("cameraUnavailableLabel")]
		UILabel cameraUnavailableLabel { get; set; }

		[Outlet ("zoomSlider")]
		UISlider zoomSlider { get; set; }

		[Outlet ("previewView")]
		PreviewView previewView { get; set; }

		AVCaptureDeviceInput videoDeviceInput;
		readonly AVCaptureSession session = new AVCaptureSession ();
		readonly AVCaptureMetadataOutput metadataOutput = new AVCaptureMetadataOutput ();

		// Communicate with the session and other session objects on this queue.
		readonly DispatchQueue sessionQueue = new DispatchQueue ("session queue");
		readonly DispatchQueue metadataObjectsQueue = new DispatchQueue ("metadata objects queue");

		NSTimer removeMetadataObjectOverlayLayersTimer;
		readonly AutoResetEvent resetEvent = new AutoResetEvent (true);

		SessionSetupResult setupResult = SessionSetupResult.success;

		bool sessionRunning;

		IDisposable runningChangeToken;

		NSObject runtimeErrorNotificationToken;

		NSObject wasInterruptedNotificationToken;

		NSObject interruptionEndedNotificationToken;

		readonly List<MetadataObjectLayer> metadataObjectOverlayLayers = new List<MetadataObjectLayer> ();

		Dictionary<string, AVMetadataObjectType> barcodeTypeMap;
		Dictionary<string, NSString> presetMap;

		UITapGestureRecognizer tapRecognizer;
		UITapGestureRecognizer OpenBarcodeURLGestureRecognizer {
			get {
				tapRecognizer = tapRecognizer ?? new UITapGestureRecognizer (OpenBarcodeUrl);
				return tapRecognizer;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Disable UI. The UI is enabled if and only if the session starts running.
			metadataObjectTypesButton.Enabled = false;
			sessionPresetsButton.Enabled = false;
			cameraButton.Enabled = false;
			zoomSlider.Enabled = false;

			// Add the open barcode gesture recognizer to the region of interest view.
			previewView.AddGestureRecognizer (OpenBarcodeURLGestureRecognizer);

			// Set up the video preview view.
			previewView.Session = session;

			// Check video authorization status. Video access is required and audio
			// access is optional. If audio access is denied, audio is not recorded
			// during movie recording.
			switch (AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video)) {
			case AVAuthorizationStatus.Authorized:
				// The user has previously granted access to the camera.
				break;


			case AVAuthorizationStatus.NotDetermined:
				// The user has not yet been presented with the option to grant
				// video access. We suspend the session queue to delay session
				// setup until the access request has completed.
				sessionQueue.Suspend ();
				AVCaptureDevice.RequestAccessForMediaType (AVMediaType.Video, granted => {
					if (!granted)
						setupResult = SessionSetupResult.notAuthorized;
					sessionQueue.Resume ();
				});
				break;


			default:
				// The user has previously denied access.
				setupResult = SessionSetupResult.notAuthorized;
				break;
			}

			// Setup the capture session.
			// In general it is not safe to mutate an AVCaptureSession or any of its
			// inputs, outputs, or connections from multiple threads at the same time.
			// Why not do all of this on the main queue?
			// Because AVCaptureSession.startRunning() is a blocking call which can
			// take a long time. We dispatch session setup to the sessionQueue so
			// that the main queue isn't blocked, which keeps the UI responsive.
			sessionQueue.DispatchAsync (ConfigureSession);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			sessionQueue.DispatchAsync (() => {
				switch (setupResult) {
				case SessionSetupResult.success:
					// Only setup observers and start the session running if setup succeeded.
					AddObservers ();
					session.StartRunning ();
					sessionRunning = session.Running;
					break;

				case SessionSetupResult.notAuthorized:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						var message = "AVCamBarcode doesn't have permission to use the camera, please change privacy settings";
						var alertController = UIAlertController.Create ("AVCamBarcode", message, UIAlertControllerStyle.Alert);
						alertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null));
						alertController.AddAction (UIAlertAction.Create ("Settings", UIAlertActionStyle.Default, action => {
							UIApplication.SharedApplication.OpenUrl (new NSUrl (UIApplication.OpenSettingsUrlString));
						}));
						PresentViewController (alertController, true, null);
					});
					break;

				case SessionSetupResult.configurationFailed:
					DispatchQueue.MainQueue.DispatchAsync (() => {
						var message = "Unable to capture media";
						var alertController = UIAlertController.Create ("AVCamBarcode", message, UIAlertControllerStyle.Alert);
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
				if (setupResult == SessionSetupResult.success) {
					session.StopRunning ();
					sessionRunning = session.Running;
					RemoveObservers ();
				}
			});

			base.ViewDidDisappear (animated);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "SelectMetadataObjectTypes") {
				Func<AVMetadataObjectType, string> key = v => v.ToString ();
				barcodeTypeMap = metadataOutput.AvailableMetadataObjectTypes
											   .GetFlags ()
											   .ToDictionary (key);
				var allItems = barcodeTypeMap.Keys.ToArray ();
				var selectedItems = metadataOutput.MetadataObjectTypes
												  .GetFlags ()
												  .Select (key)
												  .ToList ();

				var navigationController = (UINavigationController)segue.DestinationViewController;
				var selectionCtrl = (ItemSelectionViewController)navigationController.ViewControllers [0];

				selectionCtrl.Title = "Metadata Object Types";
				selectionCtrl.Delegate = this;
				selectionCtrl.Identifier = metadataObjectTypeItemSelectionIdentifier;
				selectionCtrl.AllItems = allItems;
				selectionCtrl.SelectedItems = selectedItems;
				selectionCtrl.AllowsMultipleSelection = true;
			} else if (segue.Identifier == "SelectSessionPreset") {
				Func<NSString, string> key = v => v;
				presetMap = AvailableSessionPresets ().ToDictionary (key);
				var allItems = presetMap.Keys.ToArray ();

				var navigationController = (UINavigationController)segue.DestinationViewController;
				var selectionCtrl = (ItemSelectionViewController)navigationController.ViewControllers [0];

				selectionCtrl.Title = "Session Presets";
				selectionCtrl.Delegate = this;
				selectionCtrl.Identifier = sessionPresetItemSelectionIdentifier;
				selectionCtrl.AllItems = allItems;
				selectionCtrl.SelectedItems = new List<string> { key (session.SessionPreset) };
				selectionCtrl.AllowsMultipleSelection = false;
			}
		}

		// Do not allow rotation if the region of interest is being resized.
		public override bool ShouldAutorotate ()
		{
			return !previewView.IsResizingRegionOfInterest;
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			var videoPreviewLayerConnection = previewView.VideoPreviewLayer.Connection;
			if (videoPreviewLayerConnection != null) {
				var deviceOrientation = UIDevice.CurrentDevice.Orientation;
				if (!deviceOrientation.IsPortrait () && !deviceOrientation.IsLandscape ())
					return;

				var newVideoOrientation = VideoOrientationFor (deviceOrientation);
				var oldSize = View.Frame.Size;
				var oldVideoOrientation = videoPreviewLayerConnection.VideoOrientation;
				videoPreviewLayerConnection.VideoOrientation = newVideoOrientation;

				// When we transition to the new size, we need to adjust the region
				// of interest's origin and size so that it stays anchored relative
				// to the camera.
				coordinator.AnimateAlongsideTransition (context => {
					var oldRegionOfInterest = previewView.RegionOfInterest;
					var newRegionOfInterest = new CGRect ();

					if (oldVideoOrientation == LandscapeRight && newVideoOrientation == LandscapeLeft) {
						newRegionOfInterest.X = oldSize.Width - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Y = oldRegionOfInterest.Y;
						newRegionOfInterest.Width = oldRegionOfInterest.Width;
						newRegionOfInterest.Height = oldRegionOfInterest.Height;
					} else if (oldVideoOrientation == LandscapeRight && newVideoOrientation == Portrait) {
						newRegionOfInterest.X = toSize.Width - oldRegionOfInterest.Y - oldRegionOfInterest.Height;
						newRegionOfInterest.Y = oldRegionOfInterest.X;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					} else if (oldVideoOrientation == LandscapeLeft && newVideoOrientation == LandscapeRight) {
						newRegionOfInterest.X = oldSize.Width - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Y = oldRegionOfInterest.Y;
						newRegionOfInterest.Width = oldRegionOfInterest.Width;
						newRegionOfInterest.Height = oldRegionOfInterest.Height;
					} else if (oldVideoOrientation == LandscapeLeft && newVideoOrientation == Portrait) {
						newRegionOfInterest.X = oldRegionOfInterest.Y;
						newRegionOfInterest.Y = oldSize.Width - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					} else if (oldVideoOrientation == Portrait && newVideoOrientation == LandscapeRight) {
						newRegionOfInterest.X = oldRegionOfInterest.Y;
						newRegionOfInterest.Y = toSize.Height - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					} else if (oldVideoOrientation == Portrait && newVideoOrientation == LandscapeLeft) {
						newRegionOfInterest.X = oldSize.Height - oldRegionOfInterest.Y - oldRegionOfInterest.Height;
						newRegionOfInterest.Y = oldRegionOfInterest.X;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					}

					previewView.SetRegionOfInterestWithProposedRegionOfInterest (newRegionOfInterest);
				}, context => {
					sessionQueue.DispatchAsync (() => {
						metadataOutput.RectOfInterest = previewView.VideoPreviewLayer.MapToLayerCoordinates (previewView.RegionOfInterest);
					});
					// Remove the old metadata object overlays.
					RemoveMetadataObjectOverlayLayers ();
				});
			}
		}

		AVCaptureVideoOrientation VideoOrientationFor (UIDeviceOrientation deviceOrientation)
		{
			switch (deviceOrientation) {
			case UIDeviceOrientation.Portrait:
				return Portrait;
			case UIDeviceOrientation.PortraitUpsideDown:
				return PortraitUpsideDown;
			case UIDeviceOrientation.LandscapeLeft:
				return LandscapeLeft;
			case UIDeviceOrientation.LandscapeRight:
				return LandscapeLeft;
			default:
				throw new InvalidProgramException ();
			}
		}

		#region Session Management

		void ConfigureSession ()
		{
			if (setupResult != SessionSetupResult.success)
				return;

			session.BeginConfiguration ();

			var videoDevice = DeviceWithMediaType (AVMediaType.Video, AVCaptureDevicePosition.Back);
			NSError err;
			var vDeviceInput = AVCaptureDeviceInput.FromDevice (videoDevice, out err);
			if (err != null) {
				Console.WriteLine ($"Could not create video device input: ${err}");
				setupResult = SessionSetupResult.configurationFailed;
				session.CommitConfiguration ();
				return;
			}


			if (session.CanAddInput (vDeviceInput)) {
				session.AddInput (vDeviceInput);
				videoDeviceInput = vDeviceInput;
			} else {
				Console.WriteLine ("Could not add video device input to the session");
				setupResult = SessionSetupResult.configurationFailed;
				session.CommitConfiguration ();
				return;
			}

			// Add metadata output.
			if (session.CanAddOutput (metadataOutput)) {
				session.AddOutput (metadataOutput);

				// Set this view controller as the delegate for metadata objects.
				metadataOutput.SetDelegate (this, metadataObjectsQueue);
				metadataOutput.MetadataObjectTypes = metadataOutput.AvailableMetadataObjectTypes; // Use all metadata object types by default.
				metadataOutput.RectOfInterest = CGRect.Empty;
			} else {
				Console.WriteLine ("Could not add metadata output to the session");
				setupResult = SessionSetupResult.configurationFailed;
				session.CommitConfiguration ();
				return;
			}

			session.CommitConfiguration ();
		}

		#endregion

		#region Device Configuration

		[Action ("changeCamera")]
		void ChangeCamera ()
		{
			metadataObjectTypesButton.Enabled = false;
			sessionPresetsButton.Enabled = false;
			cameraButton.Enabled = false;
			zoomSlider.Enabled = false;

			// Remove the metadata overlay layers, if any.
			RemoveMetadataObjectOverlayLayers ();

			DispatchQueue.MainQueue.DispatchAsync (() => {
				var currentVideoDevice = videoDeviceInput.Device;
				var currentPosition = currentVideoDevice.Position;

				var preferredPosition = AVCaptureDevicePosition.Unspecified;

				switch (currentPosition) {
				case AVCaptureDevicePosition.Unspecified:
				case AVCaptureDevicePosition.Front:
					preferredPosition = AVCaptureDevicePosition.Back;
					break;

				case AVCaptureDevicePosition.Back:
					preferredPosition = AVCaptureDevicePosition.Front;
					break;
				}

				var videoDevice = DeviceWithMediaType (AVMediaType.Video, preferredPosition);
				if (videoDevice != null) {
					NSError err;
					var vDeviceInput = AVCaptureDeviceInput.FromDevice (videoDevice, out err);
					if (err != null) {
						Console.WriteLine ($"Error occured while creating video device input: {err}");
						return;
					}

					session.BeginConfiguration ();

					// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
					session.RemoveInput (vDeviceInput);

					// When changing devices, a session preset that may be supported
					// on one device may not be supported by another. To allow the
					// user to successfully switch devices, we must save the previous
					// session preset, set the default session preset (High), and
					// attempt to restore it after the new video device has been
					// added. For example, the 4K session preset is only supported
					// by the back device on the iPhone 6s and iPhone 6s Plus. As a
					// result, the session will not let us add a video device that
					// does not support the current session preset.
					var previousSessionPreset = session.SessionPreset;
					session.SessionPreset = AVCaptureSession.PresetHigh;

					if (session.CanAddInput (vDeviceInput)) {
						session.AddInput (vDeviceInput);
						videoDeviceInput = vDeviceInput;
					} else {
						session.AddInput (videoDeviceInput);
					}

					// Restore the previous session preset if we can.
					if (session.CanSetSessionPreset (previousSessionPreset))
						session.SessionPreset = previousSessionPreset;

					session.CommitConfiguration ();
				}

				metadataObjectTypesButton.Enabled = true;
				sessionPresetsButton.Enabled = true;
				cameraButton.Enabled = true;
				zoomSlider.Enabled = true;
				zoomSlider.MaxValue = (float)NMath.Min (videoDeviceInput.Device.ActiveFormat.VideoMaxZoomFactor, 8);
				zoomSlider.Value = (float)videoDeviceInput.Device.VideoZoomFactor;
			});
		}

		AVCaptureDevice DeviceWithMediaType (NSString mediaType, AVCaptureDevicePosition position)
		{
			return AVCaptureDevice.DevicesWithMediaType (mediaType)
				                  .FirstOrDefault (d => d.Position == position);
		}

		[Action ("zoomCamera:")]
		void ZoomCamera (UISlider slider)
		{
			var device = videoDeviceInput.Device;

			NSError err;
			videoDeviceInput.Device.LockForConfiguration (out err);
			if (err != null) {
				Console.WriteLine ($"Could not lock for configuration: {err}");
				return;
			}

			device.VideoZoomFactor = slider.Value;
			device.UnlockForConfiguration ();
		}

		#endregion

		#region Drawing Metadata Object Overlay Layers

		// An AutoResetEvent instance is used for drawing metadata object overlays so that
		// only one group of metadata object overlays is drawn at a time.

		MetadataObjectLayer CreateMetadataObjectOverlayWithMetadataObject (AVMetadataObject metadataObject)
		{
			// Transform the metadata object so the bounds are updated to reflect those of the video preview layer.
			var transformedMetadataObject = previewView.VideoPreviewLayer.GetTransformedMetadataObject (metadataObject);

			// Create the initial metadata object overlay layer that can be used for either machine readable codes or faces.
			var metadataObjectOverlayLayer = new MetadataObjectLayer {
				MetadataObject = transformedMetadataObject,
				LineJoin = CAShapeLayer.JoinRound,
				LineWidth = 7,
				StrokeColor = View.TintColor.ColorWithAlpha (0.7f).CGColor,
				FillColor = View.TintColor.ColorWithAlpha (0.3f).CGColor
			};

			var barcodeMetadataObject = transformedMetadataObject as AVMetadataMachineReadableCodeObject;
			if (barcodeMetadataObject != null) {
				var barcodeOverlayPath = BarcodeOverlayPathWithCorners (barcodeMetadataObject.Corners);
				metadataObjectOverlayLayer.Path = barcodeOverlayPath;

				// If the metadata object has a string value, display it.
				if (barcodeMetadataObject.StringValue.Length > 0) {
					var barcodeOverlayBoundingBox = barcodeOverlayPath.BoundingBox;

					var font = UIFont.BoldSystemFontOfSize (19).ToCTFont ();
					var textLayer = new CATextLayer {
						AlignmentMode = CATextLayer.AlignmentCenter,
						Bounds = new CGRect (0, 0, barcodeOverlayBoundingBox.Size.Width, barcodeOverlayBoundingBox.Size.Height),
						ContentsScale = UIScreen.MainScreen.Scale,
						Position = new CGPoint (barcodeOverlayBoundingBox.GetMidX (), barcodeOverlayBoundingBox.GetMidY ()),
						Wrapped = true,

						// Invert the effect of transform of the video preview so the text is orientated with the interface orientation.
						Transform = CATransform3D.MakeFromAffine (previewView.Transform).Invert (default (CATransform3D)),
						AttributedString = new NSAttributedString (barcodeMetadataObject.StringValue, new CTStringAttributes {
							Font = font,
							ForegroundColor = UIColor.White.CGColor,
							StrokeWidth = -5,
							StrokeColor = UIColor.Black.CGColor
						})
					};
					textLayer.SetFont (font);
					metadataObjectOverlayLayer.AddSublayer (textLayer);
				}
			} else if (transformedMetadataObject is AVMetadataFaceObject) {
				metadataObjectOverlayLayer.Path = CGPath.FromRect (transformedMetadataObject.Bounds);
			}
			return metadataObjectOverlayLayer;
		}

		CGPath BarcodeOverlayPathWithCorners (CGPoint [] corners)
		{
			var path = new CGPath ();

			if (corners.Length > 0) {
				var start = corners [0];
				path.MoveToPoint (start);

				for (int i = 1; i < corners.Length; i++) {
					var corner = corners [i];
					path.AddLineToPoint (corner);
				}
				path.CloseSubpath ();
			}

			return path;
		}

		void RemoveMetadataObjectOverlayLayers ()
		{
			metadataObjectOverlayLayers.ForEach (l => l.RemoveFromSuperLayer ());
			metadataObjectOverlayLayers.Clear ();

			removeMetadataObjectOverlayLayersTimer?.Invalidate ();
			removeMetadataObjectOverlayLayersTimer = null;
		}

		void AddMetadataObjectOverlayLayersToVideoPreviewView (IEnumerable<MetadataObjectLayer> layers)
		{
			// Add the metadata object overlays as sublayers of the video preview layer. We disable actions to allow for fast drawing.
			CATransaction.Begin ();
			CATransaction.DisableActions = true;
			foreach (var l in layers)
				previewView.VideoPreviewLayer.AddSublayer (l);
			CATransaction.Commit ();

			// Save the new metadata object overlays.
			metadataObjectOverlayLayers.Clear ();
			metadataObjectOverlayLayers.AddRange (metadataObjectOverlayLayers);

			// Create a timer to destroy the metadata object overlays.
			removeMetadataObjectOverlayLayersTimer = NSTimer.CreateScheduledTimer (TimeSpan.FromSeconds (1), t => RemoveMetadataObjectOverlayLayers());
		}

		void OpenBarcodeUrl (UITapGestureRecognizer openBarcodeURLGestureRecognizer)
		{
			foreach (var metadataObjectOverlayLayer in metadataObjectOverlayLayers) {
				var location = openBarcodeURLGestureRecognizer.LocationInView (previewView);
				if (metadataObjectOverlayLayer.Path.ContainsPoint (location, false)) {
					var barcodeMetadataObject = metadataObjectOverlayLayer.MetadataObject as AVMetadataMachineReadableCodeObject;
					if (barcodeMetadataObject != null) {
						var val = barcodeMetadataObject.StringValue;
						if (!string.IsNullOrWhiteSpace (val)) {
							var url = NSUrl.FromString (val);
							var sharedApp = UIApplication.SharedApplication;
							if (sharedApp.CanOpenUrl (url)) {
								sharedApp.OpenUrl (url);
							}
						}
					}
				}
			}
		}

		#endregion

		#region AVCaptureMetadataOutputObjectsDelegate

		[Export ("captureOutput:didOutputMetadataObjects:fromConnection:")]
		public void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject [] metadataObjects, AVCaptureConnection connection)
		{
			// resetEvent is used to drop new notifications if old ones are still processing, to avoid queueing up a bunch of stale data.
			if (resetEvent.WaitOne (0)) {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					RemoveMetadataObjectOverlayLayers ();
					var layers = new List<MetadataObjectLayer> ();
					foreach (var metadataObject in metadataObjects) {
						var metadataObjectOverlayLayer = CreateMetadataObjectOverlayWithMetadataObject (metadataObject);
						layers.Add (metadataObjectOverlayLayer);
					}

					AddMetadataObjectOverlayLayersToVideoPreviewView (layers);
				});
				resetEvent.Set ();
			}
		}

		#endregion

		#region ItemSelectionViewControllerDelegate

		public void ItemSelectionViewController (ItemSelectionViewController itemSelectionViewController, List<string> selectedItems)
		{
			var identifier = itemSelectionViewController.Identifier;
			if (identifier == metadataObjectTypeItemSelectionIdentifier) {
				sessionQueue.DispatchAsync (() => {
					var objectTypes = selectedItems.Select (t => barcodeTypeMap [t]).Combine ();
					metadataOutput.MetadataObjectTypes = objectTypes;
				});
			} else if (identifier == sessionPresetItemSelectionIdentifier) {
				sessionQueue.DispatchAsync (() => {
					session.SessionPreset = presetMap [selectedItems.First ()];
				});
			}
		}

		#endregion

		#region Change observers

		void AddObservers ()
		{
			runningChangeToken = session.AddObserver ("running", NSKeyValueObservingOptions.New, RunningChanged);

			// Observe the previewView's regionOfInterest to update the AVCaptureMetadataOutput's
			// RectOfInterest when the user finishes resizing the region of interest.
			previewView.RegionOfInterestChanged += RegionOfInterestChanged;

			var center = NSNotificationCenter.DefaultCenter;

			runtimeErrorNotificationToken = center.AddObserver (AVCaptureSession.RuntimeErrorNotification, OnRuntimeErrorNotification, session);

			// A session can only run when the app is full screen. It will be interrupted
			// in a multi-app layout, introduced in iOS 9, see also the documentation of
			// AVCaptureSessionInterruptionReason.Add observers to handle these session
			// interruptions and show a preview is paused message.See the documentation
			// of AVCaptureSessionWasInterruptedNotification for other interruption reasons.
			wasInterruptedNotificationToken = center.AddObserver (AVCaptureSession.WasInterruptedNotification, OnSessionWasInterrupted, session);
			interruptionEndedNotificationToken = center.AddObserver (AVCaptureSession.InterruptionEndedNotification, OnSessionInterruptionEnded, session);
		}

		void RemoveObservers ()
		{
			runningChangeToken.Dispose ();
			previewView.RegionOfInterestChanged -= RegionOfInterestChanged;
			runtimeErrorNotificationToken.Dispose ();
			wasInterruptedNotificationToken.Dispose ();
			interruptionEndedNotificationToken.Dispose ();
		}

		void RunningChanged (NSObservedChange obj)
		{
			var isSessionRunning = ((NSNumber)obj.NewValue).BoolValue;

			DispatchQueue.MainQueue.DispatchAsync (() => {
				metadataObjectTypesButton.Enabled = isSessionRunning;
				sessionPresetsButton.Enabled = isSessionRunning;
				cameraButton.Enabled = isSessionRunning && AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video).Length > 1;
				zoomSlider.Enabled = isSessionRunning;
				zoomSlider.MaxValue = (float)NMath.Min (videoDeviceInput.Device.ActiveFormat.VideoMaxZoomFactor, 8);
				zoomSlider.Value = (float)(videoDeviceInput.Device.VideoZoomFactor);

				// After the session stop running, remove the metadata object overlays,
				// if any, so that if the view appears again, the previously displayed
				// metadata object overlays are removed.
				if (!isSessionRunning)
					RemoveMetadataObjectOverlayLayers ();
			});
		}

		void RegionOfInterestChanged (object sender, EventArgs e)
		{
			var pv = (PreviewView)sender;
			CGRect newRegion = pv.RegionOfInterest;

			// Update the AVCaptureMetadataOutput with the new region of interest.
			sessionQueue.DispatchAsync (() => {
				// Translate the preview view's region of interest to the metadata output's coordinate system.
				metadataOutput.RectOfInterest = previewView.VideoPreviewLayer.MapToLayerCoordinates (newRegion);

				// Ensure we are not drawing old metadata object overlays.
				DispatchQueue.MainQueue.DispatchAsync (RemoveMetadataObjectOverlayLayers);
			});
		}

		void OnRuntimeErrorNotification (NSNotification notification)
		{
			var e = new AVCaptureSessionRuntimeErrorEventArgs (notification);
			var errorVal = e.Error;
			if (errorVal == null)
				return;

			var error = (AVError)(long)errorVal.Code;
			Console.WriteLine ($"Capture session runtime error: {error}");

			// Automatically try to restart the session running if media services were
			// reset and the last start running succeeded. Otherwise, enable the user
			// to try to resume the session running.

			if (error == AVError.MediaServicesWereReset) {
				sessionQueue.DispatchAsync (() => {
					if (sessionRunning) {
						session.StartRunning ();
						sessionRunning = session.Running;
					}
				});
			}
		}

		void OnSessionWasInterrupted (NSNotification notification)
		{
			// In some scenarios we want to enable the user to resume the session running.
			// For example, if music playback is initiated via control center while
			// using AVMetadataRecordPlay, then the user can let AVMetadataRecordPlay resume
			// the session running, which will stop music playback. Note that stopping
			// music playback in control center will not automatically resume the session
			// running. Also note that it is not always possible to resume

			var reasonIntegerValue = ((NSNumber)notification.UserInfo [AVCaptureSession.InterruptionReasonKey]).Int32Value;
			var reason = (AVCaptureSessionInterruptionReason)reasonIntegerValue;

			Console.WriteLine ($"Capture session was interrupted with reason {reason}");
			if (reason == AVCaptureSessionInterruptionReason.VideoDeviceNotAvailableWithMultipleForegroundApps) {
				// Simply fade-in a label to inform the user that the camera is unavailable.
				cameraUnavailableLabel.Hidden = false;
				cameraUnavailableLabel.Alpha = 0;
				UIView.Animate (0.25, () => {
					cameraUnavailableLabel.Alpha = 1;
				});
			}
		}

		void OnSessionInterruptionEnded (NSNotification notification)
		{
			Console.WriteLine ("Capture session interruption ended");
			if (cameraUnavailableLabel.Hidden) {
				UIView.Animate (0.25, () => {
					cameraUnavailableLabel.Alpha = 0;
				}, () => {
					cameraUnavailableLabel.Hidden = true;
				});
			}
		}

		#endregion

		NSString [] AvailableSessionPresets ()
		{
			return AllSessionPresets ().Where (p => session.CanSetSessionPreset (p))
									   .ToArray ();
		}

		static IEnumerable<NSString> AllSessionPresets ()
		{
			yield return AVCaptureSession.PresetPhoto;
			yield return AVCaptureSession.PresetLow;
			yield return AVCaptureSession.PresetMedium;
			yield return AVCaptureSession.PresetHigh;
			yield return AVCaptureSession.Preset352x288;
			yield return AVCaptureSession.Preset640x480;
			yield return AVCaptureSession.Preset1280x720;
			yield return AVCaptureSession.PresetiFrame960x540;
			yield return AVCaptureSession.PresetiFrame1280x720;
			yield return AVCaptureSession.Preset1920x1080;
			yield return AVCaptureSession.Preset3840x2160;
		}
	}
}
