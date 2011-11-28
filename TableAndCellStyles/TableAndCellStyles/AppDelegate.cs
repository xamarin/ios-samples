using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Example_TableAndCellStyles
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region declarations and properties
		
		protected UIWindow window;
		protected UINavigationController navController;
		protected Example_TableAndCellStyles.Screens.iPhone.Home.HomeNavController iPhoneHome;
		
		#endregion
			
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			// create our navigation controller
			navController = new UINavigationController();
			
			// create the home screen and add it to the nav controller
			iPhoneHome = new Example_TableAndCellStyles.Screens.iPhone.Home.HomeNavController();
			navController.PushViewController(iPhoneHome, false);
			
			window.RootViewController = navController;
			
			//
			return true;
		}
			
	}
}
