using System;
using System.Collections.Generic;

using CloudKit;

namespace CloudKitAtlas {
	public class CKNotificationWrapper : IResult {
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
					new AttributeGroup(string.Empty, new Attribute[] {
						new Attribute("notificationID.hashValue", notification.NotificationId.GetHashCode().ToString ()),
						new Attribute("notificationType", NotificationTypeString),
						new Attribute("alertBody", AttrValue(notification.AlertBody)),
						new Attribute("soundName", AttrValue(notification.SoundName)),
						new Attribute("badge", AttrValue(notification.Badge?.ToString())),
						new Attribute("category", AttrValue(notification.Category)),
						new Attribute("subscriptionID", AttrValue(notification.SubscriptionID))
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

		static string AttrValue (string rawValue)
		{
			return string.IsNullOrWhiteSpace (rawValue) ? "–" : rawValue;
		}
	}
}
