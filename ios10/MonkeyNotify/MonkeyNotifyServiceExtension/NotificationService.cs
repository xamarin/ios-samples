using System;
using Foundation;
using UIKit;
using UserNotifications;

namespace MonkeyNotifyServiceExtension {
	[Register ("NotificationService")]
	public class NotificationService : UNNotificationServiceExtension {
		#region Computed Properties
		Action<UNNotificationContent> ContentHandler { get; set; }
		UNMutableNotificationContent BestAttemptContent { get; set; }
		#endregion

		#region Constructors
		protected NotificationService (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void DidReceiveNotificationRequest (UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
		{
			ContentHandler = contentHandler;
			BestAttemptContent = (UNMutableNotificationContent) request.Content.MutableCopy ();

			// Modify the notification content here...
			BestAttemptContent.Title = $"{BestAttemptContent.Title}[modified]";

			ContentHandler (BestAttemptContent);
		}

		public override void TimeWillExpire ()
		{
			// Called just before the extension will be terminated by the system.
			// Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

			ContentHandler (BestAttemptContent);
		}
		#endregion
	}
}
