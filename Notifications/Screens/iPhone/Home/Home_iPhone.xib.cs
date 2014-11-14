
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_Notifications.Screens.iPhone.Home
{
	public partial class Home_iPhone : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public Home_iPhone (IntPtr handle) : base(handle) { }

		[Export("initWithCoder:")]
		public Home_iPhone (NSCoder coder) : base(coder) { }

		public Home_iPhone () : base("Home_iPhone", null) { }
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			//---- when the add 1 minute notification is clicked, add a notification that fires
			// 1 minute from now
			btnAddLocalNotification.TouchUpInside += (s, e) => {
				//---- create the notification
				UILocalNotification notification = new UILocalNotification ();
				
				//---- set the fire date (the date time in which it will fire)
				notification.FireDate = DateTime.Now.AddSeconds (15);
				
				//---- configure the alert stuff
				notification.AlertAction = "View Alert";
				notification.AlertBody = "Your one minute alert has fired!";
				
				//---- modify the badge
				notification.ApplicationIconBadgeNumber = 1;
				
				//---- set the sound to be the default sound
				notification.SoundName = UILocalNotification.DefaultSoundName;
				
//				notification.UserInfo = new NSDictionary();
//				notification.UserInfo[new NSString("Message")] = new NSString("Your 1 minute notification has fired!");
				
				//---- schedule it
				UIApplication.SharedApplication.ScheduleLocalNotification (notification);
				
			};
		}
		
	}
}

