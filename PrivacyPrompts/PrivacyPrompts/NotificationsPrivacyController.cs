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

	public class NotificationsPrivacyController : PrivacyDetailViewController
	{
		public NotificationsPrivacyController()
		{
			/*
			 After the user interacts with the permissions dialog, AppDelegate.DidRegisterUserNotificationSettings
			 is called. In this example, we've hooked that up to an event
			*/
			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			appDelegate.NotificationsRegistered += OnNotificationsRegistered;
		}

		void OnNotificationsRegistered (UIUserNotificationSettings obj)
		{
			UpdateStatus();
		}

		protected override string CheckAccess ()
		{
			var settings = UIApplication.SharedApplication.CurrentUserNotificationSettings;
			return string.Format ("Allowed types: {0}", settings.Types);
		}

		protected override void RequestAccess ()
		{
			var settings = UIUserNotificationSettings.GetSettingsForTypes (
				               UIUserNotificationType.Alert
				               | UIUserNotificationType.Badge
				               | UIUserNotificationType.Sound,
				               new NSSet ());
			//Asynch
			UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AccessStatus.Lines = 0;

			AccessStatus.RemoveConstraints (AccessStatus.GetConstraintsAffectingLayout (UILayoutConstraintAxis.Vertical));
			AccessStatus.AddConstraint (NSLayoutConstraint.Create (AccessStatus, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 72));

		}
	}

}
