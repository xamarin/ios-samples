using System;
using UIKit;

using CoreGraphics;

namespace Example_Drawing.Screens.iPad.DrawRectVsPath
{
	public class View : UIView
	{
		#region -= constructors =-

		public View () : base() { }

		#endregion

		// rect changes depending on if the whole view is being redrawn, or just a section
		public override void Draw (CGRect rect)
		{
			Console.WriteLine ("Draw() Called");
			base.Draw (rect);
			
			using (CGContext context = UIGraphics.GetCurrentContext ()) {
				
				// fill the background with white
				// set fill color
				UIColor.White.SetFill ();
				//context.SetRGBFillColor (1, 1, 1, 1f);
				// paint
				context.FillRect (rect);
			
				// draw a rectangle using stroke rect
				UIColor.Blue.SetStroke ();
				context.StrokeRect (new CGRect (10, 10, 200, 100));
				
				// draw a rectangle using a path
				context.BeginPath ();
				context.MoveTo (220, 10);
				context.AddLineToPoint (420, 10);
				context.AddLineToPoint (420, 110);
				context.AddLineToPoint (220, 110);
				context.ClosePath ();
				UIColor.DarkGray.SetFill ();
				context.DrawPath (CGPathDrawingMode.FillStroke);
			
				// draw a rectangle using a path
				CGPath rectPath = new CGPath ();
				rectPath.AddRect (new CGRect (new CGPoint (430, 10), new CGSize (200, 100)));
				context.AddPath (rectPath);
				context.DrawPath (CGPathDrawingMode.Stroke);

			}
		}
	}
}

