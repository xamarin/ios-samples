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
		public NotificationsPrivacyController() : base(null, null)
		{
			CheckAccess = CheckNotificationsAccess;
			RequestAccess = RequestNotificationsAccess;

			/*
			 After the user interacts with the permissions dialog, AppDelegate.DidRegisterUserNotificationSettings
			 is called. In this example, we've hooked that up to an event
			*/
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			appDelegate.NotificationsRegistered += (_) => accessStatus.Text = CheckAccess ();


		}

		public string CheckNotificationsAccess()
		{
			var settings = UIApplication.SharedApplication.CurrentUserNotificationSettings;
			return "Allowed types: " + settings.Types.ToString ();
		}

		public void RequestNotificationsAccess()
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

			accessStatus.Lines = 0;

			accessStatus.RemoveConstraints (accessStatus.GetConstraintsAffectingLayout (UILayoutConstraintAxis.Vertical));
			accessStatus.AddConstraint (NSLayoutConstraint.Create (accessStatus, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 72));
			
		}
	}

}