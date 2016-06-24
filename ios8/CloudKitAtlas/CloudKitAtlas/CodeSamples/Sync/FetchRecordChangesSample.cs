using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	class ChangedRecords
	{
		readonly Results results = new Results (alwaysShowAsList: true);
		readonly Dictionary<CKRecordID, CKRecord> recordsByID = new Dictionary<CKRecordID, CKRecord> ();

		CKServerChangeToken changeToken;

		public CKRecord GetRecordById (CKRecordID recordId)
		{
			return recordsByID [recordId];
		}

		public Results GetRecords ()
		{
			return results;
		}

		public void AddRecord (CKRecord record)
		{
			results.Items.Add (new CKRecordWrapper (record));
			recordsByID [record.Id] = record;
			results.Added.Add (results.Items.Count - 1);
		}

		int IndexOfRecordByRecordID (CKRecordID recordId)
		{
			return results.Items.FindIndex (r => {
				var record = (r as CKRecordWrapper)?.Record;
				return record != null && record.Id == recordId;
			});
		}

		public void MarkRecordAsModified (CKRecord record)
		{
			var index = IndexOfRecordByRecordID (record.Id);
			if (index >= 0)
				results.Modified.Add (index);
		}

		public void MarkRecordAsDeleted (CKRecordID recordID)
		{
			var index = IndexOfRecordByRecordID (recordID);
			if (index > 0)
				results.Deleted.Add (index);
		}

		void RemoveDeletedRecords ()
		{
			foreach (var index in results.Deleted.OrderByDescending (i => i)) {
				var record = ((CKRecordWrapper)results.Items [index]).Record;
				results.Items.RemoveAt (index);
				recordsByID.Remove (record.Id);
			}
		}

		public void SetMoreComing (bool value)
		{
			results.MoreComing = value;
		}

		public void RemoveChanges ()
		{
			RemoveDeletedRecords ();

			results.Added.Clear ();
			results.Deleted.Clear ();
			results.Modified.Clear ();
		}

		public void Reset ()
		{
			changeToken = null;
			results.Reset ();
			recordsByID.Clear ();
		}
	}

	public class FetchRecordChangesSample : CodeSample
	{
		public FetchRecordChangesSample ()
		{
		}

		public override Task<Results> Run ()
		{
			throw new NotImplementedException ();
		}
	}
}

