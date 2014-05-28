using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Hello_ComplexUniversal
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
		UIViewController homeScreen;
		
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
			
			//---- by default, the universal template uses a shared .xib for a single controller
			// however, this rarely works in the real world, because often times the UX is significantly
			// different between the iPad version and the iPhone version, thus requiring different 
			// controllers altogether.
			//
			// in this sample, we load a completely different controller, depending on the device:
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				homeScreen = new Screens.HomeScreen_iPhone();
			} else {
				homeScreen = new Screens.HomeScreen_iPad();
			}
			window.RootViewController = homeScreen;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
