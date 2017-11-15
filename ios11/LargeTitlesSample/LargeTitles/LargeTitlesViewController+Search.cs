using Foundation;
using System;
using UIKit;
using System.Linq;

namespace largetitles
{
    public partial class LargeTitlesViewController : UITableViewController, IUITableViewDelegate, IUITableViewDataSource, IUISearchResultsUpdating
    {
        string[] searchResults;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var search = new UISearchController(searchResultsController: null)
            {
                DimsBackgroundDuringPresentation = false
            };
            search.SearchResultsUpdater = this;
			// ensures the segue works in the context of the underling ViewController, thanks @artemkalinovsky
			DefinesPresentationContext = true;

            NavigationItem.SearchController = search;
        }

        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            var find = searchController.SearchBar.Text;
            if (!String.IsNullOrEmpty(find))
            {
                searchResults = titles.Where(t => t.ToLower().Contains(find.ToLower())).Select(p => p).ToArray();
            }
            else
            {
                searchResults = null;
            }
            TableView.ReloadData();
        }
    }
}