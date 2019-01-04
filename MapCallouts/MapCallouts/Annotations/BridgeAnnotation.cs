using CoreLocation;
using Foundation;
using MapKit;

namespace MapCallouts.Annotations
{
    /// <summary>
    /// The custom MKAnnotation object representing the Golden Gate Bridge.
    /// </summary>
    public class BridgeAnnotation : MKAnnotation
    {
        public override CLLocationCoordinate2D Coordinate => new CLLocationCoordinate2D(37.810_000f, -122.477_450f);

        public override string Title => NSBundle.MainBundle.GetLocalizedString("BRIDGE_TITLE");

        public override string Subtitle => NSBundle.MainBundle.GetLocalizedString("BRIDGE_SUBTITLE");
    }
}