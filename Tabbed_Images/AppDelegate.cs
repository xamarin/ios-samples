using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Tabbed_Images
{
	/// <summary>
	/// The UIApplicationDelegate for the application. This class is responsible for launching the 
	/// User Interface of the application, as well as listening (and optionally responding) to 
	/// application events from iOS.
	/// </summary>
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UITabBarController tabBarController;
		
		/// <summary>
		/// This method is invoked when the application has loaded and is ready to run. In this 
		/// method you should instantiate the window, load the UI into it and then make the window
		/// visible.
		/// </summary>
		/// <remarks>
		/// You have 5 seconds to return from this method, or iOS will terminate your application.
		/// </remarks>
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			UIViewController viewController1, viewController2;
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				viewController1 = new FirstViewController ("FirstViewController_iPhone", null);
				viewController2 = new SecondViewController ("SecondViewController_iPhone", null);
			} else {
				viewController1 = new FirstViewController ("FirstViewController_iPad", null);
				viewController2 = new SecondViewController ("SecondViewController_iPad", null);
			}
			tabBarController = new UITabBarController ();
			tabBarController.ViewControllers = new UIViewController [] {
				viewController1,
				viewController2,
			};
			
			window.RootViewController = tabBarController;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
