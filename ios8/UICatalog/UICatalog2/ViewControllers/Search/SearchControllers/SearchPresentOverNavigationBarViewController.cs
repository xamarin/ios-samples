using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("SearchPresentOverNavigationBarViewController")]
	public class SearchPresentOverNavigationBarViewController : SearchControllerBaseViewController
	{
		UISearchController searchController;

		public SearchPresentOverNavigationBarViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Action ("searchButtonClicked:")]
		public void SearchButtonClicked (UIBarButtonItem sender)
		{
			// Create the search results view controller and use it for the UISearchController.
			var searchResultsController = (SearchResultsViewController)Storyboard.InstantiateViewController (SearchResultsViewController.StoryboardIdentifier);

			// Create the search controller and make it perform the results updating.
			searchController = new UISearchController (searchResultsController);
			searchController.SetSearchResultsUpdater (searchResultsController.UpdateSearchResultsForSearchController);
			searchController.HidesNavigationBarDuringPresentation = false;

			// Present the view controller.
			PresentViewController (searchController, true, null);
		}
	}
}
