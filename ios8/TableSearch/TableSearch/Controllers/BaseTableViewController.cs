using System;
using System.Collections.Generic;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// Base or common view controller to share a common UITableViewCell prototype between subclasses.
    /// </summary>
    public class BaseTableViewController : UITableViewController
    {
        protected const string TableViewCellIdentifier = "cellID";
        private const string CellNibName = "TableCell";

        public BaseTableViewController(IntPtr handle) : base(handle) { }

        public BaseTableViewController() { }

        public List<Product> FilteredProducts { get; set; } = new List<Product>();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            using (var nib = UINib.FromName(CellNibName, null))
            {
                // Required if our subclasses are to use `dequeueReusableCellWithIdentifier(_:forIndexPath:)`.
                base.TableView.RegisterNibForCellReuse(nib, TableViewCellIdentifier);
            }
        }

        protected void ConfigureCell(UITableViewCell cell, Product product)
        {
            cell.TextLabel.Text = product.Title;
            // Build the price and year string.
            cell.DetailTextLabel.Text = $"{product.IntroPrice.ToString("C")} | {product.YearIntroduced}";
        }
    }
}