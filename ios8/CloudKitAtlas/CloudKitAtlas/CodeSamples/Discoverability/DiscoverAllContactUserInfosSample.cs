using System.Linq;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class DiscoverAllContactUserInfosSample : CodeSample
	{
		public DiscoverAllContactUserInfosSample ()
			: base (title: "DiscoverAllContactUserInfos",
					className: "CKContainer",
					methodName: ".DiscoverAllContactUserInfos()",
					descriptionKey: "Discoverability.DiscoverAllContactUserInfos")
		{
		}

		public async override Task<Results> Run ()
		{
			var container = CKContainer.DefaultContainer;
			var userInfos = await container.DiscoverAllContactUserInfosAsync ();

			var items = userInfos.Select (info => new CKDiscoveredUserInfoWrapper(info)).ToArray ();
			ListHeading = items.Length > 0 ? "Discovered User Infos:" : "No Discoverable Users Found";

			return new Results (items, alwaysShowAsList: true);
		}
	}
}