using CoreLocation;
using Foundation;
using MapKit;

namespace MapCallouts.Annotations
{
    /// <summary>
    /// The custom MKAnnotation object representing the Ferry Building.
    /// </summary>
    public class FerryBuildingAnnotation : MKAnnotation
    {
        public override CLLocationCoordinate2D Coordinate => new CLLocationCoordinate2D(37.808_333f, -122.415_556f);

        public override string Title => NSBundle.MainBundle.GetLocalizedString("FERRY_BUILDING_TITLE");
    }
}