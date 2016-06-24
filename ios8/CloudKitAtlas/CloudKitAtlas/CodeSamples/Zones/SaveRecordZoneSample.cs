using System.Linq;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class SaveRecordZoneSample : CodeSample
	{
		public SaveRecordZoneSample ()
			: base (title: "FetchAllRecordZones",
					className: "CKDatabase",
					methodName: ".FetchAllRecordZones()",
					descriptionKey: "Zones.FetchAllRecordZones")
		{
		}

		public async override Task<Results> Run ()
		{
			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var zones = await privateDB.FetchAllRecordZonesAsync ();
			var results = new Results (alwaysShowAsList: true);

			if(zones != null && zones.Length > 0) {
				results.Items.AddRange (zones.Select (z => new CKRecordZoneWrapper (z)));
				ListHeading = "Zones:";
			} else {
				ListHeading = "No Zones";
			}

			return results;
		}
	}
}
