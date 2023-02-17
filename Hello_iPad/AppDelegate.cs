using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Hello_iPad {
	/// <summary>
	/// The UIApplicationDelegate for the application. This class is responsible for launching the
	/// User Interface of the application, as well as listening (and optionally responding) to
	/// application events from iOS.
	/// </summary>
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
		// class-level declarations
		UIWindow window;
		Hello_iPadViewController viewController;

		/// <summary>
		/// This method is invoked when the application has loaded and is ready to run. In this
		/// method you should instantiate the window, load the UI into it and then make the window
		/// visible.
		/// </summary>
		/// <remarks>
		/// You have 10 seconds to return from this method, or iOS will terminate your application.
		/// </remarks>
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			viewController = new Hello_iPadViewController ("Hello_iPadViewController", null);
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}
