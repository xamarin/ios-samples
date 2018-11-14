
namespace MapDemo
{
    using CoreLocation;
    using MapKit;

    public class MonkeyAnnotation : MKAnnotation
    {
        private CLLocationCoordinate2D coordinate;

        private readonly string title;

        public MonkeyAnnotation(string title, CLLocationCoordinate2D coordinate)
        {
            this.title = title;
            this.coordinate = coordinate;
        }

        public override string Title => title;

        public override CLLocationCoordinate2D Coordinate => coordinate;
    }
}