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

				// scale and translate the CTM so the image appears upright
				g.ScaleCTM (1, -1);
				g.TranslateCTM (0, -Bounds.Height);
				g.DrawImage (rect, UIImage.FromFile ("MyImage.png").CGImage);
			
				// translate the CTM by the font size so it displays on screen
				float fontSize = 35f;
				g.TranslateCTM (0, fontSize);

				// set general-purpose graphics state
				g.SetLineWidth (1.0f);
				g.SetStrokeColor (UIColor.Yellow.CGColor);
				g.SetFillColor (UIColor.Red.CGColor);
				g.SetShadow (new CGSize (5, 5), 0, UIColor.Blue.CGColor);
			
				// set text specific graphics state
				g.SetTextDrawingMode (CGTextDrawingMode.FillStroke);
				g.SelectFont ("Helvetica", fontSize, CGTextEncoding.MacRoman);

				// show the text
				g.ShowText ("Hello Core Graphics");
			}
		}
	}
}

