using Foundation;
using System;
using UIKit;
using UserNotifications;

namespace RedGreenNotifications {
	public partial class ManageNotificationsViewController : UIViewController {
		public const string RedNotificationsEnabledKey = "redNotificationsEnabledKey";
		public const string GreenNotificationsEnabledKey = "greenNotificationsEnabledKey";
		public const string ShowManageNotificationsSegue = "ShowManageNotificationsSegue";

		NSObject observer;

		public ManageNotificationsViewController (IntPtr handle) : base (handle) { }

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			UpdateUserInterface ();
			if (observer == null) {
				observer = NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillEnterForegroundNotification, HandleEnterForegroundNotification);
			}
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			observer.Dispose ();
			observer = null;
		}

		void HandleEnterForegroundNotification (NSNotification notification)
		{
			UpdateUserInterface ();
		}

		partial void HandleRedNotificationsSwitchValueChange (UISwitch sender)
		{
			NSUserDefaults.StandardUserDefaults.SetBool (sender.On, RedNotificationsEnabledKey);
		}

		partial void HandleGreenNotificationsSwitchValueChange (UISwitch sender)
		{
			NSUserDefaults.StandardUserDefaults.SetBool (sender.On, GreenNotificationsEnabledKey);
		}

		async void UpdateUserInterface ()
		{
			UNNotificationSettings settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync ();
			BeginInvokeOnMainThread (() => {
				if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized || settings.AuthorizationStatus == UNAuthorizationStatus.Provisional) {
					RedNotificationsSwitch.Enabled = true;
					RedNotificationsSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey (RedNotificationsEnabledKey);
					RedNotificationsSwitch.Hidden = false;
					RedNotificationsLabel.Hidden = false;

					GreenNotificationsSwitch.Enabled = true;
					GreenNotificationsSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey (GreenNotificationsEnabledKey);
					GreenNotificationsSwitch.Hidden = false;
					GreenNotificationsLabel.Hidden = false;

					ManageNotificationsStatusLabel.Text = "";
				} else {
					RedNotificationsSwitch.Enabled = false;
					RedNotificationsSwitch.On = false;
					RedNotificationsSwitch.Hidden = true;
					RedNotificationsLabel.Hidden = true;

					GreenNotificationsSwitch.Enabled = false;
					GreenNotificationsSwitch.On = false;
					GreenNotificationsSwitch.Hidden = true;
					GreenNotificationsLabel.Hidden = true;

					ManageNotificationsStatusLabel.Text = "Enable notifications in Settings app.";
				}
			});
		}
	}
}
