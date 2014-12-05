using System;
using Foundation;
using GameKit;
using UIKit;

namespace ButtonTapper3000 {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		public ViewController ViewController { get; private set; }
		public override UIWindow Window { get; set; }

		public static AppDelegate Shared {
			get {
				return (AppDelegate) UIApplication.SharedApplication.Delegate;
			}
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			ViewController = new ViewController ();

			var navigationController = new UINavigationController (ViewController) {
				NavigationBarHidden = true
			};

			GKLocalPlayer.LocalPlayer.AuthenticateHandler = (viewController, error) => {
				if (error != null) {
					Console.WriteLine ("Error while trying to authenticate local player: " + error.Description);
					return;
				}
				if (GKLocalPlayer.LocalPlayer.Authenticated || (viewController == null))
					return;
				navigationController.PresentViewController (viewController, true, null);
			};
		
			Window.RootViewController = navigationController;
			Window.MakeKeyAndVisible ();
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}