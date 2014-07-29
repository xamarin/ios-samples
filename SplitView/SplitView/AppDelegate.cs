using System;
using UIKit;
using Foundation;
using Example_SplitView.Screens;

namespace Example_SplitView
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		protected UIWindow window;
		protected Screens.MainSplitView.MainSplitView splitView;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			// instantiate our main split view controller
			splitView = new Screens.MainSplitView.MainSplitView ();
			
			window.RootViewController = splitView;
			
			//
			return true;
		}
	}
}
