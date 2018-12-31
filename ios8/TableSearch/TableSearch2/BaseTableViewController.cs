using System;
using UIKit;
using Foundation;

namespace TableSearch
{
	public class BaseTableViewController : UITableViewController
	{
		protected const string cellIdentifier = "cellID";

		public BaseTableViewController ()
		{
		}

		public BaseTableViewController (IntPtr handle) : base (handle)
		{
		}

		protected void ConfigureCell (UITableViewCell cell, Product product)
		{
			cell.TextLabel.Text = product.Title;
			string detailedStr = string.Format ("{0:C} | {1}", product.IntroPrice, product.YearIntroduced);
			cell.DetailTextLabel.Text = detailedStr;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterNibForCellReuse (UINib.FromName ("TableCell", null), cellIdentifier);
		}
	}
}

