using Foundation;
using UIKit;

namespace MediaCapture {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		UIWindow window;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = new RootViewController ();
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}

