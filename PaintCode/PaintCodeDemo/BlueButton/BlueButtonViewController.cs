using System;
using UIKit;
using CoreGraphics;

namespace PaintCode
{
	public class BlueButtonViewController : UIViewController
	{
		public BlueButtonViewController ()
		{
		}

		BlueButton button;
		UITextView text;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;

			nfloat statusBarHeight = UIDevice.CurrentDevice.CheckSystemVersion (7,0) ?
				UIApplication.SharedApplication.StatusBarFrame.Height : 0f;
			button = new BlueButton (new CGRect (10, 10 + statusBarHeight, 120, 120 - statusBarHeight));
			
			button.Tapped += (obj) => {
				new UIAlertView ("Tapped", "Button tapped", null, "OK", null).Show ();
			};
			
			View.AddSubview (button);
			
			
			text = new UITextView (new CGRect (10, 100 + statusBarHeight, 300, 300 - statusBarHeight));
			text.Font = UIFont.SystemFontOfSize (14f);
			text.Editable = false;
			text.Text = "PaintCode BlueButton Example\n\n"
				+ "After the button is drawn in PaintCode then added to a UIButton subclass "
				+ "Draw() method override, some color/style properties are tweaked in code "
				+ "to create the TouchDown effect.";
			View.AddSubview (text);
			
		}
	}
}

