using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Transitioning_To_Xcode_4
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		Transitioning_To_Xcode_4ViewController viewController;
		
		// This method is invoked when the application has loaded its UI and is ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new Transitioning_To_Xcode_4ViewController ("Transitioning_To_Xcode_4ViewController", null);
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
