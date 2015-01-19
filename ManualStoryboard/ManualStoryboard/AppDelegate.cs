using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace ManualStoryboard
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		public static UIStoryboard Storyboard = UIStoryboard.FromName ("MainStoryboard", null);
		public static UIViewController initialViewController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			initialViewController = Storyboard.InstantiateInitialViewController () as UIViewController;

			window.RootViewController = initialViewController;
			window.MakeKeyAndVisible ();
			return true;
		}

	}

}

