using System;
using System.Threading.Tasks;

using CloudKit;
using CoreLocation;
using Foundation;

namespace CloudKitAtlas {
	public class SaveRecordSample : CodeSample {
		public SaveRecordSample ()
			: base (title: "SaveRecord",
					className: "CKDatabase",
					methodName: ".SaveRecord()",
					descriptionKey: "Records.SaveRecord",
					inputs: new Input [] {
						new TextInput (label: "recordName", value: string.Empty),
						new TextInput (label: "zoneName", value: CKRecordZone.DefaultName),
						new TextInput (label: "name", value: string.Empty),
						new LocationInput (label: "location", isRequired: false),
						new ImageInput (label: "asset")
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string recordName, zoneName;
			if (!TryGetString ("recordName", out recordName) || !TryGetString ("zoneName", out zoneName))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var recordType = "Items";

			CKRecord record;
			if (string.IsNullOrWhiteSpace (zoneName)) {
				record = string.IsNullOrWhiteSpace (recordName)
						? new CKRecord (recordType)
						: new CKRecord (recordType, new CKRecordID (recordName));
			} else {
				var zoneID = new CKRecordZoneID (zoneName, CKContainer.OwnerDefaultName);
				record = string.IsNullOrWhiteSpace (recordName)
						? new CKRecord (recordType, zoneID)
						: new CKRecord (recordType, new CKRecordID (recordName, zoneID));
			}

			string name;
			if (TryGetString ("name", out name))
				record ["name"] = (NSString) name;

			CLLocation location;
			if (TryGetLocation ("location", out location))
				record ["location"] = location;

			NSUrl url;
			if (TryGetUrl ("asset", out url))
				record ["asset"] = new CKAsset (url);

			try {
				CKRecord rec = await privateDB.SaveRecordAsync (record);
				return ProcessResult (rec);
			} catch (NSErrorException ex) {
				// In this case we are trying to overwrite an existing record so let's fetch it and modify it.
				if (ex.Error.Code == 14) {
					var rec = await privateDB.FetchRecordAsync (record.Id);
					if (rec != null) {
						foreach (var key in record.AllKeys ())
							rec [key] = record [key];
						rec = await privateDB.SaveRecordAsync (rec);
						return ProcessResult (rec);
					}
				}
				throw ex;
			}
		}

		Results ProcessResult (CKRecord record)
		{
			var results = new Results ();
			if (record != null)
				results.Items.Add (new CKRecordWrapper (record));

			return results;
		}
	}
}
