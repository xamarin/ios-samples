using System;
using UIKit;
using Foundation;

namespace Example_Touch
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected UINavigationController mainNavController;
		protected Example_Touch.Screens.iPhone.Home.Home_iPhone iPhoneHome;
		
		#endregion
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();

			// instantiate our main navigatin controller and add it's view to the window
			mainNavController = new UINavigationController ();
			
			iPhoneHome = new Example_Touch.Screens.iPhone.Home.Home_iPhone ();
			mainNavController.PushViewController (iPhoneHome, false);
				
			window.RootViewController = mainNavController;
			
			return true;
		}
	}
}
