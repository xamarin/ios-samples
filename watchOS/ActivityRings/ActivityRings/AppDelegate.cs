using System;

using Foundation;
using HealthKit;
using UIKit;

namespace ActivityRings
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		HKHealthStore healthStore = new HKHealthStore ();

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			return true;
		}

		public override void ShouldRequestHealthAuthorization(UIApplication application)
		{
			healthStore.HandleAuthorizationForExtension( (bool success, NSError error) => {
				if (error != null && !success)
					Console.WriteLine ($"You didn't allow HealthKit to access these read/write data types. In your app, try to handle this error gracefully when a user decides not to provide access. The error was: {error.LocalizedDescription}. If you're using a simulator, try it on a device.");
			});
		}
	}
}

