using CoreLocation;
using MapKit;

namespace MapCallouts.Annotations
{
    /// <summary>
    /// The custom MKAnnotationView object representing a generic location, displaying a title and image.
    /// </summary>
    public class CustomAnnotation : MKAnnotation
    {
        private readonly CLLocationCoordinate2D coordinate;

        private readonly string title;

        public CustomAnnotation(CLLocationCoordinate2D coordinate, string title) : base()
        {
            this.coordinate = coordinate;
            this.title = title;
        }

        public override string Title => this.title;

        public override CLLocationCoordinate2D Coordinate => this.coordinate;

        public string ImageName { get; set; }
    }
}