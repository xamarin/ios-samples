using Foundation;
using System;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// The table view controller responsible for displaying the filtered products as the user types in the search field.
    /// </summary>
    public class ResultsTableController : BaseTableViewController
    {
        public ResultsTableController(IntPtr handle) : base(handle) { }

        public ResultsTableController() { }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return base.FilteredProducts.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(BaseTableViewController.TableViewCellIdentifier, indexPath);
            var product = base.FilteredProducts[indexPath.Row];
            base.ConfigureCell(cell, product);

            return cell;
        }
    }
}