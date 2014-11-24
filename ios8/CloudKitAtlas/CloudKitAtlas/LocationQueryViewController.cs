using System;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;
using System.Linq;
using System.Collections.Generic;
using CloudKit;
using ObjCRuntime;

namespace CloudKitAtlas
{
	public partial class LocationQueryViewController : UITableViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		MKPointAnnotation pin;
		CLLocation currentLocation;
		CLLocationManager locationManager;
		List<CKRecord> results;

		public LocationQueryViewController (IntPtr handle) : base (handle)
		{
			results = new List<CKRecord> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			locationManager = new CLLocationManager () {
				DistanceFilter = CLLocationDistance.FilterNone,
				DesiredAccuracy = CLLocation.AccuracyBest
			};

			locationManager.LocationsUpdated += OnLocationsUpdated;

			map.GetViewForAnnotation = (mapView, annotation) => {
				var view = new MKPinAnnotationView (annotation, string.Empty) {
					Draggable = true
				};
				return view;
			};

			locationManager.RequestAlwaysAuthorization ();
			locationManager.StartUpdatingLocation ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			locationManager.LocationsUpdated -= OnLocationsUpdated;
		}

		partial void QueryRecords (UIBarButtonItem sender)
		{
			var queryLocation = new CLLocation (pin.Coordinate.Latitude, pin.Coordinate.Longitude);

			CloudManager.QueryForRecords (queryLocation, records => {
				results = records;
				TableView.ReloadData ();
			});
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return (nint)results.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell (new NSString("cell"), indexPath);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			CKRecord record = results [indexPath.Row];
			cell.TextLabel.Text = (NSString)record ["name"];
			return cell;
		}

		void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			currentLocation = e.Locations.LastOrDefault ();

			if (pin != null)
				return;

			pin = new MKPointAnnotation ();
			pin.SetCoordinate (currentLocation.Coordinate);

			map.AddAnnotation (pin);
			map.ShowAnnotations (new [] { pin }, false);

			locationManager.StopUpdatingLocation ();
		}
	}
}
