using System;

using Foundation;
using UIKit;

namespace Stars
{
	[Register ("StarsAppDelegate")]
	public partial class StarsAppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			window.RootViewController = new StarsViewController();
			window.MakeKeyAndVisible ();
			
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "StarsAppDelegate");
		}
	}
}

