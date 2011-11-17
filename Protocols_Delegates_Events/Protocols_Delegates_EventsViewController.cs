using System;
using System.Drawing;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;

namespace Protocols_Delegates_Events
{
    public partial class Protocols_Delegates_EventsViewController : UIViewController
    {
        SampleMapDelegate _mapDelegate;
        
        public Protocols_Delegates_EventsViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
        {     
        }
     
        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
        }
     
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            
            //Exmaple 1 - Using Strongly tryped delegate
            //set the map's delegate
            _mapDelegate = new SampleMapDelegate ();
            map.Delegate = _mapDelegate;
            
            //Example 2 - Using weak deleegate
            //map.WeakDelegate = this;
            
            //Exmaple 3 - Using .NET event
            /*map.DidSelectAnnotationView += (s,e) => {
                
                var sampleAnnotation = e.View.Annotation as SampleMapAnnotation;
                
                if (sampleAnnotation != null) {
                    
                    //demo accessing the coordinate of the selected annotation to zoom in on it
                    mapView.Region = MKCoordinateRegion.FromDistance (sampleAnnotation.Coordinate, 500, 500);
                    
                    //demo accessing the title of the selected annotation
                    Console.WriteLine ("{0} was tapped", sampleAnnotation.Title);
                }     
            };*/
            
            //an arbitrary coordinate used for demonstration here
            var sampleCoordinate = new CLLocationCoordinate2D (42.3467512, -71.0969456);
            
            //center the map on the coordinate
            map.SetCenterCoordinate (sampleCoordinate, false);
  
            //create an annotation and add it to the map
            map.AddAnnotation (new SampleMapAnnotation (sampleCoordinate));
        }
     
        public override void ViewDidUnload ()
        {
            base.ViewDidUnload ();
         
            // Release any retained subviews of the main view.
            // e.g. myOutlet = null;
        }
     
        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
        }
        
        class SampleMapDelegate : MKMapViewDelegate
        {
            public override void DidSelectAnnotationView (MKMapView mapView, MKAnnotationView annotationView)
            {
                var sampleAnnotation = annotationView.Annotation as SampleMapAnnotation;
                
                if (sampleAnnotation != null) {
                    
                    //demo accessing the coordinate of the selected annotation to zoom in on it
                    mapView.Region = MKCoordinateRegion.FromDistance (sampleAnnotation.Coordinate, 500, 500);
                    
                    //demo accessing the title of the selected annotation
                    Console.WriteLine ("{0} was tapped", sampleAnnotation.Title);
                }
                
            }
        }
        
        //bind to the Objective-C selector mapView:didSelectAnnotationView:
        [Export("mapView:didSelectAnnotationView:")]
        public void DidSelectAnnotationView (MKMapView mapView, MKAnnotationView annotationView)
        {
            var sampleAnnotation = annotationView.Annotation as SampleMapAnnotation;
                
            if (sampleAnnotation != null) {
                    
                //demo accessing the coordinate of the selected annotation to zoom in on it
                mapView.Region = MKCoordinateRegion.FromDistance (sampleAnnotation.Coordinate, 500, 500);
                    
                //demo accessing the title of the selected annotation
                Console.WriteLine ("{0} was tapped", sampleAnnotation.Title);
            }        
        }
        
    }
}
