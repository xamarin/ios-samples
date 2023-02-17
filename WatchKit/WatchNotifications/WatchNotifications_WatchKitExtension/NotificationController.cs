using System;

using WatchKit;
using Foundation;

namespace WatchNotifications_iOSWatchKitExtension {
	public partial class NotificationController : WKUserNotificationInterfaceController {
		public NotificationController (IntPtr handle) : base (handle)
		{
		}


		public override void DidReceiveLocalNotification (UIKit.UILocalNotification localNotification, Action<WKUserNotificationInterfaceType> completionHandler)
		{
			base.DidReceiveLocalNotification (localNotification, completionHandler);

			Console.WriteLine ("DidReceiveLocalNotification alertbody:" + localNotification.AlertBody);
		}

		public override void DidReceiveRemoteNotification (NSDictionary remoteNotification, Action<WKUserNotificationInterfaceType> completionHandler)
		{
			base.DidReceiveRemoteNotification (remoteNotification, completionHandler);

			Console.WriteLine ("DidReceiveLocalNotification count:" + remoteNotification.Count);
		}


		public override void Awake (NSObject context)
		{
			base.Awake (context);

			// Configure interface objects here.
			Console.WriteLine ("{0} awake with context", this);
		}

		public override void WillActivate ()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine ("{0} did deactivate", this);
		}
	}
}

