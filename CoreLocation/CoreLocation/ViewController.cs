using Foundation;
using System;
using System.Linq;
using UIKit;

namespace CoreLocation
{
    public partial class ViewController : UIViewController, ICLLocationManagerDelegate
    {
        private readonly CLLocationManager locationManager = new CLLocationManager();

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // you can set the update threshold and accuracy if you want:
            //locationManager.DistanceFilter = 10d; // move ten meters before updating
            //locationManager.HeadingFilter = 3d; // move 3 degrees before updating

            // you can also set the desired accuracy:
            locationManager.DesiredAccuracy = 1000; // 1000 meters/1 kilometer
            // you can also use presets, which simply evalute to a double value:
            //locationManager.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters;

            locationManager.Delegate = this;
            locationManager.RequestWhenInUseAuthorization();

            if (CLLocationManager.LocationServicesEnabled)
            {
                locationManager.StartUpdatingLocation();
            }

            if (CLLocationManager.HeadingAvailable)
            {
                locationManager.StartUpdatingHeading();
            }
        }

        #region ICLLocationManagerDelegate

        [Export("locationManager:didUpdateHeading:")]
        public void UpdatedHeading(CLLocationManager manager, CLHeading newHeading)
        {
            trueHeadingLabel.Text = $"{newHeading.TrueHeading}º";
            magneticHeadingLabel.Text = $"{newHeading.MagneticHeading}º";
        }

        [Export("locationManager:didUpdateLocations:")]
        public void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            var location = locations.LastOrDefault();
            if (location != null)
            {
                altitudeLabel.Text = $"{location.Altitude} meters";
                longitudeLabel.Text = $"{location.Coordinate.Longitude}º";
                latitudeLabel.Text = $"{location.Coordinate.Latitude}º";
                courseLabel.Text = $"{location.Course}º";
                speedLabel.Text = $"{location.Speed} meters/s";

                // get the distance from here to paris
                distanceLabel.Text = $"{location.DistanceFrom(new CLLocation(48.857, 2.351)) / 1000} km";
            }
        }

        #endregion
    }
}