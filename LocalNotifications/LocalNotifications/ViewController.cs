using Foundation;
using System;
using UIKit;

namespace LocalNotifications
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            button.TouchUpInside += (sender, e) =>
                {
                    // create the notification
                    var notification = new UILocalNotification();

                    // set the fire date (the date time in which it will fire)
                    notification.FireDate = NSDate.FromTimeIntervalSinceNow(10);

                    // configure the alert
                    notification.AlertAction = "View Alert";
                    notification.AlertBody = "Your 10 second alert has fired!";

                    // modify the badge
                    notification.ApplicationIconBadgeNumber = 1;

                    // set the sound to be the default sound
                    notification.SoundName = UILocalNotification.DefaultSoundName;

                    // schedule it
                    UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                    Console.WriteLine("Scheduled...");
                };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}