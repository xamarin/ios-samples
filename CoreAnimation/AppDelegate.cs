using System;
using UIKit;
using Foundation;

namespace Example_CoreAnimation
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		 
		#region  declarations and properties 
		
		protected UIWindow window;
		protected Screens.iPad.Home.MainSplitView splitView;
		
		#endregion
		 
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			// instantiate our main split view controller
			splitView = new Screens.iPad.Home.MainSplitView ();
			
			window.RootViewController = splitView;
			return true;
		}
	}
}
