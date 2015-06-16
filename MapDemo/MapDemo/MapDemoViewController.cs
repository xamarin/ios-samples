using System;
using CoreGraphics;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;
using System.Linq;
using System.Collections.Generic;

namespace MapDemo
{
	public partial class MapDemoViewController : UIViewController
	{
		MyMapDelegate mapDel;
		UISearchBar searchBar;
		UISearchDisplayController searchController;
		CLLocationManager locationManager = new CLLocationManager();
		public MapDemoViewController () : base ("MapDemoViewController", null)
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
			double lat = 42.374260;
			double lon = -71.120824;
			var mapCenter = new CLLocationCoordinate2D (lat, lon);
			var mapRegion = MKCoordinateRegion.FromDistance (mapCenter, 2000, 2000);
			map.CenterCoordinate = mapCenter;
			map.Region = mapRegion;

			// add an annotation
			map.AddAnnotation (new MKPointAnnotation () {
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

			// create search controller
			searchBar = new UISearchBar (new CGRect (0, 20, View.Frame.Width, 50)) {
				Placeholder = "Enter a search query"
			};

			searchController = new UISearchDisplayController (searchBar, this);
			searchController.Delegate = new SearchDelegate (map);
			searchController.SearchResultsSource = new SearchSource (searchController, map);
			View.AddSubview (searchBar);
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

		class SearchDelegate : UISearchDisplayDelegate
		{
			MKMapView map;

			public SearchDelegate (MKMapView map)
			{
				this.map = map;
			}

			public override bool ShouldReloadForSearchString (UISearchDisplayController controller, string forSearchString)
			{
				// create search request
				var searchRequest = new MKLocalSearchRequest ();
				searchRequest.NaturalLanguageQuery = forSearchString;
				searchRequest.Region = new MKCoordinateRegion (map.UserLocation.Coordinate, new MKCoordinateSpan (0.25, 0.25));

				// perform search
				var localSearch = new MKLocalSearch (searchRequest);
				localSearch.Start (delegate (MKLocalSearchResponse response, NSError error) {
					if (response != null && error == null) {
						((SearchSource)controller.SearchResultsSource).MapItems = response.MapItems.ToList();
						controller.SearchResultsTableView.ReloadData();
					} else {
						Console.WriteLine ("local search error: {0}", error);

//						In "MKTypes.h" in the MapKit framework, the following is defined:
//
//							Error constants for the Map Kit framework.
//
//								enum MKErrorCode {
//								MKErrorUnknown = 1,
//								MKErrorServerFailure,
//								MKErrorLoadingThrottled,
//								MKErrorPlacemarkNotFound,
//							};
					}
				});

				return true;
			}
		}

		class SearchSource : UITableViewSource
		{
			static readonly string mapItemCellId = "mapItemCellId";
			UISearchDisplayController searchController;
			MKMapView map;

			public List<MKMapItem> MapItems { get; set; }

			public SearchSource (UISearchDisplayController searchController, MKMapView map)
			{
				this.searchController = searchController;
				this.map = map;

				MapItems = new List<MKMapItem> ();
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return MapItems.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (mapItemCellId);

				if (cell == null)
					cell = new UITableViewCell ();

				cell.TextLabel.Text = MapItems [indexPath.Row].Name;
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				searchController.SetActive (false, true);

				// add item to map
				CLLocationCoordinate2D coord = MapItems [indexPath.Row].Placemark.Location.Coordinate;
				map.AddAnnotation (new MKPointAnnotation () {
					Title = MapItems [indexPath.Row].Name, 
					Coordinate = coord
				});

				map.SetCenterCoordinate (coord, true);
			}
		}
	}
}

