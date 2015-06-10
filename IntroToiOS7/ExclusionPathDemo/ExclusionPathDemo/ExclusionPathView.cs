using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ExclusionPathDemo
{
	public class ExclusionPathView : UITextView
	{
	    CGPath exclusionPath;
		CGPoint initialPoint;
		CGPoint latestPoint;
		UIBezierPath bezierPath;

		public ExclusionPathView (string text)
		{
			Text = text;
			ContentInset = new UIEdgeInsets (20, 0, 0, 0);
			BackgroundColor = UIColor.White;
			exclusionPath = new CGPath ();
			bezierPath = UIBezierPath.Create ();

			LayoutManager.AllowsNonContiguousLayout = false;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			var touch = touches.AnyObject as UITouch;

			if (touch != null) {
				initialPoint = touch.LocationInView (this);
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;

			if (touch != null) {
				latestPoint = touch.LocationInView (this);
				SetNeedsDisplay ();
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			bezierPath.CGPath = exclusionPath;
			TextContainer.ExclusionPaths = new UIBezierPath[] { bezierPath };
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			if (!initialPoint.IsEmpty) {

				using (var g = UIGraphics.GetCurrentContext ()) {

					g.SetLineWidth (4);
					UIColor.Blue.SetStroke ();

					if (exclusionPath.IsEmpty) {
						exclusionPath.AddLines (new CGPoint[] { initialPoint, latestPoint });
					} else {
						exclusionPath.AddLineToPoint (latestPoint);
					}

					g.AddPath (exclusionPath);
					g.DrawPath (CGPathDrawingMode.Stroke);
				}
			}
		}
	}
}
