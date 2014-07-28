using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace SoZoomy
{
	public partial class ViewController : UIViewController
	{
		AVCaptureSession session;
		AVCaptureDevice device;
		AVCaptureMetadataOutput metadataOutput;
		Dictionary <int, FaceView> faceViews;
		nint? lockedFaceID;
		nfloat lockedFaceSize;
		double lockTime;
		AVPlayer memeEffect;
		AVPlayer beepEffect;
		const float MEME_FLASH_DELAY = 0.7f;
		const float MEME_ZOOM_DELAY = 1.1f;
		const float MEME_ZOOM_TIME = 0.25f;
		IntPtr VideoZoomFactorContext,
			VideoZoomRampingContext,
			MemePlaybackContext;

		float MaxZoom {
			get {
				return (float)Math.Min (device != null ? device.ActiveFormat.VideoMaxZoomFactor : 1, 6);
			}
		}

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ViewController ()
			: base (UserInterfaceIdiomIsPhone ? "ViewController_iPhone" : "ViewController_iPad", null)
		{
			VideoZoomFactorContext = new IntPtr ();
			VideoZoomRampingContext = new IntPtr ();
			MemePlaybackContext = new IntPtr ();
		}

		void setupAVCapture ()
		{
			session = new AVCaptureSession ();
			session.SessionPreset = AVCaptureSession.PresetHigh;
			previewView.Session = session;

			updateCameraSelection ();
			CALayer rootLayer = previewView.Layer;
			rootLayer.MasksToBounds = true;
			(previewView.Layer as AVCaptureVideoPreviewLayer).VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			previewView.Layer.BackgroundColor = UIColor.Black.CGColor;

			setupAVFoundationFaceDetection ();

			if (device != null) {
				device.AddObserver (this, (NSString) "videoZoomFactor", (NSKeyValueObservingOptions)0, 
				                    VideoZoomFactorContext);
				device.AddObserver (this, (NSString) "rampingVideoZoom", (NSKeyValueObservingOptions)0, 
				                    VideoZoomRampingContext);
			}

			session.StartRunning ();
		}

		void setupAVFoundationFaceDetection ()
		{
			faceViews = new Dictionary<int, FaceView> ();

			metadataOutput = new AVCaptureMetadataOutput ();
			if (!session.CanAddOutput (metadataOutput)) {
				metadataOutput = null;
				return;
			}

			var metaDataObjectDelegate = new MetaDataObjectDelegate ();
			metaDataObjectDelegate.DidOutputMetadataObjectsAction = DidOutputMetadataObjects;

			metadataOutput.SetDelegate (metaDataObjectDelegate, DispatchQueue.MainQueue);
			session.AddOutput (metadataOutput);

			if (!metadataOutput.AvailableMetadataObjectTypes.Contains (AVMetadataObject.TypeFace)) {
				teardownAVFoundationFaceDetection ();
				return;
			}

			metadataOutput.MetadataObjectTypes = new NSString[] { AVMetadataObject.TypeFace };
			updateAVFoundationFaceDetection ();
		}

		void updateAVFoundationFaceDetection ()
		{
			if (metadataOutput != null)
				metadataOutput.ConnectionFromMediaType (AVMediaType.Metadata).Enabled = true;
		}

		void teardownAVFoundationFaceDetection ()
		{
			if (metadataOutput != null)
				session.RemoveOutput (metadataOutput);

			metadataOutput = null;
			faceViews = null;
		}

		void teardownAVCapture ()
		{
			session.StopRunning ();

			teardownAVFoundationFaceDetection ();

			device.UnlockForConfiguration ();
			device.RemoveObserver (this, (NSString) "videoZoomFactor");
			device.RemoveObserver (this, (NSString) "rampingVideoZoom");
			device = null;

			session = null;
		}

		AVCaptureDeviceInput pickCamera ()
		{
			AVCaptureDevicePosition desiredPosition = AVCaptureDevicePosition.Back;
			bool hadError = false;

			foreach (var device in AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video)) {
				if (device.Position == desiredPosition) {
					NSError error = null;
					AVCaptureDeviceInput input = AVCaptureDeviceInput.FromDevice (device, out error);

					if (error != null) {
						hadError = true;
						displayErrorOnMainQueue (error, "Could not initialize for AVMediaTypeVideo");
					} else if (session.CanAddInput (input))
						return input;
				}
			}

			if (!hadError)
				displayErrorOnMainQueue (null, "No camera found for requested orientation");

			return null;
		}

		void updateCameraSelection ()
		{
			session.BeginConfiguration ();

			AVCaptureInput[] oldInputs = session.Inputs;
			foreach (var oldInput in oldInputs)
				session.RemoveInput (oldInput);

			AVCaptureDeviceInput input = pickCamera ();
			if (input == null) {
				foreach (var oldInput in oldInputs)
					session.AddInput (oldInput);
			} else {
				session.AddInput (input);
				device = input.Device;

				NSError error;
				if (!device.LockForConfiguration (out error))
					Console.WriteLine ("Could not lock for device: " + error.LocalizedDescription);

				updateAVFoundationFaceDetection ();
			}

			session.CommitConfiguration ();
		}

		public void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject[] faces, AVCaptureConnection connection)
		{
			List<int> unseen = faceViews.Keys.ToList ();
			List<int> seen = new List<int> ();

			CATransaction.Begin ();
			CATransaction.SetValueForKey (NSObject.FromObject (true), (NSString) (CATransaction.DisableActions.ToString ()));

			foreach (var face in faces) {
				int faceId =  (int)(face as AVMetadataFaceObject).FaceID;
				unseen.Remove (faceId);
				seen.Add (faceId);

				FaceView view;
				if (faceViews.ContainsKey (faceId))
					view = faceViews [faceId];
				else {
					view = new FaceView ();
					view.Layer.CornerRadius = 10;
					view.Layer.BorderWidth = 3;
					view.Layer.BorderColor = UIColor.Green.CGColor;
					previewView.AddSubview (view);
					faceViews.Add (faceId, view);
					view.Id = faceId;
					view.Callback = TouchCallBack;
					if (lockedFaceID != null)
						view.Alpha = 0;
				}

				AVMetadataFaceObject adjusted = (AVMetadataFaceObject)(previewView.Layer as AVCaptureVideoPreviewLayer).GetTransformedMetadataObject (face);
				view.Frame = adjusted.Bounds;
			}

			foreach (int faceId in unseen) {
				FaceView view = faceViews [faceId];
				view.RemoveFromSuperview ();
				faceViews.Remove (faceId);
				if (faceId == lockedFaceID)
					clearLockedFace ();
			}

			if (lockedFaceID != null) {
				FaceView view = faceViews [(int)lockedFaceID.GetValueOrDefault ()];
				nfloat size = (nfloat)Math.Max (view.Frame.Size.Width, view.Frame.Size.Height) / device.VideoZoomFactor;
				nfloat zoomDelta = lockedFaceSize / size;
				nfloat lockTime = (nfloat)(CATransition.CurrentMediaTime () - this.lockTime);
				float zoomRate = (float)(Math.Log (zoomDelta) / lockTime);
				if (Math.Abs (zoomDelta) > 0.1)
					device.RampToVideoZoom (zoomRate > 0 ? MaxZoom : 1, zoomRate);
			}

			CATransaction.Commit ();
		}

		void TouchCallBack (int faceId, FaceView view)
		{
			lockedFaceID = faceId;
			lockedFaceSize = (nfloat)Math.Max (view.Frame.Size.Width, view.Frame.Size.Height) / device.VideoZoomFactor;
			lockTime = CATransition.CurrentMediaTime ();

			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (0.3f);
			view.Layer.BorderColor = UIColor.Red.CGColor;
			foreach (var face in faceViews.Values) {
				if (face != view)
					face.Alpha = 0;
			}
			UIView.CommitAnimations ();

			beepEffect.Seek (CMTime.Zero);
			beepEffect.Play ();
		}

		void displayErrorOnMainQueue (NSError error, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (delegate {
				UIAlertView alert = new UIAlertView ();
				if (error != null) {
					alert.Title = message + " (" + error.Code + ")";
					alert.Message = error.LocalizedDescription;
				} else
					alert.Title = message;

				alert.AddButton ("Dismiss");
				alert.Show ();
			});
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			if (device != null) {
				if (lockedFaceID != null)
					clearLockedFace ();
				else {
					UITouch touch = (UITouch)touches.AnyObject;
					CGPoint point = touch.LocationInView (previewView);
					point = (previewView.Layer as AVCaptureVideoPreviewLayer).CaptureDevicePointOfInterestForPoint (point);

					if (device.FocusPointOfInterestSupported)
						device.FocusPointOfInterest = point;
					if (device.ExposurePointOfInterestSupported)
						device.ExposurePointOfInterest = point;
					if (device.IsFocusModeSupported (AVCaptureFocusMode.ModeAutoFocus))
						device.FocusMode = AVCaptureFocusMode.ModeAutoFocus;
				}
			}

			base.TouchesEnded (touches, evt);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (device == null)
				return;

			if (context == VideoZoomFactorContext) {
				setZoomSliderValue (device.VideoZoomFactor);
				memeButton.Enabled = (device.VideoZoomFactor > 1);
			} else if (context == VideoZoomRampingContext) {
				slider.Enabled = device.RampingVideoZoom;
				if (slider.Enabled && memeEffect.Rate == 0f)
					clearLockedFace ();
			} else if (context == MemePlaybackContext) {
				if (device.TorchAvailable)
					device.TorchMode = AVCaptureTorchMode.Off;
				fadeInFaces ();
			} else
				Console.WriteLine ("Unhandled observation: " + keyPath);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			string path = NSBundle.MainBundle.PathForResource ("Dramatic2", "m4a");
			if (path != null) {
				memeEffect = AVPlayer.FromUrl (NSUrl.FromFilename (path));
				memeEffect.AddObserver (this, (NSString) "rate", (NSKeyValueObservingOptions)0, MemePlaybackContext);
			}
			path = NSBundle.MainBundle.PathForResource ("Sosumi", "wav");
			if (path != null)
				beepEffect = AVPlayer.FromUrl (NSUrl.FromFilename (path));

			setupAVCapture ();

			if (MaxZoom == 1f && device != null) {
				displayErrorOnMainQueue (null, "Device does not support zoom");
				slider.Enabled = false;
			}
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			(previewView.Layer as AVCaptureVideoPreviewLayer).Connection.VideoOrientation = 
				(AVCaptureVideoOrientation)toInterfaceOrientation;
		}

		partial void meme (NSObject sender)
		{
			memeEffect.Seek (CMTime.Zero);
			memeEffect.Play ();
			NSObject.CancelPreviousPerformRequest (this);
			PerformSelector (new ObjCRuntime.Selector ("flash"), null, MEME_FLASH_DELAY);
			PerformSelector (new ObjCRuntime.Selector ("startZoom:"), NSNumber.FromFloat (getZoomSliderValue ()), MEME_ZOOM_DELAY);
			device.VideoZoomFactor = 1;
			foreach (var faceId in faceViews.Keys) {
				FaceView view = faceViews [faceId];
				view.Alpha = 0;
			}
		}

		[Export("flash")]
		void flash ()
		{
			if (device.TorchAvailable)
				device.TorchMode = AVCaptureTorchMode.On;
		}

		[Export("startZoom:")]
		void startZoom (NSNumber target)
		{
			float zoomPower = (float)Math.Log (target.FloatValue);
			device.RampToVideoZoom (target.FloatValue, zoomPower / MEME_ZOOM_TIME);
		}

		partial void sliderChanged (NSObject sender)
		{
			if (device != null && !device.RampingVideoZoom)
				device.VideoZoomFactor = getZoomSliderValue ();
		}

		void clearLockedFace ()
		{
			lockedFaceID = null;
			fadeInFaces ();
			device.CancelVideoZoomRamp ();
		}

		void fadeInFaces ()
		{
			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);
			foreach (var face in faceViews.Values) {
				face.Alpha = 1;
				face.Layer.BorderColor = UIColor.Green.CGColor;
			}
			UIView.CommitAnimations ();
		}

		float getZoomSliderValue ()
		{
			return (float)Math.Pow (MaxZoom, slider.Value);
		}

		void setZoomSliderValue (nfloat value)
		{
			slider.Value = (float)Math.Log (value) / (float)Math.Log (MaxZoom);
		}

		public class MetaDataObjectDelegate : AVCaptureMetadataOutputObjectsDelegate
		{
			public Action<AVCaptureMetadataOutput, AVMetadataObject[], AVCaptureConnection> DidOutputMetadataObjectsAction;

			public override void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject[] faces, AVCaptureConnection connection)
			{
				if (DidOutputMetadataObjectsAction != null)
					DidOutputMetadataObjectsAction (captureOutput, faces, connection);
			}
		}
	}
}

