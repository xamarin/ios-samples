using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using Accounts;
using AdSupport;
using AddressBook;
using AssetsLibrary;
using CoreBluetooth;
using CoreLocation;
using EventKit;
using Foundation;
using UIKit;

namespace PrivacyPrompts
{
	public partial class PrivacyClassesTableViewController : UITableViewController
	{
		public PrivacyClassesTableViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override nint RowsInSection (UITableView tableview, nint section)
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
