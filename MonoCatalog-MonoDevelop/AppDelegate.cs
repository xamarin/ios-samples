using UIKit;
using Foundation;

namespace MonoCatalog
{
		// The name AppDelegate is referenced in the MainWindow.xib file.
		public partial class AppDelegate : UIApplicationDelegate
		{
				// This method is invoked when the application is ready to run
				//
				public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
				{
						window.RootViewController = navigationController;
						window.MakeKeyAndVisible ();

						return true;
				}
		}
}

