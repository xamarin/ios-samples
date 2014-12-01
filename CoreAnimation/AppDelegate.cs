using System;
using UIKit;
using Foundation;

namespace CoreAnimationExample
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		MainSplitView splitView;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();

			// instantiate our main split view controller
			splitView = new MainSplitView ();

			window.RootViewController = splitView;
			return true;
		}
	}
}
