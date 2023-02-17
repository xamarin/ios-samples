using System.Linq;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas {
	public class FetchAllSubscriptionsSample : CodeSample {
		public FetchAllSubscriptionsSample ()
			: base (title: "FetchAllSubscriptions",
					className: "CKDatabase",
					methodName: ".FetchAllSubscriptions()",
					descriptionKey: "Subscriptions.FetchAllSubscriptions")
		{
		}

		public async override Task<Results> Run ()
		{
			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var subscriptions = await privateDB.FetchAllSubscriptionsAsync ();
			var results = new Results (alwaysShowAsList: true);

			if (subscriptions != null) {
				if (subscriptions.Length == 0) {
					ListHeading = "No Subscriptions";
				} else {
					ListHeading = "Subscriptions:";
					results.Items.AddRange (subscriptions.Select (s => new CKSubscriptionWrapper (s)));
				}
			}

			return results;
		}
	}
}
