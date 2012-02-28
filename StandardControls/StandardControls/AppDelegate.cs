using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Example_StandardControls
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected UINavigationController mainNavController;
		protected Example_StandardControls.Screens.iPhone.Home.HomeNavController iPhoneHome;
		protected Example_StandardControls.Screens.iPad.Home.HomeNavController iPadHome;
		
		#endregion
				
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();

			// instantiate our main navigatin controller and add it's view to the window
			mainNavController = new UINavigationController ();
			
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				iPhoneHome = new Example_StandardControls.Screens.iPhone.Home.HomeNavController ();
				mainNavController.PushViewController (iPhoneHome, false);
			} else {
				iPadHome = new Example_StandardControls.Screens.iPad.Home.HomeNavController ();
				mainNavController.PushViewController (iPadHome, false);
			}
			
			window.RootViewController = mainNavController;
			
			//
			return true;
		}
				
	}
}
