using System;
using System.Collections.Generic;

using CloudKit;

namespace CloudKitAtlas
{
	public class CKNotificationWrapper : IResult
	{
		readonly CKNotification notification;

		public CKNotificationWrapper (CKNotification notification)
		{
			if (notification == null)
				throw new ArgumentNullException ();

			this.notification = notification;
		}

		public List<AttributeGroup> AttributeList {
			get {
				return new List<AttributeGroup> {
					// TODO: fix camelCase to PascalCase for keys
					new AttributeGroup(string.Empty, new Attribute[] {
						new Attribute("notificationID.hashValue", notification.NotificationId.GetHashCode().ToString ()),
						new Attribute("notificationType", NotificationTypeString),
						new Attribute("alertBody", notification.AlertBody ?? "–"),
						new Attribute("soundName", notification.SoundName ?? "–"),
						new Attribute("badge", notification.Badge?.ToString() ?? "–"),
						new Attribute("category", notification.Category ?? "–"),
						new Attribute("subscriptionID", notification.SubscriptionID ?? "–")
					})
				};
			}
		}

		public string SummaryField {
			get {
				var subscriptionID = notification.SubscriptionID ?? "unknown subscription";
				return notification.AlertBody ?? $"{NotificationTypeString} notification for ${subscriptionID}.";
			}
		}

		string NotificationTypeString {
			get {
				switch (notification.NotificationType) {
				case CKNotificationType.Query:
					return "Query";
				case CKNotificationType.ReadNotification:
					return "ReadNotification";
				case CKNotificationType.RecordZone:
					return "RecordZone";
				default:
					throw new InvalidOperationException ();
				}
			}
		}
	}
}