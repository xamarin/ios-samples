using System;

using Foundation;
using UIKit;

namespace DispatchSourceExamples {
	public partial class MenuViewController : UITableViewController {
		public MenuViewController (IntPtr handle) : base (handle)
		{
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var dsViewController = (DispatchSourceViewController)Storyboard.InstantiateViewController ("DSViewController");
			dsViewController.SelectedDispatchSource = (DispatchSourceType)indexPath.Row;

			NavigationController.PushViewController (dsViewController, true);
		}
	}
}
