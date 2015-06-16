using UIKit;
using Foundation;

namespace avTouch
{
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class avTouchAppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

