using System;
using CoreGraphics;
using UIKit;
using Foundation;


namespace NSZombieApocalypse
{
	public class ZombieMeter : UIView
	{
		float zombieLevel;
		public float ZombieLevel {
			get { 
				return zombieLevel;
			}
			set {
				zombieLevel = Math.Max (0, value);
				this.SetNeedsDisplay (); 
			}
		}

		UILabel label;

		public ZombieMeter (CGRect frame) : base (frame)
		{
			BackgroundColor = UIColor.Clear;
			Layer.CornerRadius = 8;
			ZombieLevel = 0;

			label = new UILabel (new CGRect (0, 0, frame.Size.Width, 0));
			label.TextAlignment = UITextAlignment.Center;
			label.Text = "Zombie Meter";
			label.Font = UIFont.FromName ("Helvetica", 24);
			label.BackgroundColor = UIColor.Clear;
			AddSubview (label);
			label.SizeToFit ();
			var labelFrame = label.Frame;
			label.Frame = new CGRect (labelFrame.X, labelFrame.Y, frame.Size.Width, labelFrame.Size.Height);

		}

		public override bool IsAccessibilityElement {
			get {
				return true;
			}
		}

		public override string AccessibilityLabel {
			get {
				return label.AccessibilityLabel;
			}
		}

		public override string AccessibilityValue {
			get {
				return String.Format ("{0} %", ZombieLevel * 100);
			}
		}

		public override void Draw (CGRect rect)
		{
			nfloat pad = 20;
			nfloat numberOfMeters = 10;
			nfloat meterSpacing = 5;
			nfloat yOrigin = label.Frame.GetMaxY () + 10;

			UIBezierPath background = UIBezierPath.FromRoundedRect (rect, 8);
			UIColor.White.SetFill ();
			background.Fill ();

			UIColor.Black.SetStroke ();

			CGRect meter = new CGRect (
				pad, 
				yOrigin, 
				rect.Size.Width - pad * 2, 
				(rect.Size.Height - yOrigin - (numberOfMeters * meterSpacing)) / numberOfMeters
			);

			for (int k =0; k < numberOfMeters; k++) {
				meter.Y = yOrigin + (meter.Height + meterSpacing) * (numberOfMeters - 1 - k);
				var path = UIBezierPath.FromRoundedRect (meter, 2);
				path.LineWidth = 1;

				float level = ZombieLevel * 10;
				if (level > k) {
					if (k < 3) {

						UIColor.Green.SetFill ();
					} else if (k < 6) {
						UIColor.Blue.SetFill ();
					} else {
						UIColor.Red.SetFill ();
					}

					float diff = (level - k);
					if (diff > 0 && diff < 1) {

						CGRect smallerRect = meter;
						smallerRect = new CGRect (
							smallerRect.X, 
							smallerRect.Y + smallerRect.Size.Height - ((smallerRect.Size.Height) * diff), 
							smallerRect.Size.Width, 
							(smallerRect.Size.Height) * diff
						);
						var smallerPath = UIBezierPath.FromRoundedRect (smallerRect, UIRectCorner.BottomLeft | UIRectCorner.BottomRight, new CGSize (2, 2));	
						smallerPath.Fill ();
					
					} else {
						path.Fill ();
					}
				}
				path.Stroke ();
			}
		}
	}
}

