
namespace SpeedySloth {
	using CoreLocation;
	using Foundation;
	using HealthKit;
	using System;
	using UIKit;

	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		private readonly CLLocationManager locationManager = new CLLocationManager ();

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			this.RequestAccessToHealthKit ();
			this.RequestLocationServices ();

			return true;
		}

		private void RequestAccessToHealthKit ()
		{
			var healthStore = new HKHealthStore ();

			var types = new NSSet (HKObjectType.GetWorkoutType (),
								  HKSeriesType.WorkoutRouteType,
								  HKQuantityType.Create (HKQuantityTypeIdentifier.ActiveEnergyBurned),
								  HKQuantityType.Create (HKQuantityTypeIdentifier.DistanceWalkingRunning));

			healthStore.RequestAuthorizationToShare (types, types, (isSuccess, error) => {
				if (!isSuccess) {
					Console.WriteLine (error?.LocalizedDescription ?? "");
				}
			});
		}

		private void RequestLocationServices ()
		{
			if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				this.locationManager.RequestAlwaysAuthorization ();
			}
		}
	}
}
