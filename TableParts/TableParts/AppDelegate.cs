using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Example_TableParts
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		protected UIWindow window;
		protected Example_TableParts.Screens.iPhone.Home.HomeScreen iPhoneHome;
			
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();

			// create the home screen and modify it's frame
			iPhoneHome = new Example_TableParts.Screens.iPhone.Home.HomeScreen();
			iPhoneHome.View.Frame = new System.Drawing.RectangleF(0, UIApplication.SharedApplication.StatusBarFrame.Height, UIScreen.MainScreen.ApplicationFrame.Width, UIScreen.MainScreen.ApplicationFrame.Height);
			
			window.RootViewController = iPhoneHome;
			
			//
			return true;
		}
			
	}
}
