using System;
using UIKit;

namespace UICatalog {
	public partial class SearchShowResultsInSourceViewController : BaseSearchController {
		private UISearchController searchController;

		public SearchShowResultsInSourceViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Create the search controller, but we'll make sure that this SearchShowResultsInSourceViewController
			// performs the results updating.

			searchController = new UISearchController ((UIViewController) null);
			searchController.SetSearchResultsUpdater (UpdateSearchResultsForSearchController);
			searchController.DimsBackgroundDuringPresentation = false;

			// Make sure the that the search bar is visible within the navigation bar.
			searchController.SearchBar.SizeToFit ();

			// Include the search controller's search bar within the table's header view.
			TableView.TableHeaderView = searchController.SearchBar;
			DefinesPresentationContext = true;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (searchController != null) {
				searchController.Dispose ();
				searchController = null;
			}
		}
	}
}
