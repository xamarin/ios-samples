using Foundation;
using System;
using System.Linq;
using UIKit;

namespace UICatalog
{
    public partial class BaseSearchController : UITableViewController, IUISearchResultsUpdating
    {
        private const string CellIdentifier = "searchResultsCell";

        private readonly string[] allItems =
        {
            "Here's", "to", "the", "crazy", "ones.", "The", "misfits.", "The", "rebels.",
            "The", "troublemakers.", "The", "round", "pegs", "in", "the", "square", "holes.", "The", "ones", "who",
            "see", "things", "differently.", "They're", "not", "fond", "of", @"rules.", "And", "they", "have", "no",
            "respect", "for", "the", "status", "quo.", "You", "can", "quote", "them,", "disagree", "with", "them,",
            "glorify", "or", "vilify", "them.", "About", "the", "only", "thing", "you", "can't", "do", "is", "ignore",
            "them.", "Because", "they", "change", "things.", "They", "push", "the", "human", "race", "forward.",
            "And", "while", "some", "may", "see", "them", "as", "the", "crazy", "ones,", "we", "see", "genius.",
            "Because", "the", "people", "who", "are", "crazy", "enough", "to", "think", "they", "can", "change",
            "the", "world,", "are", "the", "ones", "who", "do."
        };

        private string[] items;

        private string query;

        public BaseSearchController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            items = allItems;
        }

        protected void ApplyFilter(string filter)
        {
            query = filter;
            items = string.IsNullOrEmpty(query) ? allItems : allItems.Where(s => s.Contains(query)).ToArray();

            TableView.ReloadData();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return items.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
            cell.TextLabel.Text = items[indexPath.Row];

            return cell;
        }

        #region IUISearchResultsUpdating

        [Export("updateSearchResultsForSearchController:")]
        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            // UpdateSearchResultsForSearchController is called when the controller is being dismissed
            // to allow those who are using the controller they are search as the results controller a chance to reset their state.
            // No need to update anything if we're being dismissed.
            if (searchController.Active)
            {
                ApplyFilter(searchController.SearchBar.Text);
            }
        }

        #endregion
    }
}