/*
 * This controller handles displaying a custom or static notification.
*/

using System;

using Foundation;
using WatchKit;

namespace WatchkitExtension
{
	[Register ("NotificationController")]
	public class NotificationController : WKUserNotificationInterfaceController
	{
		public NotificationController (IntPtr handle) : base (handle)
		{
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		/*
		public override void DidReceiveLocalNotification (UIKit.UILocalNotification localNotification, Action<WKUserNotificationInterfaceType> completionHandler)
		{
			// This method is called when a local notification needs to be presented.
			// Implement it if you use a dynamic glance interface.
			// Populate your dynamic glance inteface as quickly as possible.
			//
			// After populating your dynamic glance interface call the completion block.
			completionHandler(WKUserNotificationInterfaceType.Custom);
		}
		*/

		public override void DidReceiveRemoteNotification (NSDictionary remoteNotification, Action<WKUserNotificationInterfaceType> completionHandler)
		{
			// This method is called when a remote notification needs to be presented.
			// Implement it if you use a dynamic glance interface.
			// Populate your dynamic glance inteface as quickly as possible.
			//
			// After populating your dynamic glance interface call the completion block.
			completionHandler (WKUserNotificationInterfaceType.Custom);

			// Use the following constant to display the static notification.
			//completionHandler(WKUserNotificationInterfaceType.Default);
		}
	}
}

