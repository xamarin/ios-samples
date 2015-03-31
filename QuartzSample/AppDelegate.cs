using UIKit;
using Foundation;

namespace QuartzSample
{
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);

			navigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			window.AddSubview (navigationController.View);

			window.MakeKeyAndVisible ();

			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

