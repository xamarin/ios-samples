using System;
using UIKit;

namespace UICatalog
{
    public partial class SearchBarEmbeddedInNavigationBarViewController : BaseSearchController
    {
        private UISearchController searchController;

        public SearchBarEmbeddedInNavigationBarViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the search results view controller and use it for the UISearchController.
            //var searchResultsController = (SearchResultsViewController)Storyboard.InstantiateViewController(SearchResultsViewController.StoryboardIdentifier);

            // Create the search controller and make it perform the results updating.
            //var searchController = new UISearchController(searchResultsController);
            searchController = new UISearchController((UIViewController)null);
            searchController.SetSearchResultsUpdater(UpdateSearchResultsForSearchController);
            searchController.HidesNavigationBarDuringPresentation = false;

            // Configure the search controller's search bar. For more information on how to configure
            // search bars, see the "Search Bar" group under "Search".
            searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            searchController.SearchBar.ShowsCancelButton = false;
            searchController.SearchBar.Placeholder = "Search";

            // Include the search bar within the navigation bar.
            NavigationItem.TitleView = searchController.SearchBar;

            DefinesPresentationContext = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (searchController != null)
            {
                searchController.Dispose();
                searchController = null;
            }
        }
    }
}