using System;
using CoreGraphics;
using UIKit;

namespace AirLocate
{
	public class CalibrationEndViewController : UIViewController
	{
		int measured_power;

		public CalibrationEndViewController (int measuredPower)
		{
			measured_power = measuredPower;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			var measuredPowerLabel = new UILabel (new CGRect (0, 0, View.Bounds.Width, View.Bounds.Height)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
				Font = UIFont.SystemFontOfSize (32.0f),
				TextAlignment = UITextAlignment.Center,
				Text = measured_power.ToString ()
			};
			View.AddSubview (measuredPowerLabel);

			var doneButton = new UIBarButtonItem (UIBarButtonSystemItem.Done);
			doneButton.Clicked += (sender, e) => {
				NavigationController.PopViewController (true);
			};

			NavigationItem.RightBarButtonItem = doneButton;
		}
	}
}
