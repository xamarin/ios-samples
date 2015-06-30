using System;
using UIKit;
using Foundation;

namespace BasicTable {
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		protected UIWindow window;
		protected HomeScreen iPhoneHome;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
						
			iPhoneHome = new HomeScreen();
			iPhoneHome.View.Frame = new CoreGraphics.CGRect(0
						, UIApplication.SharedApplication.StatusBarFrame.Height
						, UIScreen.MainScreen.ApplicationFrame.Width
						, UIScreen.MainScreen.ApplicationFrame.Height);
			
			window.AddSubview (iPhoneHome.View);
			
			return true;
		}
	}
}