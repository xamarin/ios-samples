using Foundation;
using UIKit;
using HealthKit;
using System;


namespace ActivityRings
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window { get; set; }

		HKHealthStore healthStore = new HKHealthStore();

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif

			return true;
		}

		public override void ShouldRequestHealthAuthorization(UIApplication application)
		{

			healthStore.HandleAuthorizationForExtension( (bool success, NSError error) =>
			{
				if (error != null && !success)
				{
					Console.WriteLine($"You didn't allow HealthKit to access these read/write data types. In your app, try to handle this error gracefully when a user decides not to provide access. The error was: {error.LocalizedDescription}. If you're using a simulator, try it on a device.");
				}
			});

		}

	}
}

