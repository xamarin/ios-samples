using System;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class FetchRecordZoneSample : CodeSample
	{
		public FetchRecordZoneSample ()
			: base (title: "FetchRecordZone",
					className: "CKDatabase",
					methodName: ".FetchRecordZone()",
					descriptionKey: "Zones.FetchRecordZone",
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

			var id = new CKRecordZoneID (zoneName, CKContainer.OwnerDefaultName);
			var zone = await privateDB.FetchRecordZoneAsync (id);

			var results = new Results ();
			if (zone != null)
				results.Items.Add (new CKRecordZoneWrapper (zone));

			return results;
		}
	}
}
