using System;
using UIKit;

namespace GrowRowTable
{
	public class GrowRowTableDelegate : UITableViewDelegate
	{
		public GrowRowTableDelegate ()
		{
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// Output selected row
			Console.WriteLine("Row selected: {0}",indexPath.Row);
		}
	}
}

