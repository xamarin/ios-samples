using Foundation;
using UIKit;

namespace Cloud {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var viewController = new KeyValueViewController ();

			Window = new UIWindow (UIScreen.MainScreen.Bounds) {
				BackgroundColor = UIColor.White,
				Bounds = UIScreen.MainScreen.Bounds
			};

			Window.RootViewController = viewController;
			Window.MakeKeyAndVisible ();

			return true;
		}
	}
}