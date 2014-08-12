using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using System.Linq;

namespace CloudKitAtlas
{
	public partial class CKRecordViewController : UIViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		private MKPointAnnotation pin;
		private CLLocation currentLocation;
		private CLLocationManager locationManager;

		public CKRecordViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			map.GetViewForAnnotation = (mapView, annotation) => {
				return new MKPinAnnotationView (annotation, string.Empty) {
					Draggable = true
				};
			};

			locationManager = new CLLocationManager {
				DistanceFilter = CLLocationDistance.FilterNone,
				DesiredAccuracy = CLLocation.AccuracyBest,
			};

			locationManager.LocationsUpdated += OnLocationsUpdated;

			locationManager.RequestAlwaysAuthorization ();
			locationManager.StartUpdatingLocation ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			locationManager.LocationsUpdated -= OnLocationsUpdated;
		}

		async partial void SaveRecord (MonoTouch.UIKit.UIButton sender)
		{
			if (nameTextField.Text.Length < 1) {
				nameTextField.ResignFirstResponder ();
				return;
			}

			var saveLocation = new CLLocation (pin.Coordinate.Latitude, pin.Coordinate.Longitude);
			var record = await CloudManager.AddRecordAsync (nameTextField.Text, saveLocation);

			if (record == null) {
				Console.WriteLine ("Error: null returned on save");
				return;
			}

			nameTextField.Text = string.Empty;
			nameTextField.ResignFirstResponder ();

			var alert = UIAlertController.Create ("CloudKitAtlas", "Saved record", UIAlertControllerStyle.Alert);
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (act) => {
				DismissViewController (true, null);
			}));

			PresentViewController (alert, true, null);
		}

		private void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			currentLocation = e.Locations.LastOrDefault ();

			if (pin != null)
				return;

			pin = new MKPointAnnotation {
				Coordinate = currentLocation.Coordinate
			};

			map.AddAnnotation (pin);
			map.ShowAnnotations (new [] { pin }, false);

			locationManager.StopUpdatingLocation ();
		}
	}
}
