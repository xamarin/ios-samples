using System;
using Foundation;
using CloudKit;
using CoreLocation;
using System.Collections.Generic;
using CoreFoundation;
using System.Threading.Tasks;
using UIKit;

namespace CloudKitAtlas
{
	public class CloudManager : NSObject
	{
		const string PhotoAssetRecordType = "Photos";
		const string PhotoAssetField = "photo";

		const string ItemRecordType = "Items";
		const string NameField = "name";
		const string LocationField = "location";

		const string ReferenceSubItemsRecordType = "ReferenceSubItems";

		CKContainer container;
		CKDatabase publicDatabase;

		public bool Subscribed {
			get {
				return NSUserDefaults.StandardUserDefaults.ValueForKey (new NSString("subscriptionID")) != null;
			}
		}

		public CloudManager ()
		{
			container = CKContainer.DefaultContainer;
			publicDatabase = container.PublicCloudDatabase;
		}

		public async Task<bool> RequestDiscoverabilityPermissionAsync ()
		{
			var status = await container.RequestApplicationPermissionAsync (CKApplicationPermissions.UserDiscoverability);
			return status == CKApplicationPermissionStatus.Granted;
		}

		public async Task<CKDiscoveredUserInfo> DiscoverUserInfoAsync ()
		{
			CKRecordID recordId = await container.FetchUserRecordIdAsync ();
			return await container.DiscoverUserInfoAsync (recordId);
		}

		public async Task<CKRecord> UploadAssetAsync (NSUrl assetUrl)
		{
			var assetRecord = new CKRecord (PhotoAssetRecordType);
			var photo = new CKAsset (assetUrl);
			assetRecord [PhotoAssetField] = photo;

			return await publicDatabase.SaveRecordAsync (assetRecord);
		}

		public async Task<CKRecord> AddRecordAsync (string name, CLLocation location)
		{
			var newRecord = new CKRecord (ItemRecordType);
			newRecord [NameField] = (NSString)name;
			newRecord [LocationField] = location;

			return await publicDatabase.SaveRecordAsync (newRecord);
		}

		public async Task<CKRecord> FetchRecordAsync (string id)
		{
			var current = new CKRecordID (id);
			return await publicDatabase.FetchRecordAsync (current);
		}

		public void QueryForRecords (CLLocation location, Action<List<CKRecord>> completionHandler)
		{
			var radiusInKilometers = NSNumber.FromFloat (5f);
			var predicate = NSPredicate.FromFormat ("distanceToLocation:fromLocation:(location, %@) < %f",
				                new NSObject[] { location, radiusInKilometers });

			var query = new CKQuery (ItemRecordType, predicate) {
               SortDescriptors = new [] { new NSSortDescriptor ("creationDate", false) }
			};

			var queryOperation = new CKQueryOperation (query) {
				DesiredKeys = new [] { NameField }
			};

			var results = new List<CKRecord> ();

			queryOperation.RecordFetched = (record) => results.Add (record);

			queryOperation.Completed = (cursor, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured: {0}", error.Description);
					return;
				}

				DispatchQueue.MainQueue.DispatchAsync (() => completionHandler (results));
			};

			publicDatabase.AddOperation (queryOperation);
		}

		public async Task SaveAsync (CKRecord record)
		{
			try {
				await publicDatabase.SaveRecordAsync (record);
				Console.WriteLine ("Successfuly saved record!");
			} catch (Exception e) {
				Console.WriteLine ("An error occured: {0}", e.Message);
			}
		}

		public async Task DeleteAsync (CKRecord record)
		{
			try {
				await publicDatabase.DeleteRecordAsync (record.Id);
				Console.WriteLine ("Successfuly deleted record!");
			} catch (Exception e) {
				Console.WriteLine ("An error occured: {0}", e.Message);
			}
		}

		public void FetchRecords (string recordType, Action<List<CKRecord>> completionHandler)
		{
			var truePredicate = NSPredicate.FromValue (true);
			var query = new CKQuery (recordType, truePredicate) {
				SortDescriptors = new [] { new NSSortDescriptor ("creationDate", false) }
			};

			var queryOperation = new CKQueryOperation (query) {
				DesiredKeys = new [] { NameField }
			};

			var results = new List<CKRecord> ();

			queryOperation.RecordFetched = (record) => results.Add (record);

			queryOperation.Completed = (cursor, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured: {0}", error.Description);
					return;
				}

				InvokeOnMainThread (() => completionHandler (results));
			};

			publicDatabase.AddOperation (queryOperation);
		}

		public void QueryForRecords (string referenceRecordName, Action<List<CKRecord>> completionHandler)
		{
			var recordId = new CKRecordID (referenceRecordName);
			var parent = new CKReference (recordId, CKReferenceAction.None);

			var predicate = NSPredicate.FromFormat ("parent == %@", parent);
			var query = new CKQuery (ReferenceSubItemsRecordType, predicate) {
				SortDescriptors = new [] { new NSSortDescriptor ("creationDate", false) }
			};

			var queryOperation = new CKQueryOperation (query) {
				DesiredKeys = new [] { NameField }
			};

			var results = new List<CKRecord> ();

			queryOperation.RecordFetched = (record) => results.Add (record);

			queryOperation.Completed = (cursor, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured: {0}", error.Description);
					return;
				}

				DispatchQueue.MainQueue.DispatchAsync (() => completionHandler (results));
			};

			publicDatabase.AddOperation (queryOperation);
		}

		public void Subscribe ()
		{
			if (Subscribed)
				return;

			var truePredicate = NSPredicate.FromValue (true);
			var itemSubscription = new CKSubscription (ItemRecordType, truePredicate, CKSubscriptionOptions.FiresOnRecordCreation);

			var notification = new CKNotificationInfo {
				AlertBody = "New Item Added",
				ShouldSendContentAvailable = true
			};

			itemSubscription.NotificationInfo = notification;

			publicDatabase.SaveSubscription (itemSubscription, (sub, error) => {

				if (error != null) {
					Console.WriteLine ("An error occured: {0}", error.LocalizedDescription);
					return;
				}

				Console.WriteLine ("Subscribed to Item");
				var defaults = NSUserDefaults.StandardUserDefaults;
				defaults.SetBool (true, "subscribed");
				defaults.SetString (sub.SubscriptionId, "subscriptionID");
			});
		}

		public void Unsubscribe ()
		{
			if (!Subscribed)
				return;

			string subscriptionId = NSUserDefaults.StandardUserDefaults.StringForKey ("subscriptionID");

			var modifyOperation = new CKModifySubscriptionsOperation {
				SubscriptionIdsToDelete = new [] { subscriptionId }
			};

			modifyOperation.Completed = ((savedSubscriptions, deletedSubscriptionIds, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured: {0}", error.Description);
					return;
				}

				Console.WriteLine ("Unsubscripted to Item");
				NSUserDefaults.StandardUserDefaults.RemoveObject ("subscriptionID");
			});

			publicDatabase.AddOperation (modifyOperation);
		}
	}
}
