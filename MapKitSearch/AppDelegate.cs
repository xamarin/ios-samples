using System;
using System.Collections.Generic;
using CoreLocation;
using Foundation;
using MonoTouch.Dialog;
using MapKit;
using UIKit;

using System.Threading.Tasks;

namespace MapKitSearch {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		UIWindow window;
		CLLocationManager lm;
		CLLocation here;
		List<MKMapItem> results = new List<MKMapItem> ();

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var what = new EntryElement ("What ?", "e.g. pizza", String.Empty);
			var where = new EntryElement ("Where ?", "here", String.Empty);

			var section = new Section ();
			if (CLLocationManager.LocationServicesEnabled) {
				lm = new CLLocationManager ();
				lm.LocationsUpdated += delegate (object sender, CLLocationsUpdatedEventArgs e) {
					lm.StopUpdatingLocation ();
					here = e.Locations [e.Locations.Length - 1];
					var coord = here.Coordinate;
					where.Value = String.Format ("{0:F4}, {1:F4}", coord.Longitude, coord.Latitude);
				};
				section.Add (new StringElement ("Get Current Location", delegate {
					lm.StartUpdatingLocation ();
				}));
			}

			section.Add (new StringElement ("Search...", async delegate {
				await SearchAsync (what.Value, where.Value);
			}));

			var root = new RootElement ("MapKit Search Sample") {
				new Section ("MapKit Search Sample") { what, where },
				section
			};
			window.RootViewController = new UINavigationController (new DialogViewController (root, true));
			window.MakeKeyAndVisible ();
			return true;
		}

		CLLocationCoordinate2D Parse (string s)
		{
			if (!String.IsNullOrEmpty (s)) {
				string[] values = s.Split (new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length == 2) {
					double longitude, latitude;
					if (Double.TryParse (values [0], out longitude) && Double.TryParse (values [1], out latitude))
						return new CLLocationCoordinate2D (longitude, latitude);
				}
			}
			return new CLLocationCoordinate2D ();
		}

		async Task SearchAsync (string what, string where)
		{
			var coord = here == null ? Parse (where) : here.Coordinate;

			MKCoordinateSpan span = new MKCoordinateSpan (0.25, 0.25);
			MKLocalSearchRequest request = new MKLocalSearchRequest ();
			request.Region = new MKCoordinateRegion (coord, span);
			request.NaturalLanguageQuery = what;
			MKLocalSearch search = new MKLocalSearch (request);
			MKLocalSearchResponse response;
			try{
				response = await search.StartAsync ();

			}
			catch(Exception e){
				return;
			}	
			if (response == null)			
				return;	

			var section = new Section ("Search Results for " + what);
			results.Clear ();
			foreach (MKMapItem mi in response.MapItems) {
				results.Add (mi);
				var element = new StyledStringElement (mi.Name, mi.PhoneNumber, UITableViewCellStyle.Subtitle);
				element.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				element.Tapped += () => { results [(int)element.IndexPath.Row].OpenInMaps (); };
				section.Add (element);
			}

			var root = new RootElement ("MapKit Search Sample") { section };
			var dvc = new DialogViewController (root);
			(window.RootViewController as UINavigationController).PushViewController (dvc, true);

		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}