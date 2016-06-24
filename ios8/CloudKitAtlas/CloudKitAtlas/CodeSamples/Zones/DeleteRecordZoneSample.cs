using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class DeleteRecordZoneSample : CodeSample
	{
		public DeleteRecordZoneSample ()
			: base (title: "DeleteRecordZone",
					className: "CKDatabase",
					methodName: ".DeleteRecordZone()",
					descriptionKey: "Zones.DeleteRecordZone",
					inputs: new Input [] {
						new TextInput (label: "zoneName", value: string.Empty, isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string zoneName;
			if (TryGetString ("zoneName", out zoneName))
				return null;

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var id = new CKRecordZoneID (zoneName, CKContainer.OwnerDefaultName);
			CKRecordZoneID zoneId = await privateDB.DeleteRecordZoneAsync (id);
			var results = new Results ();

			if (zoneId != null)
				results.Items.Add (new CKRecordZoneIdWrapper (zoneId));

			return results;
		}
	}
}
