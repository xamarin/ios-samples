using System;
using UIKit;

namespace GrowRowTable
{
	public class GrowRowTableDelegate : UITableViewDelegate
	{
		public GrowRowTableDelegate ()
		{
		}

		public override nfloat EstimatedHeight (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return 40f;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// Output selected row
			Console.WriteLine("Row selected: {0}",indexPath.Row);
		}
	}
}

