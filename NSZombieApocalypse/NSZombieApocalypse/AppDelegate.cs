using UIKit;
using Foundation;

namespace NSZombieApocalypse
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		ViewController viewController;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			viewController = new ViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			return true;
		}

		public override bool AccessibilityPerformMagicTap ()
		{
			viewController.TogglePause (); // must be from the sample
			return true;
		}
	}
}