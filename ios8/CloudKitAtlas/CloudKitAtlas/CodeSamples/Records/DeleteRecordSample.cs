using System;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas {
	public class DeleteRecordSample : CodeSample {
		public DeleteRecordSample ()
			: base (title: "DeleteRecord",
					className: "CKDatabase",
					methodName: ".DeleteRecord()",
					descriptionKey: "Records.DeleteRecord",
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
			var recordID = new CKRecordID (recordName, zoneId);

			CKRecordID id = await privateDB.DeleteRecordAsync (recordID);
			var results = new Results ();

			if (id != null)
				results.Items.Add (new CKRecordIdWrapper (id));

			return results;
		}
	}
}
