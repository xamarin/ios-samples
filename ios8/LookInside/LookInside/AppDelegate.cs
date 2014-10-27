using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace LookInside
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		RootViewController rootViewController;
		UINavigationController navigationViewController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				BackgroundColor = UIColor.White
			};

			rootViewController = new RootViewController ();
			navigationViewController = new UINavigationController (rootViewController);

			window.RootViewController = navigationViewController;
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

