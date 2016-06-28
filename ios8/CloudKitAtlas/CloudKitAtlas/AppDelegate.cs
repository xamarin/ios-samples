using System;

using UIKit;
using Foundation;
using CloudKit;

namespace CloudKitAtlas
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert, null);
			application.RegisterUserNotificationSettings (notificationSettings);
			application.RegisterForRemoteNotifications ();

			return true;
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			Console.WriteLine ("Registered for Push notifications with token: {0}", deviceToken);
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			Console.WriteLine ("Push subscription failed: {0}", error.LocalizedDescription);
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			var notification = CKNotification.FromRemoteNotificationDictionary (userInfo);

			var state = application.ApplicationState;

			var viewController = Window?.RootViewController as UINavigationController;
			if (viewController != null) {
				var tableViewController = viewController.ViewControllers [0] as MainMenuTableViewController;
				if (tableViewController != null) {
					var index = tableViewController.CodeSampleGroups.Length - 1;
					if (index > 0) {
						var notificationSample = tableViewController.CodeSampleGroups [index].CodeSamples [0] as MarkNotificationsReadSample;
						notificationSample.Cache.AddNotification (notification);
						tableViewController.TableView.ReloadRows (new NSIndexPath [] { NSIndexPath.FromRowSection (index, 0) }, UITableViewRowAnimation.Automatic);
					}
				}

				if (state == UIApplicationState.Active) {
					var navigationBar = viewController.NavigationBar as NavigationBar;
					if (navigationBar != null)
						navigationBar.ShowNotificationAlert (notification);
				}
			}
		}
	}
}
