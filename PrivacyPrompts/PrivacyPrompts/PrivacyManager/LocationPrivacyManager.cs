using System;
using CoreLocation;
using Foundation;
using MapKit;

namespace PrivacyPrompts
{
	public class LocationPrivacyManager //: IPrivacyManager
	{
		readonly CLLocationManager locationManager;
		readonly LocationPrivacyViewController viewController;

		public CLLocationManager LocationManager {
			get {
				return locationManager;
			}
		}

		public LocationPrivacyManager (LocationPrivacyViewController vc)
		{
			viewController = vc;
			locationManager = new CLLocationManager ();

			// If previously allowed, start location manager
			if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
				locationManager.StartUpdatingLocation();

			locationManager.Failed += OnFailed;
			locationManager.LocationsUpdated += OnLocationsUpdated;
			locationManager.AuthorizationChanged += OnAuthorizationChanged;
		}

		void OnFailed (object sender, NSErrorEventArgs e)
		{
			LocationManager.StopUpdatingLocation ();
		}

		void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			viewController.LocationLbl.Text = LocationManager.Location.ToString();

			MKCoordinateRegion region = new MKCoordinateRegion(LocationManager.Location.Coordinate, new MKCoordinateSpan(0.1, 0.1));
			viewController.Map.SetRegion(region, true);
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			viewController.AccessStatus.Text = e.Status.ToString();
			if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
			{
				viewController.Map.ShowsUserLocation = true;
				LocationManager.StartUpdatingLocation ();
			}
		}

		public void RequestAccess ()
		{
			// Also note that info.plist has the NSLocationWhenInUseUsageDescription key
			// This call is asynchronous
			LocationManager.RequestWhenInUseAuthorization ();
		}

		public string CheckAccess ()
		{
			return CLLocationManager.Status.ToString ();
		}

		public void Dispose ()
		{
			locationManager.StopUpdatingLocation ();

			locationManager.Failed -= OnFailed;
			locationManager.LocationsUpdated -= OnLocationsUpdated;
			locationManager.AuthorizationChanged -= OnAuthorizationChanged;

			locationManager.Dispose ();
		}
	}
}