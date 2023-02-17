
namespace ARKitVision {
	using ARKit;
	using Foundation;
	using System;
	using UIKit;

	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			if (!ARConfiguration.IsSupported) {
				throw new Exception ("ARKit is not available on this device.For apps that require ARKit " +
									"for core functionality, use the `arkit` key in the key in the " +
									"`UIRequiredDeviceCapabilities` section of the Info.plist to prevent " +
									"the app from installing. (If the app can't be installed, this error " +
									"can't be triggered in a production scenario.)" +
									"In apps where AR is an additive feature, use `isSupported` to " +
									"determine whether to show UI for launching AR experiences.");
				// For details, see https://developer.apple.com/documentation/arkit
			}

			return true;
		}
	}
}
