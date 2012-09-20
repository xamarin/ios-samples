using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace PassLibrary {
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
		protected UIWindow window;
		UINavigationController navigation;
		HomeScreen iPhoneHome;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
						
			iPhoneHome = new HomeScreen();
			iPhoneHome.Title = "Pass Library";
			iPhoneHome.View.Frame = new System.Drawing.RectangleF(0
						, UIApplication.SharedApplication.StatusBarFrame.Height
						, UIScreen.MainScreen.ApplicationFrame.Width
						, UIScreen.MainScreen.ApplicationFrame.Height);

			navigation = new UINavigationController();
			navigation.PushViewController (iPhoneHome,false);
			window.RootViewController = navigation;

			
			return true;
		}
	}
}