using System;
using Foundation;
using UIKit;

namespace UICatalog {
	public partial class SearchPresentOverNavigationBarViewController : BaseSearchController {
		public SearchPresentOverNavigationBarViewController (IntPtr handle) : base (handle) { }

		partial void SearchButtonClicked (NSObject sender)
		{
			// Create the search controller and make it perform the results updating.
			var searchController = new UISearchController ((UIViewController) null);
			searchController.SetSearchResultsUpdater (UpdateSearchResultsForSearchController);
			searchController.HidesNavigationBarDuringPresentation = false;

			// Present the view controller.
			PresentViewController (searchController, true, null);
		}
	}
}
