using System;
using CoreLocation;
using MapKit;

namespace MapDemo
{
    public class MonkeyAnnotation : MKAnnotation
    {
        string title;
        CLLocationCoordinate2D coord;

        public MonkeyAnnotation (string title, CLLocationCoordinate2D coord)
        {
            this.title = title;
            this.coord = coord;
        }

        public override string Title {
            get {
                return title;
            }
        }

        public override CLLocationCoordinate2D Coordinate {
            get {
                return coord;
            }
        }
    }
}

