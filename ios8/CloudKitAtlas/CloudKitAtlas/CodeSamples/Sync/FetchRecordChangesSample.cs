using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	class ChangedRecords
	{
		public Results Results { get; } = new Results (alwaysShowAsList: true);
		readonly Dictionary<CKRecordID, CKRecord> recordsByID = new Dictionary<CKRecordID, CKRecord> ();

		public CKServerChangeToken ChangeToken { get; set; }

		public CKRecord GetRecordById (CKRecordID recordId)
		{
			return recordsByID [recordId];
		}

		public Results GetRecords ()
		{
			return Results;
		}

		public void AddRecord (CKRecord record)
		{
			Results.Items.Add (new CKRecordWrapper (record));
			recordsByID [record.Id] = record;
			Results.Added.Add (Results.Items.Count - 1);
		}

		int IndexOfRecordByRecordID (CKRecordID recordId)
		{
			return Results.Items.FindIndex (r => {
				var record = (r as CKRecordWrapper)?.Record;
				return record != null && record.Id == recordId;
			});
		}

		public void MarkRecordAsModified (CKRecord record)
		{
			var index = IndexOfRecordByRecordID (record.Id);
			if (index >= 0)
				Results.Modified.Add (index);
		}

		public void MarkRecordAsDeleted (CKRecordID recordID)
		{
			var index = IndexOfRecordByRecordID (recordID);
			if (index > 0)
				Results.Deleted.Add (index);
		}

		void RemoveDeletedRecords ()
		{
			foreach (var index in Results.Deleted.OrderByDescending (i => i)) {
				var record = ((CKRecordWrapper)Results.Items [index]).Record;
				Results.Items.RemoveAt (index);
				recordsByID.Remove (record.Id);
			}
		}

		public void SetMoreComing (bool value)
		{
			Results.MoreComing = value;
		}

		public void RemoveChanges ()
		{
			RemoveDeletedRecords ();

			Results.Added.Clear ();
			Results.Deleted.Clear ();
			Results.Modified.Clear ();
		}

		public void Reset ()
		{
			ChangeToken = null;
			Results.Reset ();
			recordsByID.Clear ();
		}
	}

	public class FetchRecordChangesSample : CodeSample
	{
		readonly Dictionary<CKRecordZoneID, ChangedRecords> recordCache = new Dictionary<CKRecordZoneID, ChangedRecords> ();

		public FetchRecordChangesSample ()
			: base (title: "CKFetchRecordChangesOperation",
					className: "CKFetchRecordChangesOperation",
					methodName: ".ctor(CKRecordZoneID, CKServerChangeToken)",
					descriptionKey: "Sync.FetchRecordChanges",
					inputs: new Input [] {
						new TextInput (label: "zoneName", value: string.Empty, isRequired: true),
						new BooleanInput (label: "cache", value: true)
			})
		{
			ListHeading = "Records:";
		}

		public override Task<Results> Run ()
		{
			string zoneName;
			bool shouldCache;
			if (!TryGetString ("zoneName", out zoneName) || !TryGetBool ("cache", out shouldCache))
				throw new InvalidProgramException ();

			var zoneId = new CKRecordZoneID (zoneName, CKContainer.OwnerDefaultName);
			ChangedRecords cache;
			if (!recordCache.TryGetValue (zoneId, out cache))
				recordCache [zoneId] = cache = new ChangedRecords ();

			cache.RemoveChanges ();

			if (!cache.Results.MoreComing && !shouldCache)
				cache.Reset ();

			CKServerChangeToken changeToken = null;

			var token = cache.ChangeToken;
			if (token != null && (shouldCache || cache.Results.MoreComing))
				changeToken = token;

			// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=42163
			//var operation = new CKFetchRecordChangesOperation (zoneId, changeToken);
			var operation = CKFetchRecordChangesOperationFactory.GetOperation (zoneId, changeToken);

			operation.DesiredKeys = new string [] { "name", "location" };
			operation.ResultsLimit = 2;
			operation.RecordChanged = (record) => {
				CKRecord cachedRecord = cache.GetRecordById (record.Id);
				if (cachedRecord != null) {
					foreach (var key in record.AllKeys ())
						cachedRecord [key] = record [key];
					cache.MarkRecordAsModified (cachedRecord);
				} else {
					cache.AddRecord (record);
				}
			};

			operation.RecordDeleted = cache.MarkRecordAsDeleted;

			var tcs = new TaskCompletionSource<Results> ();
			operation.AllChangesReported = (chToken, nsData, nsError) => {
				if (nsError == null) {
					cache.ChangeToken = chToken;
					cache.SetMoreComing (operation.MoreComing);
				}
				tcs.SetResult (cache.GetRecords ());
			};

			operation.Start ();
			return tcs.Task;
		}
	}
}
