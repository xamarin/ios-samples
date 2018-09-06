using System;

using UIKit;
using Foundation;
using UserNotifications;

namespace RedGreenNotifications
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        partial void HandleTapRedNotificationButton(UIButton sender)
        {
            bool redEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey(ManageNotificationsViewController.RedNotificationsEnabledKey);
            if (redEnabled)
            {
                var content = new UNMutableNotificationContent()
                {
                    Title = "Red Notification",
                    Body = "This is a red notification",
                    CategoryIdentifier = "red-category"
                };
                var request = UNNotificationRequest.FromIdentifier(
                    Guid.NewGuid().ToString(),
                    content,
                    UNTimeIntervalNotificationTrigger.CreateTrigger(1, false)
                );
                UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
            }
        }

        partial void HandleTapGreenNotificationButton(UIButton sender)
        {
            bool greenEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey(ManageNotificationsViewController.GreenNotificationsEnabledKey);
            if (greenEnabled)
            {
                var content = new UNMutableNotificationContent()
                {
                    Title = "Green Notification",
                    Body = "This is a green notification",
                    CategoryIdentifier = "green-category"
                };
                var request = UNNotificationRequest.FromIdentifier(
                    Guid.NewGuid().ToString(),
                    content,
                    UNTimeIntervalNotificationTrigger.CreateTrigger(1, false)
                );
                UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
            }
        }
    }
}
