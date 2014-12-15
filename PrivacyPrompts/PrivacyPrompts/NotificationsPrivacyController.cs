using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoTouch.AVFoundation;
using MonoTouch.Accounts;
using MonoTouch.AdSupport;
using MonoTouch.AddressBook;
using MonoTouch.AssetsLibrary;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreLocation;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PrivacyPrompts
{

	public class NotificationsPrivacyController : PrivacyDetailViewController
	{
		public NotificationsPrivacyController()
		{
			CheckAccess = CheckNotificationsAccess;
			RequestAccess = RequestNotificationsAccess;

			/*
			 After the user interacts with the permissions dialog, AppDelegate.DidRegisterUserNotificationSettings
			 is called. In this example, we've hooked that up to an event
			*/
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			appDelegate.NotificationsRegistered += (_) => UpdateStatus();

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
