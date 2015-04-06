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
		const string LocationSegueId = "LocationSegue";
		const string DefaultSegueId = "DefaultSegue";

		List<DataClass> availableItems;

		DataClass currentSelection;

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

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var destinationVC = segue.DestinationViewController;
			destinationVC.Title = currentSelection.ToString ();

			var privacyVC = (PrivacyDetailViewController)destinationVC;
			privacyVC.PrivacyManager = GetPrivacyManagerFor (currentSelection);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			currentSelection = GetDataClass (indexPath);
			string segueId = GetSegueIdFor (currentSelection);
			PerformSegue (segueId, this);
		}

		DataClass GetDataClass(NSIndexPath indexPath)
		{
			return availableItems [indexPath.Row];
		}

		string GetSegueIdFor(DataClass type)
		{
			switch (type) {
				case DataClass.Location:
					return LocationSegueId;

					// TODO: this is another segue
				case DataClass.Motion:
					return DefaultSegueId;

				case DataClass.Notifications:
				case DataClass.Calendars:
				case DataClass.Reminders:
				case DataClass.Contacts:
				case DataClass.Photos:
				case DataClass.Video:
				case DataClass.Microphone:
				case DataClass.Bluetooth:
				case DataClass.Facebook:
				case DataClass.Twitter:
				case DataClass.SinaWeibo:
				case DataClass.TencentWeibo:
				case DataClass.Advertising:
					return DefaultSegueId;

				default:
					throw new NotImplementedException ();
			}
		}

		IPrivacyManager GetPrivacyManagerFor(DataClass type)
		{
			switch (type) {
				case DataClass.Location:
					throw new NotImplementedException ();

					// TODO: this is another segue
				case DataClass.Motion:
					throw new NotImplementedException ();

				case DataClass.Reminders:
					return new EKEntityPrivacyManager (EKEntityType.Reminder);

				case DataClass.Calendars:
					return new EKEntityPrivacyManager (EKEntityType.Event);

				case DataClass.Facebook:
					return new SocialNetworkPrivacyManager (ACAccountType.Facebook);

				case DataClass.Twitter:
					return new SocialNetworkPrivacyManager (ACAccountType.Twitter);

				case DataClass.SinaWeibo:
					return new SocialNetworkPrivacyManager (ACAccountType.SinaWeibo);

				case DataClass.TencentWeibo:
					return new SocialNetworkPrivacyManager (ACAccountType.TencentWeibo);

				case DataClass.Notifications:
					return new NotificationsPrivacyManager ((AppDelegate)UIApplication.SharedApplication.Delegate);

				case DataClass.Contacts:
					return new AddressBookPrivacyManager ();

				case DataClass.Photos:
				case DataClass.Video:
				case DataClass.Microphone:
				case DataClass.Bluetooth:
				case DataClass.Advertising:
					throw new NotImplementedException ();

				default:
					throw new NotImplementedException ();
			}
		}
	}
}
