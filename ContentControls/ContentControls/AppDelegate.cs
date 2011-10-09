using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Example_ContentControls.Screens;

namespace Example_ContentControls
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected Screens.iPhone.Tabs.TabBarController iPhoneTabs;
		
		#endregion
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			// create the tab controller
			iPhoneTabs = new Screens.iPhone.Tabs.TabBarController();
			
			// load the tab controller onto the window
			window.RootViewController = iPhoneTabs;
			
			//
			return true;
		}
	}
}
