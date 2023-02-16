using CoreLocation;
using Foundation;
using MapKit;
using System;
using System.Threading.Tasks;
using UIKit;

namespace MapKitSearch {
	public partial class ViewController : UIViewController, IUITextFieldDelegate {
		private const string SearchSegueIdentifier = "openSearchResults";

		private CLLocationManager locationManager;

		private MKMapItem [] searchItems;

		protected ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SearchButton.Enabled = false;
			QueryTextField.Delegate = this;
			LocationTextField.Delegate = this;

			GetLocationButton.Enabled = CLLocationManager.LocationServicesEnabled;
			QueryTextField.AddTarget ((sender, e) => SearchButton.Enabled = !string.IsNullOrEmpty (QueryTextField.Text),
									 UIControlEvent.EditingChanged);

			if (CLLocationManager.LocationServicesEnabled) {
				locationManager = new CLLocationManager ();
				locationManager.RequestWhenInUseAuthorization ();
				locationManager.LocationsUpdated += (sender, e) => {
					locationManager.StopUpdatingLocation ();
					var location = e.Locations [e.Locations.Length - 1];
					LocationTextField.Text = string.Format ("{0:F4}, {1:F4}", location.Coordinate.Latitude, location.Coordinate.Longitude);
				};
			}
		}

		partial void GetCurrentLocation (UIButton sender)
		{
			locationManager.StartUpdatingLocation ();
		}

		async partial void Search (UIButton sender)
		{
			await SearchAsync (QueryTextField.Text, LocationTextField.Text).ConfigureAwait (false);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (!string.IsNullOrEmpty (segue?.Identifier) && segue.Identifier == SearchSegueIdentifier) {
				if (segue.DestinationViewController is SearchResultsController searchResultsController) {
					searchResultsController.Query = QueryTextField.Text;
					searchResultsController.Items = searchItems;
				}
			}
		}

		private async Task SearchAsync (string what, string where)
		{
			var span = new MKCoordinateSpan (0.25, 0.25);
			var request = new MKLocalSearchRequest {
				NaturalLanguageQuery = what,
				Region = new MKCoordinateRegion (ParseCoordinates (where), span),
			};

			var search = new MKLocalSearch (request);
			MKLocalSearchResponse response = null;

			try {
				response = await search.StartAsync ();
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}

			if (response != null) {
				searchItems = response.MapItems;
				PerformSegue (SearchSegueIdentifier, this);
			}
		}

		private CLLocationCoordinate2D ParseCoordinates (string coordinates)
		{
			var result = new CLLocationCoordinate2D ();

			if (!string.IsNullOrEmpty (coordinates)) {
				var values = coordinates.Split (new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length == 2) {
					if (double.TryParse (values [0], out double latitude) &&
						double.TryParse (values [1], out double longitude)) {
						result = new CLLocationCoordinate2D (latitude, longitude);
					}
				}
			}

			return result;
		}

		#region IUITextFieldDelegate

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			if (textField == QueryTextField) {
				LocationTextField.BecomeFirstResponder ();
			} else {
				textField.ResignFirstResponder ();
			}

			return true;
		}

		#endregion
	}
}
