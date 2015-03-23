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
	public abstract class PrivacyDetailViewController : UIViewController
	{
		protected UILabel TitleLabel { get; private set; }
		protected UILabel AccessStatus { get; private set; }
		protected UIButton RequestAccessButton { get; private set; }

		public void AddBaseElements (UIView mainView)
		{
			TitleLabel = new UILabel (CGRect.Empty);
			TitleLabel.TextAlignment = UITextAlignment.Center;

			AccessStatus = new UILabel (CGRect.Empty);
			AccessStatus.TextAlignment = UITextAlignment.Center;

			RequestAccessButton = UIButton.FromType (UIButtonType.RoundedRect);
			RequestAccessButton.TouchUpInside += (s, e) => RequestAccess ();

			TitleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
			AccessStatus.TranslatesAutoresizingMaskIntoConstraints = false;
			RequestAccessButton.TranslatesAutoresizingMaskIntoConstraints = false;

			// View-level constraints to set constant size values
			TitleLabel.AddConstraints (new [] {
				NSLayoutConstraint.Create (TitleLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 14),
				NSLayoutConstraint.Create (TitleLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 180),
			});

			AccessStatus.AddConstraints (new[] {
				NSLayoutConstraint.Create (AccessStatus, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 18),
				NSLayoutConstraint.Create (AccessStatus, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 180),
			});

			RequestAccessButton.AddConstraints (new[] {
				NSLayoutConstraint.Create (RequestAccessButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 14),
				NSLayoutConstraint.Create (RequestAccessButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 180),
			});
			mainView.AddSubview (TitleLabel);
			mainView.AddSubview (AccessStatus);
			mainView.AddSubview (RequestAccessButton);

			// Container view-level constraints to set the position of each subview
			mainView.AddConstraints (new[] {
				NSLayoutConstraint.Create (TitleLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (TitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Top, 1, 80),

				NSLayoutConstraint.Create (AccessStatus, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, TitleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (AccessStatus, NSLayoutAttribute.Top, NSLayoutRelation.Equal, TitleLabel, NSLayoutAttribute.Bottom, 1, 40),
				NSLayoutConstraint.Create (RequestAccessButton, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, TitleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (RequestAccessButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, AccessStatus, NSLayoutAttribute.Bottom, 1, 40),
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AddBaseElements (this.View);
			this.View.BackgroundColor = UIColor.White;

			TitleLabel.Text = Title;
			AccessStatus.Text = "Indeterminate";
			RequestAccessButton.SetTitle ("Request access", UIControlState.Normal);

			AccessStatus.Text = CheckAccess ();
		}

		protected void UpdateStatus()
		{
			InvokeOnMainThread (() => AccessStatus.Text = CheckAccess ());
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

		protected virtual void RequestAccess()
		{
		}

		protected abstract string CheckAccess();
	}
}
