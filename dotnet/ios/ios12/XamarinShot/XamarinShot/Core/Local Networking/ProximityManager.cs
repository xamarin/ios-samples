namespace XamarinShot.Models;

public interface IProximityManagerDelegate
{
        void LocationChanged (ProximityManager manager, GameTableLocation location);

        void AuthorizationChanged (ProximityManager manager, bool authorization);
}

public class ProximityManager : CLLocationManagerDelegate
{
        readonly NSUuid RegionUUID = new NSUuid ("53FA6CD3-DFE4-493C-8795-56E71D2DAEAF");

        const string RegionId = "GameRoom";

        readonly CLLocationManager locationManager = new CLLocationManager ();

        readonly CLBeaconRegion? region;

        public ProximityManager () : base ()
        {
                region = new CLBeaconRegion (RegionUUID, RegionId);

                locationManager.Delegate = this;
                RequestAuthorization ();
        }

        public static ProximityManager Shared { get; } = new ProximityManager ();

        public IProximityManagerDelegate? Delegate { get; set; }

        public GameTableLocation? ClosestLocation { get; private set; }

        public bool IsAvailable => CLLocationManager.IsMonitoringAvailable (typeof (CLBeaconRegion));

        public bool IsAuthorized => CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse ||
                                    CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways;

        public void Start ()
        {
                if (IsAvailable)
                {
                        locationManager.StartRangingBeacons (region);
                }
        }

        public void Stop ()
        {
                if (IsAvailable)
                {
                        locationManager.StopRangingBeacons (region);
                }
        }

        public override void DidRangeBeacons (CLLocationManager manager, CLBeacon [] beacons, CLBeaconRegion region)
        {
                // we want to filter out beacons that have unknown proximity
                var knownBeacon = beacons.FirstOrDefault (beacon => beacon.Proximity != CLProximity.Unknown);
                if (knownBeacon is not null)
                {
                        GameTableLocation? location = null;
                        if (knownBeacon.Proximity == CLProximity.Near || knownBeacon.Proximity == CLProximity.Immediate)
                        {
                                location = GameTableLocation.GetLocation (knownBeacon.Minor.Int32Value);
                        }

                        if (ClosestLocation != location)
                        {
                                // Closest location changed 
                                ClosestLocation = location;
                                Delegate?.LocationChanged (this, location);
                        }
                }
        }

        public override void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
        {
                Delegate?.AuthorizationChanged (this, IsAuthorized);
        }

        void RequestAuthorization ()
        {
                if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
                {
                        locationManager.RequestWhenInUseAuthorization ();
                }
        }
}
