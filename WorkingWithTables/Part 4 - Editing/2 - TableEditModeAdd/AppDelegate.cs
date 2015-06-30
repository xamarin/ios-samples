using System;
using UIKit;
using Foundation;

namespace BasicTable {
	public class Application {
		public static void Main (string[] args)
		{
			try {
				UIApplication.Main (args, null, "AppDelegate");
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			}
		}
	}
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		HomeScreen iPhoneHome;
		UINavigationController navController; 

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
					
			iPhoneHome = new HomeScreen();
			iPhoneHome.Title = "Table Editing";
			iPhoneHome.View.Frame = new CoreGraphics.CGRect(0
						, UIApplication.SharedApplication.StatusBarFrame.Height
						, UIScreen.MainScreen.ApplicationFrame.Width
						, UIScreen.MainScreen.ApplicationFrame.Height);
			

			navController = new UINavigationController();
			navController.PushViewController (iPhoneHome, false);

			window.AddSubview (navController.View);
			
			return true;
		}
	}
}