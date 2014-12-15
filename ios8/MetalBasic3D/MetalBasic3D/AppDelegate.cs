using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace MetalBasic3D
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window {
			get;
			set;
		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}

		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}

		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			GameViewController controller = (GameViewController)Window.RootViewController;

			if (controller == null) {
				Console.WriteLine ("ERROR: Failed creating a view controller!");
				return false;
			}

			var renderer = new Renderer ();
			controller.Delegate = renderer;

			GameView renderView = (GameView)controller.View;

			if (renderView == null) {
				Console.WriteLine ("ERROR: Failed creating a renderer view!");
				return false;
			}

			renderView.Delegate = renderer;

			// load all renderer assets before starting game loop
			renderer.Configure (renderView);

			// run the game loop
			controller.DispatchGameLoop ();
			return true;
		}
	}
}

