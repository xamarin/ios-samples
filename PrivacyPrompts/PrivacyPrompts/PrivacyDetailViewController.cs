using System;
using Foundation;
using UIKit;
using CoreGraphics;
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

	public class PrivacyDetailViewController : UIViewController
	{
		protected Func<String> CheckAccess;
		protected Action RequestAccess;

		protected UILabel titleLabel;
		protected UILabel accessStatus;
		protected UIButton requestAccessButton;

		static ACAccountStore accountStore;
		static CBCentralManager cbManager;

		public PrivacyDetailViewController ()
		{
		}

		public void AddBaseElements (UIView mainView)
		{
			titleLabel = new UILabel (CGRect.Empty);
			titleLabel.TextAlignment = UITextAlignment.Center;

			accessStatus = new UILabel (CGRect.Empty);
			accessStatus.TextAlignment = UITextAlignment.Center;

			requestAccessButton = UIButton.FromType (UIButtonType.RoundedRect);
			requestAccessButton.TouchUpInside += (s, e) => RequestAccess ();

			titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
			accessStatus.TranslatesAutoresizingMaskIntoConstraints = false;
			requestAccessButton.TranslatesAutoresizingMaskIntoConstraints = false;

			// View-level constraints to set constant size values
			titleLabel.AddConstraints (new [] {
				NSLayoutConstraint.Create (titleLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 14),
				NSLayoutConstraint.Create (titleLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 180),
			});

			accessStatus.AddConstraints (new[] {
				NSLayoutConstraint.Create (accessStatus, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 18),
				NSLayoutConstraint.Create (accessStatus, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 180),
			});

			requestAccessButton.AddConstraints (new[] {
				NSLayoutConstraint.Create (requestAccessButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 14),
				NSLayoutConstraint.Create (requestAccessButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 180),
			});
			mainView.AddSubview (titleLabel);
			mainView.AddSubview (accessStatus);
			mainView.AddSubview (requestAccessButton);

			// Container view-level constraints to set the position of each subview
			mainView.AddConstraints (new[] {
				NSLayoutConstraint.Create (titleLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (titleLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Top, 1, 80),

				NSLayoutConstraint.Create (accessStatus, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (accessStatus, NSLayoutAttribute.Top, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.Bottom, 1, 40),
				NSLayoutConstraint.Create (requestAccessButton, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (requestAccessButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, accessStatus, NSLayoutAttribute.Bottom, 1, 40),
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AddBaseElements (this.View);
			this.View.BackgroundColor = UIColor.White;

			titleLabel.Text = Title;
			accessStatus.Text = "Indeterminate";
			requestAccessButton.SetTitle ("Request access", UIControlState.Normal);

			accessStatus.Text = CheckAccess ();
		}

		protected void UpdateStatus()
		{
			InvokeOnMainThread (() => accessStatus.Text = CheckAccess ());
		}

		public static PrivacyDetailViewController CreateFor (DataClass selected)
		{
			PrivacyDetailViewController viewController = null;

			switch (selected) {
			case DataClass.Location:
				viewController = new LocationPrivacyViewController ();
				break;
			case DataClass.Notifications:
				viewController = new NotificationsPrivacyController ();
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
			return viewController;
		}
	}
}
