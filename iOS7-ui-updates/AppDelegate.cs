using Foundation;
using UIKit;

namespace ImageView {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		UIWindow window;
		UINavigationController navigationController;
		UIViewController viewController;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			viewController = new ImageViewController ();

			navigationController = new UINavigationController ();
			navigationController.PushViewController (viewController, false);

			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = navigationController
			};

			window.AddSubview (navigationController.View);
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

