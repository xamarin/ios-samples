using System;

using UIKit;
using CoreGraphics;

namespace SpeedSketch
{
	public class CanvasContainerView : UIView
	{
		UIView canvasView;

		UIView documentView;
		public UIView DocumentView {
			get {
				return documentView;
			}
			set {
				var previousView = documentView;
				if (previousView != null)
					previousView.RemoveFromSuperview ();

				documentView = value;
				if (documentView != null) {
					documentView.Frame = canvasView.Bounds;
					canvasView.AddSubview (documentView);
				}
			}
		}

		CanvasContainerView (CGRect frame, UIView canvasView)
			: base (frame)
		{
			this.canvasView = canvasView;

			BackgroundColor = UIColor.LightGray;
			AddSubview (canvasView);
		}

		public static CanvasContainerView FromCanvasSize (CGSize canvasSize)
		{
			var screenBounds = UIScreen.MainScreen.Bounds;
			var minDimension = NMath.Min (screenBounds.Width, screenBounds.Height);
			var baseInset = 44f;
			var size = canvasSize.Add (baseInset * 2);
			size.Width = NMath.Max (minDimension, size.Width);
			size.Height = NMath.Max (minDimension, size.Height);

			var frame = new CGRect (CGPoint.Empty, size);

			var canvasOrigin = new CGPoint ((frame.Width - canvasSize.Width) / 2, (frame.Height - canvasSize.Height) / 2);
			var canvasFrame = new CGRect (canvasOrigin, canvasSize);
			var canvasView = new UIView (canvasFrame);
			canvasView.BackgroundColor = UIColor.White;
			canvasView.Layer.ShadowOffset = new CGSize (0, 3);
			canvasView.Layer.ShadowRadius = 4;
			canvasView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			canvasView.Layer.ShadowOpacity = 1f;

			return new CanvasContainerView (frame, canvasView);
		}
	}
}
