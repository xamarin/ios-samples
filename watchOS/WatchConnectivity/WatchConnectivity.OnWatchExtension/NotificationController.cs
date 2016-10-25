using System;

using WatchKit;
using Foundation;

namespace WatchConnectivity.OnWatchExtension
{
	public partial class NotificationController : WKUserNotificationInterfaceController
	{
		protected NotificationController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public NotificationController()
		{
			// Initialize variables here.
			// Configure interface objects here.
		}

		//		public override void DidReceiveLocalNotification (UIKit.UILocalNotification localNotification, Action<WKUserNotificationInterfaceType> completionHandler)
		//		{
		//			// This method is called when a local notification needs to be presented.
		//			// Implement it if you use a dynamic notification interface.
		//			// Populate your dynamic notification interface as quickly as possible.
		//			//
		//			// After populating your dynamic notification interface call the completion block.
		//			completionHandler.Invoke (WKUserNotificationInterfaceType.Custom);
		//		}
		//
		//		public override void DidReceiveRemoteNotification (NSDictionary remoteNotification, Action<WKUserNotificationInterfaceType> completionHandler)
		//		{
		//			// This method is called when a remote notification needs to be presented.
		//			// Implement it if you use a dynamic notification interface.
		//			// Populate your dynamic notification interface as quickly as possible.
		//			//
		//			// After populating your dynamic notification interface call the completion block.
		//			completionHandler.Invoke (WKUserNotificationInterfaceType.Custom);
		//		}

		public override void WillActivate()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine("{0} will activate", this);
		}

		public override void DidDeactivate()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine("{0} did deactivate", this);
		}
	}
}
