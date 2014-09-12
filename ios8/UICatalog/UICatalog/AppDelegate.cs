using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			UISplitViewController splitViewController = (UISplitViewController)Window.RootViewController;

			splitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;

			return true;
		}
	}
}

