using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace PinchIt
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				app.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);
				var frame = window.Frame;
				frame.Y = 20;
				window.Frame = frame;
			}

			PinchLayout pinchLayout = new PinchLayout ();
			pinchLayout.ItemSize = new CGSize (100.0f, 100.0f);

			window.RootViewController = (new ViewController (pinchLayout));
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}

	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

