using System;
using Foundation;
using UIKit;

namespace AirLocate
{
	public partial class MenuViewController : UITableViewController
	{
		UIViewController[] controllers = new UIViewController [4];

		public MenuViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			controllers [0] = (UIViewController)Storyboard.InstantiateViewController ("MonitoringViewController");
			controllers [1] = new RangingViewController (UITableViewStyle.Plain);
			controllers [2] = new CalibrationBeginViewController (UITableViewStyle.Plain);
			controllers [3] = (UIViewController)Storyboard.InstantiateViewController ("ConfigurationViewController");
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			NavigationController.PushViewController (controllers [indexPath.Row], true);
		}
	}
}
