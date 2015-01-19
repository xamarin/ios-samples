using System;
using UIKit;
using Foundation;

namespace MonoDevelopTouchCells
{

	public class TableViewDelegate : UITableViewDelegate
	{
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			CustomCell cell = tableView.CellAt (indexPath) as CustomCell;

			if (cell != null) {
				cell.CheckButtonTouchDown (null, null);
			}

			tableView.DeselectRow (indexPath, true);
		}

		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			CustomCell cell = tableView.CellAt (indexPath) as CustomCell;

			if (cell != null) {
				AppDelegate ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
				ad.ShowDetail (cell);
			}
		}
	}
}
