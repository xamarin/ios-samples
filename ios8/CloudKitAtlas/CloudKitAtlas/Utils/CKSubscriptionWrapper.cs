using System.Collections.Generic;

using CloudKit;
using static CloudKit.CKSubscriptionType;
using static CloudKit.CKSubscriptionOptions;

namespace CloudKitAtlas
{
	public class CKSubscriptionWrapper : IResult
	{
		readonly CKSubscription subscription;

		public string SummaryField {
			get {
				switch (subscription.SubscriptionType) {
				case RecordZone:
					return $"RecordZone({subscription.ZoneID.ZoneName})";
				case Query:
					var queryParams = string.Join (",", GetParams ());
					return $"Query({queryParams})";
				default:
					return subscription.SubscriptionId;
				}
			}
		}

		public List<AttributeGroup> AttributeList {
			get {
				var subscriptionType = "";
				switch (subscription.SubscriptionType) {
				case RecordZone:
					subscriptionType = "RecordZone";
					break;
				case Query:
					subscriptionType = "Query";
					break;
				}

				var options = subscription.SubscriptionOptions;
				var firesOnRecordCreation = options.HasFlag (FiresOnRecordCreation) ? "Y" : "N";
				var firesOnRecordUpdate = options.HasFlag (FiresOnRecordUpdate) ? "Y" : "N";
				var firesOnRecordDeletion = options.HasFlag (FiresOnRecordDeletion) ? "Y" : "N";
				var firesOnce = options.HasFlag (FiresOnce) ? "Y" : "N";

				var predicate = subscription.Predicate;
				var groups = new List<AttributeGroup> {
					new AttributeGroup (title: "", attributes: new Attribute[] {
						new Attribute (key: "subscriptionID", value: subscription.SubscriptionId),
						new Attribute (key: "subscriptionType", value: subscriptionType),
						new Attribute (key: "predicate", value: predicate != null ? predicate.PredicateFormat : string.Empty)
					}),
					new AttributeGroup (title: "Options", attributes: new Attribute [] {
						new Attribute (key: "FiresOnRecordCreation", value: firesOnRecordCreation),
						new Attribute (key: "FiresOnRecordUpdate", value: firesOnRecordUpdate),
						new Attribute (key: "FiresOnRecordDeletion", value: firesOnRecordDeletion),
						new Attribute (key: "FiresOnce", value: firesOnce)
					})
				};
				var zoneID = subscription.ZoneID;
				if (zoneID != null) {
					var attribs = groups [0].Attributes;
					attribs.Add (new Attribute (key: "zoneID"));
					attribs.Add (new Attribute (key: "zoneName", value: subscription.ZoneID.ZoneName, isNested: true));
					attribs.Add (new Attribute (key: "ownerName", value: subscription.ZoneID.OwnerName, isNested: true));

				}
				return groups;
			}
		}

		public CKSubscriptionWrapper (CKSubscription subscription)
		{
			this.subscription = subscription;
		}

		IEnumerable<string> GetParams ()
		{
			var recordType = subscription.RecordType;
			if (recordType != null)
				yield return recordType;

			var predicate = subscription.Predicate;
			if (predicate != null)
				yield return predicate.PredicateFormat;
		}
	}
}
