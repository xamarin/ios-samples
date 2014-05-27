using System;

using CoreGraphics;
using UIKit;

namespace CoreGraphicsSamples
{
	public class TriangleView : UIView
	{
		public TriangleView ()
		{
			BackgroundColor = UIColor.White;
		}

		public override void Draw (CGRect rect)
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
			
				path.AddLines (new CGPoint[]{
				new CGPoint (100, 200),
				new CGPoint (160, 100), 
				new CGPoint (220, 200)});
			
				path.CloseSubpath ();
			
				//use a dashed line
				g.SetLineDash (0, new nfloat[]{10, 4});
			
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
					new CGPoint (path.BoundingBox.Left, path.BoundingBox.Top), 
					new CGPoint (path.BoundingBox.Right, path.BoundingBox.Bottom), 
					CGGradientDrawingOptions.DrawsBeforeStartLocation);
				}
			}
		}
	}
}

