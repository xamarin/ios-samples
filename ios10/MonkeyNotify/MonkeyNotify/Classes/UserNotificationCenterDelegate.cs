using System;
using UserNotifications;

namespace MonkeyNotify
{
	public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
	{
		#region Constructors
		public UserNotificationCenterDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void WillPresentNotification (UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
		{
			// Do something with the notification
			Console.WriteLine ("Active Notification: {0}", notification);

            // Tell system to display the notification anyway or use
			// `None` to say we have handled the display locally.
            completionHandler (UNNotificationPresentationOptions.Alert);
		}

		public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{
			// Take action based on Action ID
			switch (response.ActionIdentifier)
			{
				case "reply":
					// Do something
					Console.WriteLine("Received the REPLY custom action.");
					break;
				default:
					// Take action based on identifier
					if (response.IsDefaultAction)
					{
						// Handle default action...
						Console.WriteLine("Handling the default action.");
					}
					else if (response.IsDismissAction)
					{
						// Handle dismiss action
						Console.WriteLine("Handling a custom dismiss action.");
					}
					break;
			}

			// Inform caller it has been handled
			completionHandler();
		}
		#endregion
	}
}