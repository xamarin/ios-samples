using CoreLocation;
using Foundation;
using MapKit;

namespace MapCallouts.Annotations
{
    /// <summary>
    /// The custom MKAnnotation object representing the city of San Francisco.
    /// </summary>
    public class SanFranciscoAnnotation : MKAnnotation
    {
        public override CLLocationCoordinate2D Coordinate => new CLLocationCoordinate2D(37.779_379f, -122.418_433f);

        public override string Title =>  NSBundle.MainBundle.GetLocalizedString("SAN_FRANCISCO_TITLE");

        public override string Subtitle =>  NSBundle.MainBundle.GetLocalizedString("SAN_FRANCISCO_SUBTITLE");
    }
}