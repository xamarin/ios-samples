using System;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas {
	public class FetchRecordSample : CodeSample {
		public FetchRecordSample ()
			: base (title: "FetchRecord",
					className: "CKDatabase",
					methodName: ".FetchRecord()",
					descriptionKey: "Records.FetchRecord",
					inputs: new Input [] {
						new TextInput (label: "recordName", value: string.Empty, isRequired: true),
						new TextInput (label: "zoneName", value: CKRecordZone.DefaultName, isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string zoneName, recordName;
			if (!TryGetString ("zoneName", out zoneName) || !TryGetString ("recordName", out recordName))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var zoneId = new CKRecordZoneID (zoneName, CKContainer.OwnerDefaultName);
			var recordId = new CKRecordID (recordName, zoneId);

			var record = await privateDB.FetchRecordAsync (recordId);
			var results = new Results ();

			if (record != null)
				results.Items.Add (new CKRecordWrapper (record));

			return results;
		}
	}
}
