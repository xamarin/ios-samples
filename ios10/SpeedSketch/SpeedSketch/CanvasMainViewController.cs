using System;
using CoreGraphics;
using UIKit;

using static UIKit.UIViewAutoresizing;
using static SpeedSketch.Helpers;
 

namespace SpeedSketch
{
	public class CanvasMainViewController : UIViewController, IUIGestureRecognizerDelegate
	{
		public CanvasMainViewController ()
		{
		}

		StrokeCGView cgView;
		RingControl leftRingControl;

		StrokeGestureRecognizer fingerStrokeRecognizer;
		StrokeGestureRecognizer pencilStrokeRecognizer;

		UIButton clearButton;
		UIButton pencilButton;

		Action [] configurations;

		readonly StrokeCollection strokeCollection = new StrokeCollection ();

		UIScrollView scrollView;
		CanvasContainerView canvasContainerView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var bounds = View.Bounds;
			var screenBounds = UIScreen.MainScreen.Bounds;
			var maxScreenDimension = NMath.Max (screenBounds.Width, screenBounds.Height);

			UIViewAutoresizing flexibleDimensions = FlexibleWidth | FlexibleHeight;

			scrollView = new UIScrollView (bounds) {
				AutoresizingMask = flexibleDimensions
			};
			View.AddSubview (scrollView);

			var frame = new CGRect (CGPoint.Empty, new CGSize (maxScreenDimension, maxScreenDimension));
			cgView = new StrokeCGView (frame) {
				AutoresizingMask = flexibleDimensions
			};

			View.BackgroundColor = UIColor.White;

			canvasContainerView = CanvasContainerView.FromCanvasSize (cgView.Frame.Size);
			canvasContainerView.DocumentView = cgView;
			scrollView.ContentSize = canvasContainerView.Frame.Size;
			scrollView.ContentOffset = new CGPoint ((canvasContainerView.Frame.Width - scrollView.Bounds.Width) / 2,
													(canvasContainerView.Frame.Height - scrollView.Bounds.Height) / 2);
			scrollView.AddSubview (canvasContainerView);
			scrollView.BackgroundColor = canvasContainerView.BackgroundColor;
			scrollView.MaximumZoomScale = 3;
			scrollView.MinimumZoomScale = 0.5f;
			scrollView.PanGestureRecognizer.AllowedTouchTypes = TouchTypes (UITouchType.Direct);
			scrollView.PinchGestureRecognizer.AllowedTouchTypes = TouchTypes (UITouchType.Direct);

			// TODO: delegate
			//scrollView.Delegate = this;

			// We put our UI elements on top of the scroll view, so we don't want any of the
			// delay or cancel machinery in place.
			scrollView.DelaysContentTouches = false;

			fingerStrokeRecognizer = new StrokeGestureRecognizer (StrokeUpdated) {
				Delegate = this,
				CancelsTouchesInView = false,
				IsForPencil = false,
				CoordinateSpaceView = cgView,
			};
			scrollView.AddGestureRecognizer (fingerStrokeRecognizer);
		}

		void StrokeUpdated ()
		{
			throw new NotImplementedException ();
		}
	}
}
