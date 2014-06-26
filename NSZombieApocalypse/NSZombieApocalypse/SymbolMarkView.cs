using System;
using CoreGraphics;

using UIKit;

namespace NSZombieApocalypse
{
	public class SymbolMarkView :UIButton
	{
		UILabel label; 

		string symbol;
		public string Symbol {
			get { return symbol; }
			set {
				symbol = value;
				label.Text = symbol;
			}
		}

		public SymbolMarkView (CGRect frame): base (frame)
		{
			label = new UILabel (new CGRect (0, 0, frame.Size.Width, frame.Size.Height));
			label.TextAlignment = UITextAlignment.Center;
			label.Font = UIFont.FromName ("HelveticaNeue-Bold", 48);
			label.BackgroundColor = UIColor.Clear;
			AddSubview (label);
		}

		public override void Draw (CGRect rect)
		{
			rect = rect.Inset (4, 4);
			UIBezierPath path = UIBezierPath.FromArc (new CGPoint (rect.GetMidX (), rect.GetMidY ()), rect.Size.Width / 2, 0, 180, true);
			path.LineWidth = 8;

			UIColor.White.SetFill ();
			path.Fill ();

			UIColor.Black.SetStroke ();
			path.Stroke ();
		}
	}
}

