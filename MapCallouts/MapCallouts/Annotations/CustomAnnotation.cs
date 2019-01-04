using CoreLocation;
using Foundation;
using MapKit;
using ObjCRuntime;

namespace MapCallouts.Annotations
{
    /// <summary>
    /// The custom MKAnnotationView object representing a generic location, displaying a title and image.
    /// </summary>
    public class CustomAnnotation : MKAnnotation
    {
        private CLLocationCoordinate2D coordinate;

        private readonly string title;

        public CustomAnnotation(CLLocationCoordinate2D coordinate, string title) : base()
        {
            this.coordinate = coordinate;
            this.title = title;
        }

        public override string Title => this.title;

        public override CLLocationCoordinate2D Coordinate => this.coordinate;

        public string ImageName { get; set; }

        //[return: Release]
        //public override NSObject Copy()
        //{
        //    var res = new CustomAnnotation(this.Coordinate, this.title);
        //    return res;
        //}

        //[return: Release]
        //public override NSObject MutableCopy()
        //{
        //    var res = new CustomAnnotation(this.Coordinate, this.title);
        //    return res;
        //}
    }
}