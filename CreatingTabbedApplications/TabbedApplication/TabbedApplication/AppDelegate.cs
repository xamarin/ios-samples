using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace TabbedApplication
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		TabController tabController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			tabController = new TabController ();
			window.RootViewController = tabController;

			window.MakeKeyAndVisible ();
            
			return true;
		}
	}
}

