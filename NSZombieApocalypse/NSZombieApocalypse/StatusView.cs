using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace NSZombieApocalypse
{
	public class StatusView : UIView
	{
		UILabel label;
		public string Status {
			set {
				if (value.Equals (label.Text))
					return;

				label.Text = value;
			}
		}

		public StatusView (RectangleF frame) : base (frame)
		{

			Layer.BorderColor = UIColor.Black.CGColor;
			Layer.BorderWidth = 1;
			Layer.CornerRadius = 8;

			BackgroundColor = UIColor.White.ColorWithAlpha (0.75f);
			label = new UILabel (new RectangleF (0, 0, frame.Size.Width, frame.Size.Height));
			label.Lines = 0;
			label.TextAlignment = UITextAlignment.Center;
			AddSubview (label);
			label.BackgroundColor = UIColor.Clear;
			label.Font = UIFont.FromName ("Helvetica", 36);
		}
	}
}

