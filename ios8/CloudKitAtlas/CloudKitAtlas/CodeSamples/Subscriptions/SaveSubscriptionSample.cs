using System;
using System.Threading.Tasks;

using CloudKit;
using Foundation;

namespace CloudKitAtlas {
	public class SaveSubscriptionSample : CodeSample {
		public override string Error {
			get {
				string subscriptionType, zoneName;
				if (TryGetString ("subscriptionType", out subscriptionType)
					&& subscriptionType == "RecordZone"
					&& TryGetString ("zoneName", out zoneName)) {
					if (string.IsNullOrWhiteSpace (zoneName))
						return "zoneName cannot be empty";
					if (zoneName == CKRecordZone.DefaultName)
						return "Cannot create a subscription on the Default Zone";
				}
				return null;
			}
		}

		public SaveSubscriptionSample ()
			: base (title: "SaveSubscription",
					className: "CKDatabase",
					methodName: ".SaveSubscription()",
					descriptionKey: "Subscriptions.SaveSubscription",
					inputs: new Input [] {
						new SelectionInput (label: "subscriptionType", items: new Input[] {
							new Input (label: "RecordZone", toggleIndexes: new int[] {1}),
							new Input (label: "Query", toggleIndexes: new int[] {2, 3, 4, 5, 6})
						}),
						new TextInput (label: "zoneName", value: string.Empty),
						new TextInput (label: "name BEGINSWITH", value: string.Empty, isHidden: true),
						new BooleanInput (label: "FiresOnRecordCreation", value: true, isHidden: true),
						new BooleanInput (label: "FiresOnRecordUpdate", value: true, isHidden: true),
						new BooleanInput (label: "FiresOnRecordDeletion", value: true, isHidden: true),
						new BooleanInput (label: "FiresOnce", value: false, isHidden: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string subscriptionType;
			if (!TryGetString ("subscriptionType", out subscriptionType))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			CKSubscription subscription;
			var notificationInfo = new CKNotificationInfo {
				ShouldBadge = true
			};

			var recordType = "Items";

			string zoneName;
			if (TryGetString ("zoneName", out zoneName) && subscriptionType == "RecordZone") {
				notificationInfo.AlertBody = $"Zone {zoneName} has changed.";
				var zoneID = new CKRecordZoneID (zoneName, CKContainer.OwnerDefaultName);
				subscription = new CKSubscription (zoneID, 0) {
					NotificationInfo = notificationInfo
				};
			} else {
				var subscriptionOptions = (CKSubscriptionOptions) 0;

				bool firesOnRecordCreation;
				if (TryGetBool ("FiresOnRecordCreation", out firesOnRecordCreation) && firesOnRecordCreation)
					subscriptionOptions |= CKSubscriptionOptions.FiresOnRecordCreation;

				bool firesOnRecordUpdate;
				if (TryGetBool ("FiresOnRecordUpdate", out firesOnRecordUpdate) && firesOnRecordUpdate)
					subscriptionOptions |= CKSubscriptionOptions.FiresOnRecordUpdate;

				bool firesOnRecordDeletion;
				if (TryGetBool ("FiresOnRecordDeletion", out firesOnRecordDeletion) && firesOnRecordDeletion)
					subscriptionOptions |= CKSubscriptionOptions.FiresOnRecordDeletion;

				bool firesOnce;
				if (TryGetBool ("FiresOnce", out firesOnce) && firesOnce)
					subscriptionOptions |= CKSubscriptionOptions.FiresOnce;

				string beginsWithText;
				NSPredicate predicate = TryGetString ("name BEGINSWITH", out beginsWithText)
							? NSPredicate.FromFormat ("name BEGINSWITH %@", (NSString) beginsWithText)
							: NSPredicate.FromValue (true);

				notificationInfo.AlertBody = $"Changed {recordType} satisfying {predicate.PredicateFormat}";
				subscription = new CKSubscription (recordType, predicate, subscriptionOptions) {
					NotificationInfo = notificationInfo
				};
			}

			var sub = await privateDB.SaveSubscriptionAsync (subscription);
			var results = new Results ();
			if (sub != null)
				results.Items.Add (new CKSubscriptionWrapper (sub));

			return results;
		}
	}
}
