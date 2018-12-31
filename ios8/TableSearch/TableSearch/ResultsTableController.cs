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
            return base.filteredProducts.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(BaseTableViewController.tableViewCellIdentifier, indexPath);
            var product = base.filteredProducts[indexPath.Row];
            base.ConfigureCell(cell, product);


            return cell;
        }
    }
}