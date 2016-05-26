using Foundation;
using UIKit;

namespace MemoryDemo
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		MemoryDemoViewController viewController;
		UICollectionViewFlowLayout layout;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			layout = new UICollectionViewFlowLayout {
				SectionInset = new UIEdgeInsets (10f, 10f, 10f, 10f)
			};

			viewController = new MemoryDemoViewController (layout);

			window = new UIWindow(UIScreen.MainScreen.Bounds) {
				RootViewController = viewController
			};

			window.MakeKeyAndVisible ();

			return true;
		}
	}
}