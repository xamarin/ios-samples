using System;
using UIKit;
using Foundation;

namespace Example_Drawing
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected UINavigationController mainNavController;
		protected Example_Drawing.Screens.iPad.Home.HomeScreen iPadHome;
		
		#endregion
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();

			// instantiate our main navigatin controller and add it's view to the window
			mainNavController = new UINavigationController ();
			mainNavController.NavigationBar.Translucent = false;
			
			iPadHome = new Example_Drawing.Screens.iPad.Home.HomeScreen ();
			mainNavController.PushViewController (iPadHome, false);
			
			window.RootViewController = mainNavController;
			
			//
			return true;
		}
	}
}
