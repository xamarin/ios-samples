using System;

using Foundation;
using UIKit;

namespace Quotes
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UINavigationController viewController = new UINavigationController ();

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			// Create and push our master view controller
			var allTheQuotes = new MasterViewController (null, NSBundle.MainBundle);
			viewController.PushViewController (allTheQuotes, false);

			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}