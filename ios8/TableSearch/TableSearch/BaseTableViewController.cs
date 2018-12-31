using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// Base or common view controller to share a common UITableViewCell prototype between subclasses.
    /// </summary>
    public class BaseTableViewController : UITableViewController
    {
        protected const string tableViewCellIdentifier = "cellID";
        private const string nibName = "TableCell";

        public List<Product> filteredProducts = new List<Product>();
        private readonly NSNumberFormatter numberFormatter = new NSNumberFormatter();

        public BaseTableViewController(IntPtr handle) : base(handle) { }

        public BaseTableViewController() { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.numberFormatter.NumberStyle = NSNumberFormatterStyle.Currency;
            this.numberFormatter.FormatterBehavior = NSNumberFormatterBehavior.Default;


            var nib = UINib.FromName(BaseTableViewController.nibName, null);

            // Required if our subclasses are to use `dequeueReusableCellWithIdentifier(_:forIndexPath:)`.
            base.TableView.RegisterNibForCellReuse(nib, BaseTableViewController.tableViewCellIdentifier);
        }

        protected void ConfigureCell(UITableViewCell cell, Product product)
        {
            cell.TextLabel.Text = product.Title;

            // Build the price and year string.
            // Use NSNumberFormatter to get the currency format out of this NSNumber (product.introPrice).
            var priceString = this.numberFormatter.StringFromNumber(new NSNumber(product.IntroPrice));

            cell.DetailTextLabel.Text = $"{priceString} | {product.YearIntroduced}";
        }
    }
}