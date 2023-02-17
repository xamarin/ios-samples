using System.Linq;
using System.Threading.Tasks;

using CloudKit;
using CoreLocation;
using Foundation;

namespace CloudKitAtlas {
	public class PerformQuerySample : CodeSample {
		CLLocation Location {
			get {
				object locationObj;
				Data.TryGetValue ("Location", out locationObj);
				return locationObj as CLLocation;
			}
		}

		public PerformQuerySample ()
			: base (title: "PerformQuery",
					className: "CKDatabase",
					methodName: ".PerformQuery()",
					descriptionKey: "Query.PerformQuery",
					inputs: new Input [] {
						new LocationInput (label: "Location", isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			var location = Location;
			if (location == null)
				return null;

			var container = CKContainer.DefaultContainer;
			var publicDB = container.PublicCloudDatabase;
			var query = new CKQuery ("Items", NSPredicate.FromValue (true)) {
				SortDescriptors = new NSSortDescriptor [] {
					new CKLocationSortDescriptor ("location", location)
				}
			};

			var defaultZoneId = new CKRecordZoneID (CKRecordZone.DefaultName, CKContainer.OwnerDefaultName);
			CKRecord [] recordArray = await publicDB.PerformQueryAsync (query, defaultZoneId);
			var results = new Results (alwaysShowAsList: true);

			var len = recordArray.Length;
			if (len == 0)
				ListHeading = "No matching items";
			else if (len == 1)
				ListHeading = "Found 1 matching item:";
			else
				ListHeading = $"Found {recordArray.Length} matching items:";

			results.Items.AddRange (recordArray.Select (r => new CKRecordWrapper (r)));
			return results;
		}
	}
}
