using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace ManualStoryboard
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public override UIWindow Window {
			get;
			set;
		}

		public static UIStoryboard Storyboard = UIStoryboard.FromName ("MainStoryboard", null);
		public static UIViewController initialViewController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			initialViewController = Storyboard.InstantiateInitialViewController () as UIViewController;

			Window.RootViewController = initialViewController;
			Window.MakeKeyAndVisible ();
			return true;
		}

	}


}

