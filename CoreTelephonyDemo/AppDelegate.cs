using Foundation;
using UIKit;

namespace CoreTelephonyDemo
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		CoreTelephonyDemoViewController viewController;
		UIWindow window;
		UINavigationController navigationController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow ();
			navigationController = new UINavigationController ();
			viewController = new CoreTelephonyDemoViewController ();
			navigationController.PushViewController (viewController, true);
			window.RootViewController = navigationController;
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}

