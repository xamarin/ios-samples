using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CloudKit;

namespace CloudCaptions
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		[Outlet("tableController")]
		public MainViewController TableController { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// Override point for customization after application launch.
			UIUserNotificationSettings notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert, null);
			app.RegisterUserNotificationSettings (notificationSettings);;
			app.RegisterForRemoteNotifications ();

			return true;
		}

		public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			if (TableController == null)
				return;

			// Sends the ID of the record save that triggered the push to the tableViewController
			var recordInfo = (CKQueryNotification)CKQueryNotification.FromRemoteNotificationDictionary (userInfo);
			TableController.LoadNewPostsWithRecordID (recordInfo.RecordId);
		}
	}
}

