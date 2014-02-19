using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;
using MonoTouch.AVFoundation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreText;
using MonoTouch.ObjCRuntime;
using System.Threading;

namespace QRchestra
{
	public partial class ReceiveViewController : UIViewController
	{
		UIPopoverController sendPopoverController;

		int barcodeIndex;

		AVCaptureVideoPreviewLayer previewLayer;
		CALayer barcodeTargetLayer;
		SessionManager sessionManager;
		Synth synth;
		NSTimer stepTimer;
		NSTimer barcodeTimer;

		public UIColor OverlayColor {
			get {
				return UIColor.Green;
			}
		}

		CGPath createPathForPoints (NSDictionary[] points)
		{
			CGPath path = new CGPath ();
			PointF point;

			if (points.Length > 0) {
				points [0].ToPoint (out point);
				path.MoveToPoint (point);

				int i = 1;
				while (i < points.Length) {
					points [i].ToPoint (out point);
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
				LayerVideoGravity = AVLayerVideoGravity.ResizeAspectFill
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

			stepTimer = NSTimer.CreateRepeatingScheduledTimer (0.15, step);
		}

		partial void handleTap (UIGestureRecognizer recognizer)
		{
			PointF tapPoint = recognizer.LocationInView (previewView);
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

		void focus (PointF point)
		{
			PointF convertedFocusPoint = previewLayer.CaptureDevicePointOfInterestForPoint (point);
			sessionManager.AutoFocus (convertedFocusPoint);
		}

		void expose (PointF point)
		{
			PointF convertedExposurePoint = previewLayer.CaptureDevicePointOfInterestForPoint (point);
			sessionManager.Expose (convertedExposurePoint);
		}

		void resetFocusAndExposure ()
		{
			var pointOfInterest = new PointF (0.5f, 0.5f);

			sessionManager.AutoFocus (pointOfInterest);
			sessionManager.Expose (pointOfInterest);
			sessionManager.FocusMode = AVCaptureFocusMode.ModeContinuousAutoFocus;
		}

		[Export("step")]
		void step ()
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

