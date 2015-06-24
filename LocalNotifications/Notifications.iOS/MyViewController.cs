using System;
using CoreGraphics;

using UIKit;
using Foundation;

namespace Notifications
{   

    public class MyViewController : UIViewController
    {
        private readonly nfloat buttonHeight = 50;
        private readonly nfloat buttonWidth = 300;
		private UIButton button;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            AddButtonToView();

            button.TouchUpInside += (sender, e) =>
                                         {
                                             //---- create the notification
                                             var notification = new UILocalNotification();

                                             //---- set the fire date (the date time in which it will fire)
											notification.FireDate = NSDate.FromTimeIntervalSinceNow(15);

                                             //---- configure the alert stuff
                                             notification.AlertAction = "View Alert";
                                             notification.AlertBody = "Your 15 second alert has fired!";

                                             //---- modify the badge
                                             notification.ApplicationIconBadgeNumber = 1;

                                             //---- set the sound to be the default sound
                                             notification.SoundName = UILocalNotification.DefaultSoundName;

                                             //---- schedule it
                                             UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                                         };
        }

        private void AddButtonToView()
        {
            button = UIButton.FromType(UIButtonType.RoundedRect);

            button.Frame = new CGRect(
                View.Frame.Width / 2 - buttonWidth / 2,
                View.Frame.Height / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight);
            button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
                                       UIViewAutoresizing.FlexibleBottomMargin;
            button.SetTitle("Add notification", UIControlState.Normal);
            button.ContentMode = UIViewContentMode.ScaleToFill;

            View.AddSubview(button);
        }
    }
}
