using System;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.UIKit;

namespace CoreGraphicsSamples
{
	public class TriangleView : UIView
	{
		public TriangleView ()
		{
			BackgroundColor = UIColor.White;
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
			
			//get graphics context
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
			
				//set up drawing attributes
				g.SetLineWidth (10);
				UIColor.Blue.SetFill ();
				UIColor.Red.SetStroke ();
			
				//create geometry
				var path = new CGPath ();
			
				path.AddLines (new PointF[]{
				new PointF (100, 200),
				new PointF (160, 100), 
				new PointF (220, 200)});
			
				path.CloseSubpath ();
			
				//use a dashed line
				g.SetLineDash (0, new float[]{10, 4});
			
				//add geometry to graphics context and draw it
				g.AddPath (path);		
				g.DrawPath (CGPathDrawingMode.FillStroke);
			
				// add the path back to the graphics context so that it is the current path
				g.AddPath (path);
				// set the current path to be the clipping path
				g.Clip ();
			
				// the color space determines how Core Graphics interprets color information
				using (CGColorSpace rgb = CGColorSpace.CreateDeviceRGB()) {
					CGGradient gradient = new CGGradient (rgb, new CGColor[] {
					UIColor.Blue.CGColor,
					UIColor.Yellow.CGColor
				});
				
					// draw a linear gradient
					g.DrawLinearGradient (
					gradient, 
					new PointF (path.BoundingBox.Left, path.BoundingBox.Top), 
					new PointF (path.BoundingBox.Right, path.BoundingBox.Bottom), 
					CGGradientDrawingOptions.DrawsBeforeStartLocation);
				}
			}
		}
	}
}

