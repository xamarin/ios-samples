using System;
using MonoTouch.UIKit;

namespace MonoDevelopTouchCells
{

	public class TableViewDelegate : UITableViewDelegate
	{

		public TableViewDelegate ()
		{
		}
		
		public override void RowSelected (MonoTouch.UIKit.UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			CustomCell cell = tableView.CellAt(indexPath) as CustomCell;
			
			if (cell != null) {
				cell.CheckButtonTouchDown(null, null);
			}
			
			tableView.DeselectRow(indexPath, true);
		}

		public override void AccessoryButtonTapped (MonoTouch.UIKit.UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			CustomCell cell = tableView.CellAt(indexPath) as CustomCell;
			
			if (cell != null) {
				AppDelegate ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
				ad.ShowDetail(cell);
			}
		}

		
	}
}
