using System;
using Foundation;
using UIKit;

namespace CustomTransitionDemo
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		ControllerOne controllerOne;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			controllerOne = new ControllerOne ();

			window.RootViewController = controllerOne;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}