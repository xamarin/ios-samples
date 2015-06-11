using System;
using CoreLocation;
using Foundation;
using UIKit;

namespace AirLocate
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		CLLocationManager locationManager;

		public override UIWindow Window {
			get;
			set;
		}

		public override void FinishedLaunching (UIApplication application)
		{
			locationManager = new CLLocationManager ();

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				locationManager.RequestWhenInUseAuthorization ();
			}
			// A user can transition in or out of a region while the application is not running.
			// When this happens CoreLocation will launch the application momentarily, call this delegate method
			// and we will let the user know via a local notification.
			locationManager.DidDetermineState += (sender, e) => {
				string body = null;
				if (e.State == CLRegionState.Inside)
					body = "You're inside the region";
				else if (e.State == CLRegionState.Outside)
					body = "You're outside the region";

				if (body != null) {
					var notification = new UILocalNotification () { AlertBody = body };

					// If the application is in the foreground, it will get called back to ReceivedLocalNotification
					// If its not, iOS will display the notification to the user.
					UIApplication.SharedApplication.PresentLocalNotificationNow (notification);
				}
			};
		}

		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
		{
			// If the application is in the foreground, we will notify the user of the region's state via an alert.
			new UIAlertView (notification.AlertBody, String.Empty, null, "OK", null).Show ();
		}

		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

