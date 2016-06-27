using System;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class SaveRecordZoneSample : CodeSample
	{
		public SaveRecordZoneSample ()
			: base (title: "SaveRecordZone",
					className: "CKDatabase",
					methodName: ".SaveRecordZone()",
					descriptionKey: "Zones.SaveRecordZone",
					inputs: new Input [] {
						new TextInput (label: "zoneName", value: string.Empty, isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string zoneName;
			if (!TryGetString ("zoneName", out zoneName))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			CKRecordZone recordZone = await privateDB.SaveRecordZoneAsync (new CKRecordZone (zoneName));
			var results = new Results ();

			if (recordZone == null)
				throw new InvalidProgramException ();

			if (recordZone != null)
				results.Items.Add (new CKRecordZoneWrapper (recordZone));

			return results;
		}
	}
}
