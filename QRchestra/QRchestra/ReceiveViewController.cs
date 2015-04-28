using System;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using AVFoundation;
using CoreAnimation;
using CoreText;
using ObjCRuntime;
using System.Threading;

namespace QRchestra
{
	public partial class ReceiveViewController : UIViewController
	{
		int barcodeIndex;

		AVCaptureVideoPreviewLayer previewLayer;
		CALayer barcodeTargetLayer;
		SessionManager sessionManager;
		Synth synth;
		NSTimer barcodeTimer;

		public UIColor OverlayColor {
			get {
				return UIColor.Green;
			}
		}

		CGPath createPathForPoints (CGPoint[] points)
		{
			CGPath path = new CGPath ();
			CGPoint point;

			if (points.Length > 0) {
				point = points [0];
				path.MoveToPoint (point);

				int i = 1;
				while (i < points.Length) {
					point = points [i];
					path.AddLineToPoint (point);
					i++;
				}

				path.CloseSubpath ();
			}

			return path;
		}

		public ReceiveViewController () : base ("ReceiveViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			sessionManager = new SessionManager ();
			sessionManager.StartRunning ();

			previewLayer = new AVCaptureVideoPreviewLayer (sessionManager.CaptureSession) {
				Frame = previewView.Bounds,
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};

			if (previewLayer.Connection != null && previewLayer.Connection.SupportsVideoOrientation)
				previewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
			previewView.Layer.AddSublayer (previewLayer);
			previewView.Layer.MasksToBounds = true;

			barcodeTargetLayer = new CALayer () {
				Frame = View.Layer.Bounds
			};
			View.Layer.AddSublayer (barcodeTargetLayer);

			synth = new Synth ();
			synth.LoadPreset (this);
		}

		partial void handleTap (UIGestureRecognizer recognizer)
		{
			CGPoint tapPoint = recognizer.LocationInView (previewView);
			focus (tapPoint);
			expose (tapPoint);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public void SendViewControllerFinished (SendViewController controller)
		{
			DismissViewController (true, null);
		}

		partial void showInfo (NSObject sender)
		{
			SendViewController controller = new SendViewController ();
			controller.Finished = SendViewControllerFinished;
			controller.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			PresentViewController (controller, true, null);
		}

		void focus (CGPoint point)
		{
			CGPoint convertedFocusPoint = previewLayer.CaptureDevicePointOfInterestForPoint (point);
			sessionManager.AutoFocus (convertedFocusPoint);
		}

		void expose (CGPoint point)
		{
			CGPoint convertedExposurePoint = previewLayer.CaptureDevicePointOfInterestForPoint (point);
			sessionManager.Expose (convertedExposurePoint);
		}

		void resetFocusAndExposure ()
		{
			var pointOfInterest = new CGPoint (0.5f, 0.5f);

			sessionManager.AutoFocus (pointOfInterest);
			sessionManager.Expose (pointOfInterest);
			sessionManager.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
		}

		[Export("step:")]
		void step (NSTimer timer)
		{
			if (sessionManager.Barcodes == null || sessionManager.Barcodes.Count < 1)
				return;

			lock (sessionManager) {
				barcodeIndex = (barcodeIndex + 1) % sessionManager.Barcodes.Count;
				var barcode = (AVMetadataMachineReadableCodeObject)sessionManager.Barcodes [barcodeIndex];

				if (barcodeTimer != null)
					barcodeTimer.Invalidate ();
				barcodeTimer = NSTimer.CreateScheduledTimer (0.5, this, new Selector ("removeDetectedBarcodeUI"), null, false);
				var transformedBarcode =
					(AVMetadataMachineReadableCodeObject)previewLayer.GetTransformedMetadataObject (barcode);
				CGPath barcodeBoundary = createPathForPoints (transformedBarcode.Corners);

				CATransaction.Begin ();
				CATransaction.DisableActions = true;
				removeDetectedBarcodeUI ();
				barcodeTargetLayer.AddSublayer (barcodeOverlayLayer (barcodeBoundary, OverlayColor));
				CATransaction.Commit ();

				string noteString = barcode.StringValue;
				int note = 0;
				if (int.TryParse (noteString, out note)) {
					note -= 24;
					if (note >= 0 && note <= 127) {
						synth.StartPlayNoteNumber (note);
						Thread.Sleep (TimeSpan.FromMilliseconds (0.5));
						synth.StopPlayNoteNumber (note);
					}
				}
			}
		}

		[Export("removeDetectedBarcodeUI")]
		void removeDetectedBarcodeUI ()
		{
			removeAllSublayersFromLayer (barcodeTargetLayer);
		}

		CAShapeLayer barcodeOverlayLayer (CGPath path, UIColor color)
		{
			return new CAShapeLayer () {
				Path = path,
				LineJoin = CAShapeLayer.JoinRound,
				LineWidth = 2.0f,
				StrokeColor = color.CGColor,
				FillColor = color.ColorWithAlpha (0.2f).CGColor
			};
		}

		void removeAllSublayersFromLayer (CALayer layer)
		{
			if (layer == null)
				return;

			var sublayers = layer.Sublayers;

			if (sublayers == null)
				return;

			foreach (var sublayer in sublayers)
				sublayer.RemoveFromSuperLayer ();
		}
	}
}

