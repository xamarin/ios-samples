using System;
using System.Collections.Generic;
using System.Linq;
using CloudKit;
using Foundation;
using UIKit;

namespace CloudKitAtlas
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
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
					var group
					var index = tableViewController.CodeSampleGroups.Length - 1;
					var notificationSample = tableViewController.CodeSampleGroups.LastOrDefault ()?.CodeSamples.First () as MarkNotificationsReadSample;
					notificationSample
				}

			}
			//		if let tableViewController = viewController.viewControllers [0] as? MainMenuTableViewController {
			//			let index = tableViewController.codeSampleGroups.count - 1
	 
			//		if let notificationSample = tableViewController.codeSampleGroups.last?.codeSamples.first as? MarkNotificationsReadSample {
			//				notificationSample.cache.addNotification (notification)

			//			tableViewController.tableView.reloadRowsAtIndexPaths ([NSIndexPath (forRow: index, inSection: 0)], withRowAnimation: .Automatic)
   //                 }
			//		}
			//		if state == .Active {
			//			if let navigationBar = viewController.navigationBar as? NavigationBar {
			//				navigationBar.showNotificationAlert (notification)
	
			//		}
			//		}

			//	}

		}
	}
}
