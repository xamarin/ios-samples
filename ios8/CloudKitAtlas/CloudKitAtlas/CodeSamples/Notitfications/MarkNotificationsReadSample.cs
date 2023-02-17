using System.Collections.Generic;
using System.Threading.Tasks;

using UIKit;
using Foundation;
using CloudKit;

namespace CloudKitAtlas {
	public class NotificationsCache {
		public Results Results { get; private set; } = new Results (null, alwaysShowAsList: true);

		public void AddNotification (CKNotification notification)
		{
			Results.Items.Add (new CKNotificationWrapper (notification));
			Results.Added.Add (Results.Items.Count - 1);
		}

		public HashSet<int> AddedIndices {
			get {
				return Results.Added;
			}
		}

		public CKNotificationID [] NewNotificationIDs {
			get {
				var ids = new CKNotificationID [Results.Added.Count];
				var i = 0;
				foreach (var index in Results.Added) {
					var notification = Results.Items [index] as CKNotification;
					var id = notification?.NotificationId;
					if (notification != null && id != null)
						ids [i++] = id;
				}
				return ids;
			}
		}

		public CKNotificationID [] NotificationIDsToBeMarkedAsRead { get; set; } = new CKNotificationID [0];

		public void MarkAsRead ()
		{
			var notificationIDs = NotificationIDsToBeMarkedAsRead;

			foreach (var notificationID in notificationIDs) {
				var index = Results.Items.FindIndex (result => {
					var notification = result as CKNotification;
					if (notification != null)
						return notification.NotificationId == notificationID;
					return false;
				});
				if (index >= 0)
					Results.Added.Remove (index);
			}
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = Results.Added.Count;
		}
	}

	public class MarkNotificationsReadSample : CodeSample {
		public NotificationsCache Cache { get; private set; } = new NotificationsCache ();

		public MarkNotificationsReadSample ()
				: base (title: "CKMarkNotificationsReadOperation",
						className: "CKMarkNotificationsReadOperation",
						methodName: ".ctor(CKNotificationID[])",
						descriptionKey: "Notifications.MarkAsRead")
		{
		}

		public override Task<Results> Run ()
		{
			CKNotificationID [] ids = Cache.NewNotificationIDs;
			var tcs = new TaskCompletionSource<Results> ();

			if (ids.Length > 0) {
				var operation = new CKMarkNotificationsReadOperation (ids);
				operation.Completed = (CKNotificationID [] notificationIDsMarkedRead, NSError operationError) => {
					if (operationError != null) {
						tcs.SetException (new NSErrorException (operationError));
					} else {
						Cache.NotificationIDsToBeMarkedAsRead = notificationIDsMarkedRead;
						tcs.SetResult (Cache.Results);
					}
				};

				operation.Start ();
			} else {
				tcs.SetResult (Cache.Results);
			}

			return tcs.Task;
		}
	}
}
