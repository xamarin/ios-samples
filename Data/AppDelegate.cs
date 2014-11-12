using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using Foundation;

namespace Xamarin
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		protected UIWindow window;
		protected UINavigationController navController;
		protected Screens.NavTable.HomeNavController navTable;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
			
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			navController = new UINavigationController ();
			navTable = new Screens.NavTable.HomeNavController ();
			navController.PushViewController (navTable, false);
			
			// add the nav controller to the window
			window.RootViewController = navController;
			
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}
