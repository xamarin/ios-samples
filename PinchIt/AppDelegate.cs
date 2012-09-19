using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PinchIt
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			PinchLayout pinchLayout = new PinchLayout ();
			pinchLayout.ItemSize = new SizeF (100.0f, 100.0f);

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

