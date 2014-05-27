using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using GameKit;

namespace GameCenterSample
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		MainViewController viewController;
		bool gameCenterAuthenticationComplete = false;
		string currentPlayerID;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			if (!isGameCenterAPIAvailable ())
			{
				new UIAlertView ("Error", "Game Center is not supported on this device", null, "OK", null).Show();
				return true;
			}
			viewController = new MainViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			GKLocalPlayer.LocalPlayer.Authenticate (viewController.authenticatedHandler);

			return true;
		}

		public override void DidEnterBackground (UIApplication application)
		{
		}

		private bool isGameCenterAPIAvailable()
		{
			return UIDevice.CurrentDevice.CheckSystemVersion (4, 1);
		}
	}
}

