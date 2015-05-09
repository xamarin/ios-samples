using System;

using UIKit;
using Foundation;

namespace WatchNotifications_iOS
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		/// <summary>
		/// Generate a local notification
		/// </summary>
		partial void UIButton7_TouchUpInside (UIButton sender)
		{
			// create the notification
			var notification = new UILocalNotification();

			// set the fire date (the date time in which it will fire)
			notification.FireDate = NSDate.Now.AddSeconds(10); //DateTime.Now.AddSeconds(10));
			notification.TimeZone = NSTimeZone.DefaultTimeZone;
			// configure the alert stuff
			notification.AlertTitle = "Alert Title";
			notification.AlertAction = "Alert Action";
			notification.AlertBody = "Alert Body: 10 sec alert fired!";

			notification.UserInfo = NSDictionary.FromObjectAndKey (new NSString("UserInfo for notification"), new NSString("Notification"));

			// modify the badge - has no effect on the Watch
			//notification.ApplicationIconBadgeNumber = 1;

			// set the sound to be the default sound
			//notification.SoundName = UILocalNotification.DefaultSoundName;

			// schedule it
			UIApplication.SharedApplication.ScheduleLocalNotification(notification);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

