using Foundation;
using UIKit;

namespace Appearance {
	/// <summary>
	/// Sizes the window according to the screen, for iPad as well as iPhone support
	/// </summary>
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		UIWindow window;
		UINavigationController navigationController;
		AppearanceViewController v;
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			v = new AppearanceViewController();
			navigationController = new UINavigationController();
			navigationController.PushViewController (v, false);

			
			UINavigationBar.Appearance.TintColor = UIColor.FromRGB(0, 127, 70); // light green


			window = new UIWindow (UIScreen.MainScreen.Bounds);	
			window.BackgroundColor = UIColor.FromRGB(240,240,240);
			window.Bounds = UIScreen.MainScreen.Bounds;
			window.AddSubview(navigationController.View);
            window.MakeKeyAndVisible ();
			return true;
		}
	}
}