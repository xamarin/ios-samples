using System;
using UIKit;
using Foundation;

namespace AVCam
{
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications ();
			return true;
		}

		public override void WillTerminate (UIApplication application)
		{
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications ();
		}

		public override void WillEnterForeground (UIApplication application)
		{
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications ();
		}

		public override void DidEnterBackground (UIApplication application)
		{
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications ();
		}
	}
}