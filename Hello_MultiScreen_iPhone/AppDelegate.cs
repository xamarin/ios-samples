using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Hello_MultiScreen_iPhone
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		//---- declarations
		UIWindow window;

		// This method is invoked when the application has loaded its UI and it is ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			this.window = new UIWindow (UIScreen.MainScreen.Bounds);

			//---- instantiate a new navigation controller
			var rootNavigationController = new UINavigationController();
			//---- instantiate a new home screen
			HomeScreen homeScreen = new HomeScreen();
			//---- add the home screen to the navigation controller (it'll be the top most screen)
			rootNavigationController.PushViewController(homeScreen, false);

			//---- set the root view controller on the window. the nav controller will handle the rest
			this.window.RootViewController = rootNavigationController;

			this.window.MakeKeyAndVisible ();

			return true;
		}
	}
}
