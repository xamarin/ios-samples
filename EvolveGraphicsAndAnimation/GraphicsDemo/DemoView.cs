using System;
using UIKit;

using CoreGraphics;
using CoreAnimation;

namespace GraphicsDemo
{
	public class DemoView : UIView
	{
		CGPath path;
		CGPoint initialPoint;
		CGPoint latestPoint;

		CALayer layer;

		public DemoView ()
		{
			BackgroundColor = UIColor.White;

			path = new CGPath ();

			//create layer
			layer = new CALayer ();
			layer.Bounds = new CGRect (0, 0, 50, 50);
			layer.Position = new CGPoint (50, 50);
			layer.Contents = UIImage.FromFile ("monkey.png").CGImage;
			layer.ContentsGravity = CALayer.GravityResizeAspect;
			layer.BorderWidth = 1.5f;
			layer.CornerRadius = 5;
			layer.BorderColor = UIColor.Blue.CGColor;
			layer.BackgroundColor = UIColor.Purple.CGColor;
		}

		public override void TouchesBegan (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			
			if (touch != null) {
				initialPoint = touch.LocationInView (this);
			}
		}

		public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			
			if (touch != null) {
				latestPoint = touch.LocationInView (this);
				SetNeedsDisplay ();
			}
		}

		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			//add layer with image and animate along path

			if (layer.SuperLayer == null)
				Layer.AddSublayer (layer);

			// create a keyframe animation for the position using the path
			layer.Position = latestPoint;
			CAKeyFrameAnimation animPosition = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("position");
			animPosition.Path = path;
			animPosition.Duration = 3;
			layer.AddAnimation (animPosition, "position");
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			if (!initialPoint.IsEmpty) {

				//get graphics context
				using(CGContext g = UIGraphics.GetCurrentContext ()){
						
					//set up drawing attributes
					g.SetLineWidth (2);
					UIColor.Red.SetStroke ();

					//add lines to the touch points
					if (path.IsEmpty) {
						path.AddLines (new CGPoint[]{initialPoint, latestPoint});
					} else {
						path.AddLineToPoint (latestPoint);
					}
				
					//use a dashed line
					g.SetLineDash (0, new nfloat[]{5, 2});
									
					//add geometry to graphics context and draw it
					g.AddPath (path);		
					g.DrawPath (CGPathDrawingMode.Stroke);
				}
			}
		}	           
	}
}
