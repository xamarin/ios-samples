using CoreLocation;
using MapKit;

namespace RegionDefiner
{
    public class MyAnnotation : MKAnnotation
    {
        private readonly CLLocationCoordinate2D coordinate;
        private readonly string title;
        private readonly string subtitle;

        public MyAnnotation(CLLocationCoordinate2D coordinate, string title, string subtitle)
        {
            this.coordinate = coordinate;
            this.title = title;
            this.subtitle = subtitle;
        }

        public override string Title => this.title;

        public override string Subtitle => this.subtitle;

        public override CLLocationCoordinate2D Coordinate => this.coordinate;
    }
}