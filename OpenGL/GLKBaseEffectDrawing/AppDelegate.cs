using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace GLKBaseEffectDrawing
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		MCViewController controller;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			controller = new MCViewController ();
			window.RootViewController = controller;

			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

