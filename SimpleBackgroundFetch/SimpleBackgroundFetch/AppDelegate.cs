using System;

using Foundation;
using UIKit;

namespace SimpleBackgroundFetch
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

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval (UIApplication.BackgroundFetchIntervalMinimum);
			return true;
		}

		public override void PerformFetch (UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			UINavigationController navigationController = Window.RootViewController as UINavigationController;
			UIViewController topViewController = navigationController.TopViewController;

			if (topViewController is RootViewController) {
				(topViewController as RootViewController).InsertNewObjectForFetch (completionHandler);
				UIApplication.SharedApplication.ApplicationIconBadgeNumber++;
			} else {
				completionHandler (UIBackgroundFetchResult.Failed);
			}
		}

		public override void WillEnterForeground (UIApplication application)
		{
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}
	}
}

