using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace CoreTelephonyDemo
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		CoreTelephonyDemoViewController viewController;
		UIWindow window;
		UINavigationController navigationController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow ();			
			navigationController = new UINavigationController ();
			viewController = new CoreTelephonyDemoViewController ();
			navigationController.PushViewController (viewController, true);
			window.AddSubview (navigationController.View);
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}

