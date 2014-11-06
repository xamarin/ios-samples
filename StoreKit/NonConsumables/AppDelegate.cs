using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace NonConsumables {
	public class Application {
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
		UIWindow window;
		UINavigationController navigationController;
		UIViewController viewController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new NonConsumableViewController();
			
			navigationController = new UINavigationController();
			navigationController.PushViewController (viewController, false);

			// If you have defined a view, add it here:
			//window.AddSubview (navigationController.View);
			window.RootViewController = navigationController;
			
			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}