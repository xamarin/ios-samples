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
		List<DataClass> availableItems;

		public PrivacyClassesTableViewController (IntPtr handle)
			: base (handle)
		{
			availableItems = new List<DataClass> () {
				DataClass.Location,
				DataClass.Reminders,
				DataClass.Calendars,
				DataClass.Contacts,
				DataClass.Photos,
				DataClass.Video,
				DataClass.Microphone,
				DataClass.Bluetooth,
				DataClass.Motion,
				DataClass.Facebook,
				DataClass.Twitter,
				DataClass.SinaWeibo,
				DataClass.TencentWeibo,
				DataClass.Advertising,
			};

			// iOS8
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
				availableItems.Add (DataClass.Notifications);
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return availableItems.Count;
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

			int index = TableView.IndexPathForSelectedRow.Row;
			DataClass selected = availableItems[index];

			viewController = PrivacyDetailViewController.CreateFor (selected);
			viewController.Title = selected.ToString ();

			NavigationController.PushViewController (viewController, true);
		}
	}
}
