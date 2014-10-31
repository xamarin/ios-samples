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

namespace PrivacyPrompts {

	public partial class PrivacyClassesTableViewController : UITableViewController {

		public enum DataClass {
			Location,
			Notifications,
			Calendars,
			Contacts,
			Photos,
			Video,
			Reminders,
			Microphone,
			Bluetooth,
			Motion,
			Facebook,
			Twitter,
			SinaWeibo,
			TencentWeibo,
			Advertising
		}

		CLLocationManager locationManager;
		ACAccountStore accountStore;
		EKEventStore eventStore;
		ALAssetsLibrary assetLibrary;
		CBCentralManager cbManager;
		ABAddressBook addressBook;

		public PrivacyClassesTableViewController (IntPtr handle) : base (handle)
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
			switch (selected) {
			case DataClass.Location:
				viewController = new LocationPrivacyViewController ();
				break;
			case DataClass.Contacts:
				viewController = new PrivacyDetailViewController (
					CheckAddressBookAccess, 
					RequestAddressBookAccess
				);
				break;
			case DataClass.Calendars:
				viewController = new PrivacyDetailViewController(
					() => CheckEventStoreAccess (EKEntityType.Event),
					() => RequestEventStoreAccess (EKEntityType.Event)
				);
				break;
			case DataClass.Reminders:
				viewController = new PrivacyDetailViewController (
					() => CheckEventStoreAccess (EKEntityType.Reminder),
					() => RequestEventStoreAccess (EKEntityType.Reminder)
				);
				break;
			case DataClass.Photos:
				viewController = new PrivacyDetailViewController (
					CheckPhotosAuthorizationStatus,
					() => { 
						RequestPhotoAccess(false);
						RequestPhotoAccess(true);
					}
				);
				break;
			case DataClass.Microphone:
				viewController = new PrivacyDetailViewController (
					() => { return "Not determined"; },
					() => {
						RequestMicrophoneAccess (true);
						RequestMicrophoneAccess (false);
					}
				);
				break;
			case DataClass.Bluetooth:
				viewController = new PrivacyDetailViewController (
					CheckBluetoothAccess, 
					RequestBluetoothAccess
				);
				break;
			case DataClass.Facebook:
				viewController = new PrivacyDetailViewController (
					() => CheckSocialAccountAuthorizationStatus (ACAccountType.Facebook),
					RequestFacebookAccess
				);
				break;
			case DataClass.Twitter:
				viewController = new PrivacyDetailViewController (
					() => CheckSocialAccountAuthorizationStatus (ACAccountType.Twitter),
					RequestTwitterAccess
				);
				break;
			case DataClass.SinaWeibo:
				viewController = new PrivacyDetailViewController (
					() => CheckSocialAccountAuthorizationStatus (ACAccountType.SinaWeibo),
					RequestSinaWeiboAccess
				);
				break;
			case DataClass.TencentWeibo:
				viewController = new PrivacyDetailViewController (
					() => CheckSocialAccountAuthorizationStatus (ACAccountType.TencentWeibo),
					RequestTencentWeiboAccess
				);
				break;
			case DataClass.Advertising:
				viewController = new PrivacyDetailViewController (
					AdvertisingIdentifierStatus,
					() => {
					}
				);
				break;
			case DataClass.Video:
				viewController = new VideoCaptureViewController ();
				break;
			case DataClass.Motion:
				viewController = new MotionPrivacyController ();
				break;
			case DataClass.Notifications:
				viewController = new NotificationsPrivacyController ();
				break;
		 	default:
				throw new ArgumentOutOfRangeException();
			}
			viewController.Title = selected.ToString ();

			NavigationController.PushViewController (viewController, true);
		}


		#region Contacts methods

		public string CheckAddressBookAccess ()
		{
			return ABAddressBook.GetAuthorizationStatus ().ToString ();
		}

		public void RequestAddressBookAccess ()
		{
			NSError error;
			addressBook = ABAddressBook.Create (out error);

			if (addressBook != null) {
				addressBook.RequestAccess (delegate (bool granted, NSError accessError) {
					ShowAlert (DataClass.Contacts, granted ? "granted" : "denied");
				});
			}
		}

		#endregion

		#region EventStore methods

		public string CheckEventStoreAccess (EKEntityType type)
		{
			return EKEventStore.GetAuthorizationStatus (type).ToString();
		}

		public void RequestEventStoreAccess (EKEntityType type)
		{
			if (eventStore == null)
				eventStore = new EKEventStore ();

			eventStore.RequestAccess (type, delegate (bool granted, NSError error) {
				ShowAlert (type == EKEntityType.Event ? DataClass.Calendars : DataClass.Reminders,
				           granted ? "granted" : "denied");
			});
		}

		#endregion

		#region Photos methods

		public string CheckPhotosAuthorizationStatus ()
		{
			return ALAssetsLibrary.AuthorizationStatus.ToString ();
		}

		public void RequestPhotoAccess (bool useImagePicker)
		{
			if (useImagePicker) {
				UIImagePickerController picker = new UIImagePickerController ();
				picker.Delegate = this;
				PresentViewController (picker, true, null);
			} else {
				if (assetLibrary == null) 
					assetLibrary = new ALAssetsLibrary ();

				assetLibrary.Enumerate (ALAssetsGroupType.All,
				                        delegate { }, delegate { });
			}
		}

		#endregion

		#region Microphone methods

		public void RequestMicrophoneAccess (bool usePermissionAPI)
		{
			AVAudioSession audioSession = AVAudioSession.SharedInstance();
			if (!usePermissionAPI) {
				NSError error;
				audioSession.SetCategory (AVAudioSession.CategoryRecord, out error);
			} else {
				audioSession.RequestRecordPermission (delegate(bool granted) {
					ShowAlert (DataClass.Microphone, granted ? "granted" : "denied");
				});
			}
		}

		#endregion

		#region Bluetooth methods

		public string CheckBluetoothAccess ()
		{
			if (cbManager == null)
				cbManager = new CBCentralManager ();

			CBCentralManagerState state = cbManager.State;
			return state.ToString ();
		}

		public void RequestBluetoothAccess ()
		{
			if (cbManager == null)
				cbManager = new CBCentralManager ();

			if (cbManager.State == CBCentralManagerState.PoweredOn)
				cbManager.ScanForPeripherals (new CBUUID [0]);
			else {
				UIAlertView alert = new UIAlertView ("Error", "Bluetooth must be enabled",
				                                     null, "Okay", null);
				alert.Show ();
			}
		}

		#endregion

		#region Social methods

		public string CheckSocialAccountAuthorizationStatus (NSString accountTypeIdentifier)
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType socialAccount = accountStore.FindAccountType (accountTypeIdentifier);

			DataClass dataClass;

			if (accountTypeIdentifier == ACAccountType.Facebook)
				dataClass = DataClass.Facebook;
			else if (accountTypeIdentifier == ACAccountType.Twitter)
				dataClass = DataClass.Twitter;
			else if (accountTypeIdentifier == ACAccountType.SinaWeibo)
				dataClass = DataClass.SinaWeibo;
			else
				dataClass = DataClass.TencentWeibo;

			return socialAccount.AccessGranted ? "granted" : "denied";
		}

		public void RequestFacebookAccess ()
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType facebookAccount = accountStore.FindAccountType (ACAccountType.Facebook);

			AccountStoreOptions options = new AccountStoreOptions () { FacebookAppId = "MY_CODE" };
			options.SetPermissions (ACFacebookAudience.Friends, new [] { "email", "user_about_me" });

			accountStore.RequestAccess (facebookAccount, options, delegate (bool granted, NSError error) {
				ShowAlert (DataClass.Facebook, granted ? "granted" : "denied");
			});
		}

		public void RequestTwitterAccess ()
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType twitterAccount = accountStore.FindAccountType (ACAccountType.Twitter);

			accountStore.RequestAccess (twitterAccount, null, delegate (bool granted, NSError error) {
				ShowAlert (DataClass.Twitter, granted ? "granted" : "denied");
			});
		}

		public void RequestSinaWeiboAccess ()
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType sinaWeiboAccount = accountStore.FindAccountType (ACAccountType.SinaWeibo);

			accountStore.RequestAccess (sinaWeiboAccount, null, delegate(bool granted, NSError error) {
				ShowAlert (DataClass.SinaWeibo, granted ? "granted" : "denied");
			});
		}

		public void RequestTencentWeiboAccess ()
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType tencentWeiboAccount = accountStore.FindAccountType (ACAccountType.TencentWeibo);

			AccountStoreOptions options = new AccountStoreOptions ();
			options.TencentWeiboAppId = "MY_ID";

			accountStore.RequestAccess (tencentWeiboAccount, options, delegate (bool granted, NSError error) {
				ShowAlert (DataClass.TencentWeibo, granted ? "granted" : "denied");
			});
		}

		#endregion

		#region Advertising

		public string AdvertisingIdentifierStatus ()
		{
			return 
			           ASIdentifierManager.SharedManager.IsAdvertisingTrackingEnabled ?
			           "granted" : "denied";
		}

		#endregion

		void ShowAlert (DataClass dataClass, string status)
		{
			string message = String.Format ("Access to {0} is {1}.", dataClass, status);
			InvokeOnMainThread (delegate {
				UIAlertView alert = new UIAlertView ("Request Status", message, null, "OK", null);
				alert.Show ();
			});
		}
	}
}