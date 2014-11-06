using System;
using CoreGraphics;
using UIKit;
using Foundation;

namespace TableEditing {
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		
		#region -= main =-

		public static void Main (string[] args)
		{
			try {
				UIApplication.Main (args, null, "AppDelegate");
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			}
		}

		#endregion

		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected TableEditing.Screens.HomeScreen iPhoneHome;
		
		#endregion
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			//---- create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
						
			//---- create the home screen
			iPhoneHome = new TableEditing.Screens.HomeScreen();
			iPhoneHome.View.Frame = new CGRect 
				(0, UIApplication.SharedApplication.StatusBarFrame.Height,
				 UIScreen.MainScreen.ApplicationFrame.Width,
				 UIScreen.MainScreen.ApplicationFrame.Height);
			
			window.AddSubview (iPhoneHome.View);
			
			return true;
		}
	}
}