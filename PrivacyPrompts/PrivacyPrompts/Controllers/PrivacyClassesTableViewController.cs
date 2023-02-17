using System;
using System.Collections.Generic;

using UIKit;
using Foundation;
using EventKit;
using Accounts;

namespace PrivacyPrompts {
	public partial class PrivacyClassesTableViewController : UITableViewController {
		const string LocationSegueId = "LocationSegue";
		const string MotionSegueId = "MotionSegue";
		const string DefaultSegueId = "DefaultSegue";

		List<DataClass> availableItems;

		DataClass currentSelection;

		public PrivacyClassesTableViewController (IntPtr handle)
			: base (handle)
		{
			availableItems = new List<DataClass> () {
				DataClass.Advertising,
				DataClass.AppleMusic,
				DataClass.Bluetooth,
				DataClass.Calendars,
				DataClass.Contacts,
				DataClass.Facebook,
				DataClass.Location,
				DataClass.Microphone,
				DataClass.Motion,
				DataClass.Photos,
				DataClass.Reminders,
				DataClass.SinaWeibo,
				DataClass.SpeechRecognition,
				DataClass.TencentWeibo,
				DataClass.Twitter,
				DataClass.Video,
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
			var vc = segue.DestinationViewController;
			vc.Title = currentSelection.ToString ();

			if (segue.Identifier == DefaultSegueId)
				Setup ((PrivacyDetailViewController) vc, currentSelection);
			else if (segue.Identifier == MotionSegueId)
				Setup ((MotionPrivacyController) vc);
			else if (segue.Identifier == LocationSegueId)
				Setup ((LocationPrivacyViewController) vc);
			else
				base.PrepareForSegue (segue, sender);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			currentSelection = GetDataClass (indexPath);
			string segueId = GetSegueIdFor (currentSelection);
			PerformSegue (segueId, this);
		}

		DataClass GetDataClass (NSIndexPath indexPath)
		{
			return availableItems [indexPath.Row];
		}

		string GetSegueIdFor (DataClass type)
		{
			switch (type) {
			case DataClass.Location:
				return LocationSegueId;

			case DataClass.Motion:
				return MotionSegueId;

			case DataClass.Advertising:
			case DataClass.AppleMusic:
			case DataClass.Bluetooth:
			case DataClass.Calendars:
			case DataClass.Contacts:
			case DataClass.Facebook:
			case DataClass.Microphone:
			case DataClass.Notifications:
			case DataClass.Photos:
			case DataClass.Reminders:
			case DataClass.SinaWeibo:
			case DataClass.SpeechRecognition:
			case DataClass.TencentWeibo:
			case DataClass.Twitter:
			case DataClass.Video:
				return DefaultSegueId;

			default:
				throw new NotImplementedException ();
			}
		}

		void Setup (PrivacyDetailViewController vc, DataClass type)
		{
			IPrivacyManager manager = null;
			switch (type) {
			case DataClass.Reminders:
				manager = new EKEntityPrivacyManager (EKEntityType.Reminder);
				break;

			case DataClass.Calendars:
				manager = new EKEntityPrivacyManager (EKEntityType.Event);
				break;

			case DataClass.Facebook:
				manager = new SocialNetworkPrivacyManager (ACAccountType.Facebook);
				break;

			case DataClass.Twitter:
				manager = new SocialNetworkPrivacyManager (ACAccountType.Twitter);
				break;

			case DataClass.SinaWeibo:
				manager = new SocialNetworkPrivacyManager (ACAccountType.SinaWeibo);
				break;

			case DataClass.TencentWeibo:
				manager = new SocialNetworkPrivacyManager (ACAccountType.TencentWeibo);
				break;

			case DataClass.Notifications:
				manager = new NotificationsPrivacyManager ((AppDelegate) UIApplication.SharedApplication.Delegate);
				break;

			case DataClass.Contacts:
				manager = new AddressBookPrivacyManager ();
				break;

			case DataClass.Photos:
				manager = new PhotoPrivacyManager ();
				break;

			case DataClass.Video:
				manager = new VideoCapturePrivacyManager ();
				break;

			case DataClass.Microphone:
				manager = new MicrophonePrivacyManager ();
				break;

			case DataClass.Bluetooth:
				manager = new BluetoothPrivacyManager ();
				break;

			case DataClass.Advertising:
				manager = new AdvertisingPrivacyManager ();
				break;

			case DataClass.AppleMusic:
				manager = new AppleMusicPrivacyManager ();
				break;

			case DataClass.SpeechRecognition:
				manager = new SpeechRecognitionPrivacyManager ();
				break;

			default:
				throw new NotImplementedException ();
			}

			vc.PrivacyManager = manager;
		}

		void Setup (MotionPrivacyController vc)
		{
			vc.PrivacyManager = new MotionPrivacyManager ();
		}

		void Setup (LocationPrivacyViewController vc)
		{
			vc.PrivacyManager = new LocationPrivacyManager ();
		}
	}
}
