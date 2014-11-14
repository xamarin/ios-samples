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
        string _title;
        
        public SampleMapAnnotation (CLLocationCoordinate2D coordinate)
        {
            Coordinate = coordinate;
            _title = "Sample";
        }
        
        public override CLLocationCoordinate2D Coordinate { get; set; }
        
        public override string Title {
            get {
                return _title;
            }            
        }
    }
}

