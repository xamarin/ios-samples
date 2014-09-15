using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace HelloGoodbye
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		StartViewController startViewController;
		UINavigationController navigationController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			startViewController = new StartViewController ();
			navigationController = new UINavigationController (startViewController);
			navigationController.NavigationBar.TintColor = StyleUtilities.ForegroundColor;

			window.RootViewController = navigationController;

			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

