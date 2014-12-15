using System;
using CoreLocation;
using MapKit;

namespace Protocols_Delegates_Events
{
    /// <summary>
    /// Annotation class that subclasses MKAnnotation abstract class
    /// MKAnnotation is bound by MonoTouch to the MKAnnotation protocol
    /// </summary>
    public class SampleMapAnnotation : MKAnnotation
    {
		CLLocationCoordinate2D coordinate;
        string _title;

        public SampleMapAnnotation (CLLocationCoordinate2D coordinateToSet)
        {
			coordinate = coordinateToSet;
            _title = "Sample";
        }

		public override CLLocationCoordinate2D Coordinate {
			get {
				return coordinate;
			}
		}

		public override void SetCoordinate (CLLocationCoordinate2D coordinateToSet)
		{
			coordinate = coordinateToSet;
		}

        public override string Title {
            get {
                return _title;
            }
        }
    }
}

