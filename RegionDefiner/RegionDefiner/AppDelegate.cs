using System;
using Foundation;
using UIKit;

namespace RegionDefiner
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		RegionDefinerViewController viewController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new RegionDefinerViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			return true;
		}

		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
		{
			return UIInterfaceOrientationMask.Portrait;
		}
	}
}

