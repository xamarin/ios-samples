using System;

using WatchKit;
using Foundation;

namespace WatchNotifications_iOSWatchKitExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		public InterfaceController (IntPtr handle) : base (handle)
		{
		}


		public override void HandleLocalNotificationAction (string identifier, UIKit.UILocalNotification localNotification)
		{
			base.HandleLocalNotificationAction (identifier, localNotification);

			Console.WriteLine ("HandleLocalNotificationAction alertbody:" + localNotification.AlertBody);
		}

		public override void HandleRemoteNotificationAction (string identifier, NSDictionary remoteNotification)
		{
			base.HandleRemoteNotificationAction (identifier, remoteNotification);

			Console.WriteLine ("HandleRemoteNotificationAction count:"  + remoteNotification.Count);
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

