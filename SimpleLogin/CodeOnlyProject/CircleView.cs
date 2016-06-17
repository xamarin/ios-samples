using System;
using UIKit;
using CoreGraphics;

namespace CodeOnlyDemo
{
	class CircleView : UIView
	{
		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			//get graphics context
			using (var g = UIGraphics.GetCurrentContext())
			{
				// set up drawing attributes
				g.SetLineWidth(10.0f);
				UIColor.Green.SetFill();
				UIColor.Blue.SetStroke();

				// create geometry
				var path = new CGPath();
				path.AddArc(Bounds.GetMidX(), Bounds.GetMidY(), 50f, 0, 2.0f * (float)Math.PI, true);

				// add geometry to graphics context and draw
				g.AddPath(path);
				g.DrawPath(CGPathDrawingMode.FillStroke);
			}
		}

		public CircleView()
		{
			BackgroundColor = UIColor.White;
		}
	}
}

