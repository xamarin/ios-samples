using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreLocation;

namespace GpsWatch
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		CLLocationManager locationManager;
		CLLocationCoordinate2D locationCoordinate;
		CLAuthorizationStatus status;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Console.WriteLine ("containing iOS app's state {0}", UIApplication.SharedApplication.ApplicationState);

			status = CLLocationManager.Status;
			if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active)
				SetupLocationManager ();

			return true;
		}

		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
			SetupLocationManager ();
		}

		void SetupLocationManager()
		{
			if (locationManager != null)
				return;

			locationManager = new CLLocationManager ();
			locationManager.AuthorizationChanged += OnAuthorizationChanged;
			locationManager.LocationsUpdated += OnLocationsUpdated;
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			Console.WriteLine ("new authorization state = {0}", status);
			status = e.Status;

			switch (status) {
				case CLAuthorizationStatus.AuthorizedAlways:
					locationManager.StartUpdatingLocation ();
					break;

				case CLAuthorizationStatus.NotDetermined:
					locationManager.RequestAlwaysAuthorization ();
					break;

				default:
					break;
			}
		}

		void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			locationCoordinate = e.Locations.Last ().Coordinate;
		}

		public override void HandleWatchKitExtensionRequest (UIApplication application, NSDictionary userInfo, Action<NSDictionary> reply)
		{
			reply (new NSDictionary ("status", NSNumber.FromUInt32 ((uint)status),
				"lon", NSNumber.FromDouble (locationCoordinate.Longitude),
				"lat", NSNumber.FromDouble (locationCoordinate.Latitude)));
		}
	}
}

