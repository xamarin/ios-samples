using System;
using Foundation;
using UIKit;

namespace SpriteTour {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			return true;
		}

		// class-level declarations
		public override UIWindow Window {
			get;
			set;
		}
	}
}
