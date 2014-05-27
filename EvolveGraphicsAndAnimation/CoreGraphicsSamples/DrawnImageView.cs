using System;
using UIKit;

using CoreGraphics;

namespace CoreGraphicsSamples
{
	public class DrawnImageView : UIView
	{
		public DrawnImageView ()
		{
			BackgroundColor = UIColor.White;
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);
			
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.ScaleCTM (1, -1);
				g.TranslateCTM (0, -Bounds.Height);
				g.DrawImage (rect, UIImage.FromFile ("MyImage.png").CGImage);
			
				float fontSize = 50f;
			
				g.TranslateCTM (0, fontSize);
				g.SetLineWidth (2.0f);
				g.SetStrokeColor (UIColor.Green.CGColor);
				g.SetFillColor (UIColor.Purple.CGColor);
				g.SetShadow (new CGSize (5, 5), 0, UIColor.Blue.CGColor);
			
				g.SetTextDrawingMode (CGTextDrawingMode.FillStroke);
				g.SelectFont ("Helvetica", fontSize, CGTextEncoding.MacRoman);
				g.ShowText ("Hello Evolve!");
			}
		}
	}
}

