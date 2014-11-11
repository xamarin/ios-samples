using System;
using UIKit;
using Foundation;

namespace Example_CoreLocation
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected MainViewController mainViewController;

		#endregion
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			// instantiate and load our main screen onto the window
			mainViewController = new MainViewController ();
			window.RootViewController = mainViewController;
			
			//
			return true;
		}
	}
}
