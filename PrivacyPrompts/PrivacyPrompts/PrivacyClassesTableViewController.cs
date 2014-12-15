using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoTouch.AVFoundation;
using MonoTouch.Accounts;
using MonoTouch.AdSupport;
using MonoTouch.AddressBook;
using MonoTouch.AssetsLibrary;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreLocation;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PrivacyPrompts {

	public partial class PrivacyClassesTableViewController : UITableViewController {
		public PrivacyClassesTableViewController (IntPtr handle) : base (handle)
		{
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return 1 + (int) DataClass.Advertising;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell ("BasicCell");
			cell.TextLabel.Text = ((DataClass) indexPath.Row).ToString ();
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			PrivacyDetailViewController viewController = null;

			DataClass selected = (DataClass)TableView.IndexPathForSelectedRow.Row;

			viewController = PrivacyDetailViewController.CreateFor (selected);
			viewController.Title = selected.ToString ();

			NavigationController.PushViewController (viewController, true);
		}
	}
}
