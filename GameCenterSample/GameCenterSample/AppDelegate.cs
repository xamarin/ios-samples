using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.GameKit;

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
			
			viewController = new MainViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			if (!isGameCenterAPIAvailable ())
				gameCenterAuthenticationComplete = false;

			else {

				
				GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) => 
				{
					if(ui != null)
					{
						viewController.PresentViewController(ui,true,null);
					}
					else if(error != null)
					{
						new UIAlertView ("Error", error.ToString(), null, "OK", null).Show();
					}
					else if(GKLocalPlayer.LocalPlayer.Authenticated)
					{
						this.gameCenterAuthenticationComplete = true;

						//Switching Users
						if(currentPlayerID != null || currentPlayerID != GKLocalPlayer.LocalPlayer.PlayerID)
						{
							currentPlayerID = GKLocalPlayer.LocalPlayer.PlayerID;
							viewController.player = new PlayerModel();
							viewController.player.loadStoredScores();
							viewController.player.loadSotredAchievements();

						}
					}
					else
					{
						this.gameCenterAuthenticationComplete = false;
					}


				};
			}
			
			return true;
		}

		public override void DidEnterBackground (UIApplication application)
		{
			this.gameCenterAuthenticationComplete = false;
		}

		private bool isGameCenterAPIAvailable()
		{
			return UIDevice.CurrentDevice.CheckSystemVersion (4, 1);
		}
	}
}

