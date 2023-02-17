
namespace XamarinShot.Models {
	using CoreLocation;
	using Foundation;
	using System.Linq;

	public interface IProximityManagerDelegate {
		void LocationChanged (ProximityManager manager, GameTableLocation location);

		void AuthorizationChanged (ProximityManager manager, bool authorization);
	}

	public class ProximityManager : CLLocationManagerDelegate {
		private readonly NSUuid RegionUUID = new NSUuid ("53FA6CD3-DFE4-493C-8795-56E71D2DAEAF");

		private const string RegionId = "GameRoom";

		private readonly CLLocationManager locationManager = new CLLocationManager ();

		private readonly CLBeaconRegion region;

		public ProximityManager () : base ()
		{
			this.region = new CLBeaconRegion (RegionUUID, RegionId);

			this.locationManager.Delegate = this;
			this.RequestAuthorization ();
		}

		public static ProximityManager Shared { get; } = new ProximityManager ();

		public IProximityManagerDelegate Delegate { get; set; }

		public GameTableLocation ClosestLocation { get; private set; }

		public bool IsAvailable => CLLocationManager.IsMonitoringAvailable (typeof (CLBeaconRegion));

		public bool IsAuthorized => CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse ||
									CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways;

		public void Start ()
		{
			if (this.IsAvailable) {
				this.locationManager.StartRangingBeacons (this.region);
			}
		}

		public void Stop ()
		{
			if (this.IsAvailable) {
				this.locationManager.StopRangingBeacons (region);
			}
		}

		public override void DidRangeBeacons (CLLocationManager manager, CLBeacon [] beacons, CLBeaconRegion region)
		{
			// we want to filter out beacons that have unknown proximity
			var knownBeacon = beacons.FirstOrDefault (beacon => beacon.Proximity != CLProximity.Unknown);
			if (knownBeacon != null) {
				GameTableLocation location = null;
				if (knownBeacon.Proximity == CLProximity.Near || knownBeacon.Proximity == CLProximity.Immediate) {
					location = GameTableLocation.GetLocation (knownBeacon.Minor.Int32Value);
				}

				if (this.ClosestLocation != location) {
					// Closest location changed 
					this.ClosestLocation = location;
					this.Delegate?.LocationChanged (this, location);
				}
			}
		}

		public override void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
		{
			this.Delegate?.AuthorizationChanged (this, this.IsAuthorized);
		}

		private void RequestAuthorization ()
		{
			if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				this.locationManager.RequestWhenInUseAuthorization ();
			}
		}
	}
}
