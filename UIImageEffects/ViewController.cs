using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System;

namespace UIImageEffects
{
	partial class ViewController : UIViewController
	{
		UIImage image;
		int imageIndex;
		const string DidFirstRunKey = "DidFirstRun";

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public ViewController () : base ("ViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			image = UIImage.FromBundle ("DisplayImage");
			updateImage (null);

			showAlertForFirstRun ();
		}

		[Export("updateImage:")]
		void updateImage (UITapGestureRecognizer recognizer)
		{
			if (imageIndex > 4)
				imageIndex = 0;

			string effectText = "";
			UIImage effectImage = null;

			switch (imageIndex) {
			case 0:
				effectImage = image;
				break;
			case 1:
				effectImage = image.ApplyLightEffect ();
				effectText = "Light";
				effectLabel.TextColor = UIColor.White;
				break;
			case 2:
				effectImage = image.ApplyExtraLightEffect ();
				effectText = "Extra Light";
				effectLabel.TextColor = UIColor.LightGray;
				break;
			case 3:
				effectImage = image.ApplyDarkEffect ();
				effectText = "Dark";
				effectLabel.TextColor = UIColor.DarkGray;
				break;
			case 4:
				effectImage = image.ApplyTintEffect (UIColor.Blue);
				effectText = "Color tint";
				effectLabel.TextColor = UIColor.DarkGray;
				break;
			default:
				break;
			}

			imageView.Image = effectImage;
			effectLabel.Text = effectText;

			imageIndex++;
		}

		void showAlertForFirstRun ()
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;
			bool didFirstRun = userDefaults.BoolForKey (DidFirstRunKey);
			if (!didFirstRun) {
				new UIAlertView ("Tap to change image effect", "", null, "Dismiss").Show ();
				userDefaults.SetBool (true, DidFirstRunKey);
			}
		}
	}
}
