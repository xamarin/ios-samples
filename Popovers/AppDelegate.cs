using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Popovers
{
	[Register ("PopoversAppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
 		[Outlet]
		public UIWindow Window { get; set; }

		[Outlet]
		public UISplitViewController SplitViewController { get; set; }

		[Outlet]
		public RootViewController RootViewController { get; set; }

		[Outlet]
		public DetailViewController DetailViewController { get; set; }
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			SplitViewController.WillHideViewController += DetailViewController.WillHideViewController;
			SplitViewController.WillShowViewController += DetailViewController.WillShowViewController;
			SplitViewController.WillPresentViewController += DetailViewController.WillPresentViewController;

			Window.AddSubview (SplitViewController.View);
			Window.MakeKeyAndVisible ();
			return true;
		}
	}
}
