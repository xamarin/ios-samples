using System;
using CoreLocation;
using Foundation;
using MapKit;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	public class LocationPrivacyManager : IPrivacyManager
	{
		public event EventHandler LocationChanged;

		readonly CLLocationManager locationManager;

		TaskCompletionSource<object> tcs;

		public CLLocationManager LocationManager {
			get {
				return locationManager;
			}
		}

		public MKCoordinateRegion Region { get; private set; }

		public string LocationInfo {
			get {
				return LocationManager.Location.ToString();
			}
		}

		public LocationPrivacyManager ()
		{
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
			Region = new MKCoordinateRegion(LocationManager.Location.Coordinate, new MKCoordinateSpan(0.1, 0.1));

			var handler = LocationChanged;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
				LocationManager.StartUpdatingLocation ();

			if(tcs != null)
				tcs.SetResult (null);
		}

		public Task RequestAccess ()
		{
			// Also note that info.plist has the NSLocationWhenInUseUsageDescription key
			// This call is asynchronous
			LocationManager.RequestWhenInUseAuthorization ();

			tcs = new TaskCompletionSource<object> ();
			return tcs.Task;
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