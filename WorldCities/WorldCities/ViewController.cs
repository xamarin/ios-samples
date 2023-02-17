using CoreLocation;
using Foundation;
using MapKit;
using ObjCRuntime;
using System;
using UIKit;

namespace WorldCities {
	public partial class ViewController : UIViewController {
		private WorldCity selectedCity;

		protected ViewController (IntPtr handle) : base (handle) { }

		partial void SelectorChanged (UISegmentedControl sender)
		{
			switch (sender.SelectedSegment) {
			case 1:
				mapView.MapType = MapKit.MKMapType.Satellite;
				break;
			case 2:
				mapView.MapType = MapKit.MKMapType.Hybrid;
				break;
			default:
				mapView.MapType = MapKit.MKMapType.Standard;
				break;
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue?.DestinationViewController is CitiesViewController controller) {
				controller.CityChanged += OnCityChanged;
			}
		}

		private void OnCityChanged (object sender, CityEventArgs e)
		{
			(sender as CitiesViewController).CityChanged -= OnCityChanged;
			selectedCity = e.City;
			ChooseWorldCity ();
		}

		private void ChooseWorldCity ()
		{
			var place = selectedCity;
			Title = place.Name;

			var current = mapView.Region;
			if (current.Span.LatitudeDelta < 10) {
				PerformSelector (new Selector ("AnimateToWorld"), null, 0.3f);
				PerformSelector (new Selector ("AnimateToPlace"), null, 1.7f);
			} else {
				PerformSelector (new Selector ("AnimateToPlace"), null, 0.3f);
			}
		}

		[Export ("AnimateToWorld")]
		private void AnimateToWorld ()
		{
			var city = selectedCity;
			var region = mapView.Region;
			var zoomOut = new MKCoordinateRegion (new CLLocationCoordinate2D ((region.Center.Latitude + city.Latitude) / 2d,
																			(region.Center.Longitude + city.Longitude) / 2d),
												 new MKCoordinateSpan (90, 90));
			mapView.SetRegion (zoomOut, true);
		}

		[Export ("AnimateToPlace")]
		private void AnimateToPlace ()
		{
			var city = selectedCity;
			var region = new MKCoordinateRegion (new CLLocationCoordinate2D (city.Latitude, city.Longitude),
												new MKCoordinateSpan (0.4, 0.4));
			mapView.SetRegion (region, true);
		}
	}
}
