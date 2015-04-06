using System;
using Foundation;
using UIKit;
using CoreGraphics;
using AddressBook;
using EventKit;
using AssetsLibrary;
using AVFoundation;
using CoreBluetooth;
using Accounts;
using AdSupport;
using CoreLocation;

namespace PrivacyPrompts
{
	[Register("PrivacyDetailViewController")]
	public partial class PrivacyDetailViewController : UIViewController, IPrivacyViewController
	{
		public UILabel TitleLabel {
			get {
				return titleLbl;
			}
		}

		public UILabel AccessStatus {
			get {
				return accessStatus;
			}
		}

		public UIButton RequestAccessButton {
			get {
				return requestBtn;
			}
		}

		// Dependency Injection via property
		public IPrivacyManager PrivacyManager { get; set; }

		public PrivacyDetailViewController(IntPtr handle)
			: base(handle)
		{
		}

		public PrivacyDetailViewController()
		{
			throw new InvalidProgramException ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RequestAccessButton.TouchUpInside += (s, e) => PrivacyManager.RequestAccess ();

			TitleLabel.Text = Title;
			AccessStatus.Text = "Indeterminate";
			RequestAccessButton.SetTitle ("Request access", UIControlState.Normal);

			AccessStatus.Text = PrivacyManager.CheckAccess ();
		}

		protected void UpdateStatus()
		{
			InvokeOnMainThread (() => AccessStatus.Text = PrivacyManager.CheckAccess ());
		}

		/*
		public static PrivacyDetailViewController CreateFor (DataClass selected)
		{
			PrivacyDetailViewController viewController = null;
			UIStoryboard storyboard = UIStoryboard.FromName ("MainStoryboard", null);

			switch (selected) {
			case DataClass.Location:
					viewController = (PrivacyDetailViewController)storyboard.InstantiateViewController ("DetailViewController");
//				viewController = new LocationPrivacyViewController ();
				break;
				case DataClass.Notifications:
					viewController = (PrivacyDetailViewController)storyboard.InstantiateViewController ("DetailViewController");
//					viewController = new NotificationsPrivacyController ();
				break;
			case DataClass.Calendars:
				viewController = new EKEntityPrivacyController (EKEntityType.Event);
				break;
			case DataClass.Reminders:
				viewController = new EKEntityPrivacyController (EKEntityType.Reminder);
				break;
			case DataClass.Contacts:
				viewController = new AddressBookPrivacyController ();
				break;
			case DataClass.Photos:
				viewController = new PhotoPrivacyController ();
				break;
			case DataClass.Video:
				viewController = new VideoCapturePrivacyController ();
				break;
			case DataClass.Microphone:
				viewController = new MicrophonePrivacyController ();
				break;
			case DataClass.Bluetooth:
				viewController = new BluetoothPrivacyController ();
				break;
			case DataClass.Motion:
				viewController = new MotionPrivacyController ();
				break;
			case DataClass.Facebook:
				viewController = new SocialNetworkPrivacyController (ACAccountType.Facebook);
				break;
			case DataClass.Twitter:
				viewController = new SocialNetworkPrivacyController (ACAccountType.Twitter);
				break;
			case DataClass.SinaWeibo:
				viewController = new SocialNetworkPrivacyController (ACAccountType.SinaWeibo);
				break;
			case DataClass.TencentWeibo:
				viewController = new SocialNetworkPrivacyController (ACAccountType.TencentWeibo);
				break;
			case DataClass.Advertising:
				viewController = new AdvertisingPrivacyController ();
				break;
			default:
				throw new ArgumentOutOfRangeException ();
			}
			viewController.Title = selected.ToString ();
			Console.WriteLine (viewController.Title);
			return viewController;
		}
		*/
	}
}
