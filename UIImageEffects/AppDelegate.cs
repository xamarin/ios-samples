using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UIImageEffects
{

	public class EffectsViewController : UIViewController {
		UIImageView imageView;
		UIImage sourceImage;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			imageView = new UIImageView (View.Bounds) {
				UserInteractionEnabled = true
			};
			imageView.AddGestureRecognizer (new UITapGestureRecognizer ((x)=> {
				UpdateImage ();
			}));
			View.AddSubview (imageView);

			sourceImage = UIImage.FromBundle ("DisplayImage");
			Console.WriteLine ("Got: {0}", sourceImage); 
			UpdateImage ();
			var alert = new UIAlertView ("Tap to change image effect", "Tap on the change", null, "Dismiss");
			alert.Show ();
		}

		int effectIndex;

		void UpdateImage ()
		{
	
			UIImage effectImage;

			if (effectIndex > 5)
				effectIndex = 0;
			switch (effectIndex) {
			case 1:
				effectImage = sourceImage.ApplyLightEffect ();
				break;
			case 2:
				effectImage = sourceImage.ApplyExtraLightEffect ();
				break;
			case 3:
				effectImage = sourceImage.ApplyDarkEffect ();
				break;
			case 4:
				effectImage = sourceImage.ApplyTintEffect (UIColor.Blue);
				break;
			case 5:
				effectImage = sourceImage.ApplyTintEffect (UIColor.Green);
				break;
			case 0:
			default:
				effectImage = sourceImage;
				break;
			}
			imageView.Image = effectImage;
			effectIndex++;
		}
	}

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = new EffectsViewController ();
			window.MakeKeyAndVisible ();
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

