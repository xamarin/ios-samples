using System;
using Foundation;
using UIKit;

namespace SoZoomy {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			Window.RootViewController = new ViewController ();
			Window.MakeKeyAndVisible ();
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}