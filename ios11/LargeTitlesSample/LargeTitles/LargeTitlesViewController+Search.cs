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
            //TODO: enable search once I've figured out the segue losing the navbar
            //NavigationItem.SearchController = search;
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