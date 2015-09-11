using Foundation;
using UIKit;

namespace FileSystem
{
	/// <summary>
	/// Sizes the window according to the screen, for iPad as well as iPhone support
	/// </summary>
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var viewController = new FileSystemViewController();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.BackgroundColor = UIColor.White;
			window.Bounds = UIScreen.MainScreen.Bounds;
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}
