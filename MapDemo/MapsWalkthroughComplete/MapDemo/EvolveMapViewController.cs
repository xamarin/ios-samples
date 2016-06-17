using System;
using CoreGraphics;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace MapDemo
{
	public partial class EvolveMapViewController : UIViewController
	{
		MKMapView map;
		MapDelegate mapDelegate;
		CLLocationManager locationManager = new CLLocationManager();

		public override void LoadView ()
		{
			map = new MKMapView (UIScreen.MainScreen.Bounds);
			View = map;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (UIDevice.CurrentDevice.CheckSystemVersion(8,0)){
				locationManager.RequestWhenInUseAuthorization ();
			}



			// change map type, show user location and allow zooming and panning
			map.MapType = MKMapType.Standard;
			map.ShowsUserLocation = true;
			map.ZoomEnabled = true;
			map.ScrollEnabled = true;

			// set map center and region
			double lat = 30.2652233534254;
			double lon = -97.73815460962083;
			CLLocationCoordinate2D mapCenter = new CLLocationCoordinate2D (lat, lon);
			MKCoordinateRegion mapRegion = MKCoordinateRegion.FromDistance (mapCenter, 100, 100);
			map.CenterCoordinate = mapCenter;
			map.Region = mapRegion;

			// set the map delegate
			mapDelegate = new MapDelegate ();
			map.Delegate = mapDelegate;

			// add a custom annotation at the map center
			map.AddAnnotations (new ConferenceAnnotation ("Evolve Conference", mapCenter));

			// add an overlay of the hotel
			MKPolygon hotelOverlay = MKPolygon.FromCoordinates (
				new CLLocationCoordinate2D[] {
				new CLLocationCoordinate2D(30.2649977168594, -97.73863627705),
				new CLLocationCoordinate2D(30.2648461170005, -97.7381627734755),
				new CLLocationCoordinate2D(30.2648355402574, -97.7381750192576),
				new CLLocationCoordinate2D(30.2647791309417, -97.7379872505988),
				new CLLocationCoordinate2D(30.2654525150319, -97.7377341711021),
				new CLLocationCoordinate2D(30.2654807195004, -97.7377994819399),
				new CLLocationCoordinate2D(30.2655089239607, -97.7377994819399),
				new CLLocationCoordinate2D(30.2656428950368, -97.738346460207),
				new CLLocationCoordinate2D(30.2650364981811, -97.7385709662122),
				new CLLocationCoordinate2D(30.2650470749025, -97.7386199493406)
			});

			map.AddOverlay (hotelOverlay);

			UITapGestureRecognizer tap = new UITapGestureRecognizer (g => {
				var pt = g.LocationInView (map);
				CLLocationCoordinate2D tapCoord = map.ConvertPoint (pt, map);

				Console.WriteLine ("new CLLocationCoordinate2D({0}, {1}),", tapCoord.Latitude, tapCoord.Longitude);
			});

			map.AddGestureRecognizer (tap);
		}			
	}
}

