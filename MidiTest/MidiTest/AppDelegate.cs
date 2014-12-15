using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;

namespace MidiTest
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			var label = new UILabel (new CGRect (20, 100, 300, 80)) {
				Text = "Playing back",
				TextColor = UIColor.Black
			};
			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = new UIViewController () { label },
				BackgroundColor = UIColor.White
			};
			window.MakeKeyAndVisible ();

			var audiotest = new AudioTest ();
			audiotest.MidiTest (label);
			return true;
		}

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

