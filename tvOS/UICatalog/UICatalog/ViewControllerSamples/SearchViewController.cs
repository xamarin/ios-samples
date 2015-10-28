using System;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class SearchViewController : UIViewController {

		[Export ("initWithCoder:")]
		public SearchViewController (NSCoder coder): base (coder)
		{
		}

		partial void ShowSearchController (NSObject sender)
		{
			var resultsController = Storyboard.InstantiateViewController (SearchResultsViewController.StoryboardIdentifier) as SearchResultsViewController;
			if (resultsController == null)
				throw new Exception ("Unable to instantiate a SearchResultsViewController.");

			var searchController = new UISearchController (resultsController) {
				SearchResultsUpdater = resultsController,
				HidesNavigationBarDuringPresentation = false
			};

			searchController.SearchBar.Placeholder = "Enter keyword (e.g. iceland)";
			SplitViewController?.PresentViewController (searchController, true, null);
		}
	}
}
