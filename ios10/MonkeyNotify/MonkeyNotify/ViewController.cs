using System;
using UserNotifications;
using UIKit;
using Foundation;

namespace MonkeyNotify {
	public partial class ViewController : UIViewController {
		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
		#endregion

		#region Custom Actions
		partial void SimpleNotificationTapped (Foundation.NSObject sender)
		{
			// Create content
			var content = new UNMutableNotificationContent ();
			content.Title = "Notification Title";
			content.Subtitle = "Notification Subtitle";
			content.Body = "This is the message body of the notification.";

			// Fire trigger in twenty seconds
			var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger (20, false);

			var requestID = "sampleRequest";
			var request = UNNotificationRequest.FromIdentifier (requestID, content, trigger);

			UNUserNotificationCenter.Current.AddNotificationRequest (request, (err) => {
				if (err != null) {
					// Report error
					Console.WriteLine ("Error: {0}", err);
				} else {
					// Report Success
					Console.WriteLine ("Notification Scheduled: {0}", request);
				}
			});
		}

		partial void CustomActionTapped (Foundation.NSObject sender)
		{
			// Create action
			var actionID = "reply";
			var title = "Reply";
			var action = UNNotificationAction.FromIdentifier (actionID, title, UNNotificationActionOptions.None);

			// Create category
			var categoryID = "message";
			var actions = new UNNotificationAction [] { action };
			var intentIDs = new string [] { };
			//var categoryOptions = new UNNotificationCategoryOptions[] { };
			var category = UNNotificationCategory.FromIdentifier (categoryID, actions, intentIDs, UNNotificationCategoryOptions.None);

			// Register category
			var categories = new UNNotificationCategory [] { category };
			UNUserNotificationCenter.Current.SetNotificationCategories (new NSSet<UNNotificationCategory> (categories));

			// Create content
			var content = new UNMutableNotificationContent ();
			content.Title = "Custom Action";
			content.Subtitle = "Notification Subtitle";
			content.Body = "This is the message body of the notification.";
			content.Badge = 1;
			content.CategoryIdentifier = "message";

			// Fire trigger in twenty seconds
			var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger (20, false);

			var requestID = "sampleRequest";
			var request = UNNotificationRequest.FromIdentifier (requestID, content, trigger);

			UNUserNotificationCenter.Current.AddNotificationRequest (request, (err) => {
				if (err != null) {
					// Report error
					Console.WriteLine ("Error: {0}", err);
				} else {
					// Report Success
					Console.WriteLine ("Notification Scheduled: {0}", request);
				}
			});
		}
		#endregion
	}
}
