using System;
using UIKit;
using Foundation;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	public class NotificationsPrivacyManager : IPrivacyManager
	{
		readonly AppDelegate applicationDelegate;
		TaskCompletionSource<object> tcs;

		UIUserNotificationType NotificationTypes {
			get {
				return UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
			}
		}

		public NotificationsPrivacyManager (AppDelegate appDelegate)
		{
			applicationDelegate = appDelegate;

			// After the user interacts with the permissions dialog, AppDelegate.DidRegisterUserNotificationSettings
			// is called. In this example, we've hooked that up to an event
			applicationDelegate.NotificationsRegistered += (obj) => tcs.SetResult(null);
		}

		public Task RequestAccess ()
		{
			if (NotificationTypes == UIUserNotificationType.None) {
				tcs = new TaskCompletionSource<object> ();

				var settings = UIUserNotificationSettings.GetSettingsForTypes (
					UIUserNotificationType.Alert
					| UIUserNotificationType.Badge
					| UIUserNotificationType.Sound,
					new NSSet ());

				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				return tcs.Task;
			} else {
				return Task.FromResult<object> (null);
			}
		}

		public string CheckAccess ()
		{
			return string.Format ("Allowed types: {0}", NotificationTypes);
		}
	}
}