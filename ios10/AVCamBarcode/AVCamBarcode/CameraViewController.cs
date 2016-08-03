using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using AVFoundation;
using CoreGraphics;
using CoreFoundation;

using static AVFoundation.AVCaptureVideoOrientation;

namespace AVCamBarcode
{
	// MARK: Session Management

	enum SessionSetupResult
	{
		success,
		notAuthorized,
		configurationFailed
	}

	public class CameraViewController : UIViewController, IAVCaptureMetadataOutputObjectsDelegate, ItemSelectionViewControllerDelegate
	{
		const string metadataObjectTypeItemSelectionIdentifier = "MetadataObjectTypes";
		const string sessionPresetItemSelectionIdentifier = "SessionPreset";

		[Outlet ("metadataObjectTypesButton")]
		UIButton metadataObjectTypesButton { get; set; }

		[Outlet("sessionPresetsButton")]
		UIButton sessionPresetsButton { get; set; }

		[Outlet ("cameraButton")]
		UIButton cameraButton { get; set; }

		[Outlet ("zoomSlider")]
		UISlider zoomSlider { get; set; }

		[Outlet("previewView")]
		PreviewView previewView { get; set; }

		readonly AVCaptureSession session = new AVCaptureSession ();
		readonly AVCaptureMetadataOutput metadataOutput = new AVCaptureMetadataOutput ();

		// Communicate with the session and other session objects on this queue.
		readonly DispatchQueue sessionQueue = new DispatchQueue ("session queue");

		SessionSetupResult setupResult = SessionSetupResult.success;
		bool isSessionRunning;

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
					isSessionRunning = session.Running;
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
					isSessionRunning = session.Running;
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

		public override bool ShouldAutorotate ()
		{
			// Do not allow rotation if the region of interest is being resized.
			return !previewView.IsResizingRegionOfInterest;
		}

		public override void ViewWillTransitionToSize (CGSize size, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (size, coordinator);

			var videoPreviewLayerConnection = previewView.VideoPreviewLayer.Connection;
			if (videoPreviewLayerConnection != null) {
				var deviceOrientation = UIDevice.CurrentDevice.Orientation;
				if (!deviceOrientation.IsPortrait() && !deviceOrientation.IsLandscape())
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
					} else if( oldVideoOrientation == LandscapeRight && newVideoOrientation == Portrait) {
						newRegionOfInterest.X = size.Width - oldRegionOfInterest.Y - oldRegionOfInterest.Height;
						newRegionOfInterest.Y = oldRegionOfInterest.X;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					} else if (oldVideoOrientation == LandscapeLeft && newVideoOrientation == LandscapeRight) {
						newRegionOfInterest.X = oldSize.Width - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Y = oldRegionOfInterest.Y;
						newRegionOfInterest.Width = oldRegionOfInterest.Width;
						newRegionOfInterest.Height = oldRegionOfInterest.Height;
					} else if( oldVideoOrientation == LandscapeLeft && newVideoOrientation == Portrait) {
						newRegionOfInterest.X = oldRegionOfInterest.Y;
						newRegionOfInterest.Y = oldSize.Width - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					} else if( oldVideoOrientation == Portrait && newVideoOrientation == LandscapeRight) {
						newRegionOfInterest.X = oldRegionOfInterest.Y;
						newRegionOfInterest.Y = size.Height - oldRegionOfInterest.X - oldRegionOfInterest.Width;
						newRegionOfInterest.Width = oldRegionOfInterest.Height;
						newRegionOfInterest.Height = oldRegionOfInterest.Width;
					} else if(oldVideoOrientation == Portrait && newVideoOrientation == LandscapeLeft) {
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

		#endregion

		void ConfigureSession ()
		{
			throw new NotImplementedException ();
		}

		void OpenBarcodeUrl (UITapGestureRecognizer openBarcodeURLGestureRecognizer)
		{
			throw new NotImplementedException ();
		}

		public void ItemSelectionViewController (ItemSelectionViewController itemSelectionViewController, List<string> selectedItems)
		{
			throw new NotImplementedException ();
		}

		void AddObservers ()
		{
			
		}

		void RemoveObservers ()
		{
		}

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

		void RemoveMetadataObjectOverlayLayers ()
		{
			throw new NotImplementedException ();
		}

	}
}
