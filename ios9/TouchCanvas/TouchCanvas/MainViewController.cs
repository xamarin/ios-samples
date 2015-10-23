using System;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas {
	public partial class MainViewController : UIViewController {

		bool visualizeAzimuth;

		ReticleView reticleView;
		ReticleView ReticleView {
			get {
				reticleView = reticleView ?? new ReticleView (CGRect.Empty) {
					TranslatesAutoresizingMaskIntoConstraints = false,
					Hidden = true
				};

				return reticleView;
			}
		}

		public CanvasView CanvasView {
			get {
				return (CanvasView)View;
			}
		}

		[Export ("initWithCoder:")]
		public MainViewController (NSCoder coder) : base (coder)
		{
		}

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			CanvasView.AddSubview (ReticleView);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			CanvasView.DrawTouches (touches, evt);

			if (visualizeAzimuth) {
				ReticleView.Hidden = false;
				UpdateReticleView ((UITouch)touches.First());
			}
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			CanvasView.DrawTouches (touches, evt);
			var touch = (UITouch)touches.FirstOrDefault ();
			if (touch == null)
				return;

			UpdateReticleView (touch);

			// Use the last predicted touch to update the reticle.
			UITouch[] predictedTouches = evt?.GetPredictedTouches (touch);
			UITouch predictedTouch = predictedTouches?.LastOrDefault ();

			if (predictedTouch != null)
				UpdateReticleView (predictedTouch, true);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			CanvasView.DrawTouches (touches, evt);
			CanvasView.EndTouches (touches, false);

			ReticleView.Hidden = true;
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			if (touches == null)
				return;

			CanvasView.EndTouches (touches, true);
			ReticleView.Hidden = true;
		}

		public override void TouchesEstimatedPropertiesUpdated (NSSet touches)
		{
			CanvasView.TouchesEstimatedPropertiesUpdated (touches);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.LandscapeRight;
		}

		partial void ClearView (UIBarButtonItem sender)
		{
			CanvasView.Clear ();
		}

		partial void ToggleDebugDrawing (UIButton sender)
		{
			CanvasView.IsDebuggingEnabled = !CanvasView.IsDebuggingEnabled;
			visualizeAzimuth = !visualizeAzimuth;
			sender.Selected = CanvasView.IsDebuggingEnabled;
		}

		partial void ToggleUsePreciseLocations (UIButton sender)
		{
			CanvasView.UsePreciseLocations = !CanvasView.UsePreciseLocations;
			sender.Selected = CanvasView.UsePreciseLocations;
		}

		void UpdateReticleView (UITouch touch, bool predicated = false)
		{
			if (touch == null)
				return;

			ReticleView.PredictedDotLayer.Hidden = !predicated;
			ReticleView.PredictedLineLayer.Hidden = !predicated;

			ReticleView.Center = touch.LocationInView (touch.View);

			var azimuthAngle = touch.GetAzimuthAngle (touch.View);
			var azimuthUnitVector = touch.GetAzimuthUnitVector (touch.View);
			var altitudeAngle = touch.AltitudeAngle;

			if (predicated) {
				ReticleView.PredictedAzimuthAngle = azimuthAngle;
				ReticleView.PredictedAzimuthUnitVector = azimuthUnitVector;
				ReticleView.PredictedAzimuthAngle = altitudeAngle;
			} else {
				ReticleView.ActualAzimuthAngle = azimuthAngle;
				ReticleView.ActualAzimuthUnitVector = azimuthUnitVector;
				ReticleView.ActualAzimuthAngle = altitudeAngle;
			}
		}
	}
}
