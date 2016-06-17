using Foundation;
using UIKit;

namespace Newsstand {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		UIWindow window;
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				BackgroundColor = UIColor.White,
				Bounds = UIScreen.MainScreen.Bounds
			};

			window.RootViewController = new NewsstandViewController ();
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}