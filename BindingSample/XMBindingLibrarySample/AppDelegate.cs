using Foundation;
using UIKit;

namespace XMBindingLibrarySample {
	[Register (nameof (AppDelegate))]
	public partial class AppDelegate : UIApplicationDelegate {
		RootViewController rootViewController;

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds);

			rootViewController = new RootViewController ();

			Window.RootViewController = rootViewController;
			Window.MakeKeyAndVisible ();

			return true;
		}
	}
}
