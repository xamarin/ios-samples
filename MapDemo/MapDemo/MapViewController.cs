using System;
using UIKit;
using CoreLocation;
using MapKit;

namespace MapDemo
{
	partial class MapViewController : UIViewController
	{
		MyMapDelegate mapDel;
		UISearchController searchController;
		CLLocationManager locationManager = new CLLocationManager ();

		public MapViewController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			locationManager.RequestWhenInUseAuthorization ();

			// set map type and show user location
			map.MapType = MKMapType.Standard;
			map.ShowsUserLocation = true;
			map.Bounds = View.Bounds;

			// set map center and region
			const double lat = 42.374260;
			const double lon = -71.120824;
			var mapCenter = new CLLocationCoordinate2D (lat, lon);
			var mapRegion = MKCoordinateRegion.FromDistance (mapCenter, 2000, 2000);
			map.CenterCoordinate = mapCenter;
			map.Region = mapRegion;

			// add an annotation
			map.AddAnnotation (new MKPointAnnotation {
				Title = "MyAnnotation", 
				Coordinate = new CLLocationCoordinate2D (42.364260, -71.120824)
			});

			// set the map delegate
			mapDel = new MyMapDelegate ();
			map.Delegate = mapDel;

			// add a custom annotation
			map.AddAnnotation (new MonkeyAnnotation ("Xamarin", mapCenter));

			// add an overlay
			var circleOverlay = MKCircle.Circle (mapCenter, 1000);
			map.AddOverlay (circleOverlay);

			var searchResultsController = new SearchResultsViewController (map);


			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
			searchController = new UISearchController (searchResultsController) {
				SearchResultsUpdater = searchUpdater
			};

			searchController.SearchBar.SizeToFit ();
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
			searchController.SearchBar.Placeholder = "Enter a search query";

			searchController.HidesNavigationBarDuringPresentation = false;
			NavigationItem.TitleView = searchController.SearchBar;
			DefinesPresentationContext = true;
		}

		public class SearchResultsUpdator : UISearchResultsUpdating
		{
			public event Action<string> UpdateSearchResults = delegate {};

			public override void UpdateSearchResultsForSearchController (UISearchController searchController)
			{
				this.UpdateSearchResults (searchController.SearchBar.Text);
			}
		}

		class MyMapDelegate : MKMapViewDelegate
		{
			string pId = "PinAnnotation";
			string mId = "MonkeyAnnotation";

			public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
			{
				MKAnnotationView anView;

				if (annotation is MKUserLocation)
					return null; 

				if (annotation is MonkeyAnnotation) {

					// show monkey annotation
					anView = mapView.DequeueReusableAnnotation (mId);

					if (anView == null)
						anView = new MKAnnotationView (annotation, mId);

					anView.Image = UIImage.FromFile ("monkey.png");
					anView.CanShowCallout = true;
					anView.Draggable = true;
					anView.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);

				} else {

					// show pin annotation
					anView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (pId);

					if (anView == null)
						anView = new MKPinAnnotationView (annotation, pId);

					((MKPinAnnotationView)anView).PinColor = MKPinAnnotationColor.Red;
					anView.CanShowCallout = true;
				}

				return anView;
			}

			public override void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
			{
				var monkeyAn = view.Annotation as MonkeyAnnotation;

				if (monkeyAn != null) {
					var alert = new UIAlertView ("Monkey Annotation", monkeyAn.Title, null, "OK");
					alert.Show ();
				}
			}

			public override MKOverlayView GetViewForOverlay (MKMapView mapView, IMKOverlay overlay)
			{
				var circleOverlay = overlay as MKCircle;
				var circleView = new MKCircleView (circleOverlay);
				circleView.FillColor = UIColor.Red;
				circleView.Alpha = 0.4f;
				return circleView;
			}
		}
	}
}
