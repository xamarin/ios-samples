using System;
using Foundation;
using UIKit;
using AVFoundation;

namespace BracketStripes
{
	public partial class CapturePreviewView : UIView
	{
		private NSString capturingStillImageKeypath = new NSString ("capturingStillImage");
		private UIView flashView;
		private AVCaptureVideoPreviewLayer previewLayer;

		private AVCaptureOutput CaptureOutput { get; set; }

		public CapturePreviewView (IntPtr handle) : base (handle)
		{
		}

		public void ConfigureCaptureSession (AVCaptureSession captureSession, AVCaptureStillImageOutput captureOutput)
		{
			if (previewLayer != null) {
				previewLayer.RemoveFromSuperLayer ();
				previewLayer = null;
			}

			previewLayer = new AVCaptureVideoPreviewLayer (captureSession) {
				VideoGravity = AVLayerVideoGravity.ResizeAspect,
				Frame = Bounds
			};

			Layer.AddSublayer (previewLayer);

			CaptureOutput = captureOutput;

			CaptureOutput.AddObserver (this, capturingStillImageKeypath, NSKeyValueObservingOptions.New, IntPtr.Zero);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if ((ofObject == CaptureOutput) && (keyPath == capturingStillImageKeypath)) {
				var ch = new NSObservedChange (change);
				var value = (NSNumber)ch.NewValue;
				AnimateVisualShutter (value.BoolValue);
				return;
			}

			base.ObserveValue (keyPath, ofObject, change, context);
		}

		protected override void Dispose (bool disposing)
		{
			CaptureOutput.RemoveObserver (this, capturingStillImageKeypath);
			base.Dispose (disposing);
		}

		private void AnimateVisualShutter (bool start)
		{
			if (start) {
				if (flashView != null)
					flashView.RemoveFromSuperview ();

				flashView = new UIView (Bounds) {
					BackgroundColor = UIColor.White,
					Alpha = 0.0f
				};

				AddSubview (flashView);

				UIView.Animate (0.1, () => {
					flashView.Alpha = 1f;
				});
			} else {
				UIView.Animate (0.1, () => {
					flashView.Alpha = 1f;
				}, () => {
					flashView.RemoveFromSuperview ();
					flashView = null;
				});
			}
		}
	}
}
