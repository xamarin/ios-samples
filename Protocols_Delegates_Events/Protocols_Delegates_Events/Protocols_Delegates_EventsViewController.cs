using System;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;

namespace Protocols_Delegates_Events
{
	public partial class Protocols_Delegates_EventsViewController : UIViewController
	{
		#region Private Variables
		private SampleMapDelegate _mapDelegate;
		#endregion

		#region Constructors
		public Protocols_Delegates_EventsViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region View lifecycle
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

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion
	}
}

