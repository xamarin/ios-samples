using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	partial class NotificationViewController : UIViewController
	{
		#region Constructors
		public NotificationViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Get the current state of the notification settings
			var settings = UIApplication.SharedApplication.CurrentUserNotificationSettings;

			// Wireup button
			SendButton.TouchUpInside += (sender, e) => {
				// Create a new local notification
				UILocalNotification notification = new UILocalNotification(){
					AlertBody = "Go Bananas - You've got Monkey Mail!",
					AlertAction = null,
					ApplicationIconBadgeNumber = 1,
					Category = "MONKEYMESSAGE_ID",
					FireDate = NSDate.FromTimeIntervalSinceNow(15) // Fire message in 15 seconds
				};

				// Schedule the notification
				UIApplication.SharedApplication.ScheduleLocalNotification(notification);
				Console.WriteLine("Notification scheduled...");
			};

			// Enable the button if the application has been allowed to send notifications
			SendButton.Enabled = ((settings.Types & UIUserNotificationType.Alert) == UIUserNotificationType.Alert);
		}
		#endregion
	}
}
