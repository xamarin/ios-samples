using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.HealthKit;

namespace Fit
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public static HKHealthStore HealthStore { get; private set; }

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			HealthStore = new HKHealthStore ();
			SetupHealthStoreForTabBarControllers ();

			return true;
		}

		void SetupHealthStoreForTabBarControllers ()
		{
			var tabBarController = (UITabBarController)Window.RootViewController;
			foreach (UINavigationController navigationController in tabBarController.ViewControllers) {
				IHealthStore controller = navigationController.TopViewController as IHealthStore;
				if (controller != null)
					controller.HealthStore = HealthStore;
			}
		}
	}
}