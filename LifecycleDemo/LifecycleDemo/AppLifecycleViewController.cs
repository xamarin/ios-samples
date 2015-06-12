using System;
using UIKit;
using CoreGraphics;

namespace Lifecycle.iOS
{
    public class AppLifecycleViewController : UIViewController
    {
		UILabel label;
        nfloat labelWidth = 300;
        nfloat labelHeight = 200;

        public AppLifecycleViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            var frame = new CGRect(
                View.Frame.Width / 2 - labelWidth / 2,
                View.Frame.Height / 2 - labelHeight / 2,
                labelWidth,
                labelHeight);

			label = new UILabel(frame);

			label.Text = "App Lifecycle Demo";
			label.Font = UIFont.FromName("Helvetica-Bold", 50f);
			label.AdjustsFontSizeToFitWidth = true;

			// here we can use a notification to let us know when the app has entered the foreground
			// from the background, and update the text in the View
			// this causes a flicker, but we will use it for demo purposes
			UIApplication.Notifications.ObserveWillEnterForeground ((sender, args) => {
				label.Text = "Welcome back!";
			});


            View.AddSubview(label);
        }

    }
}

