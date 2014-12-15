using System;

using Foundation;
using UIKit;
using MapKit;
using CoreGraphics;
using CoreLocation;

namespace Footprint
{
	[Register ("MainViewController")]
	public class MainViewController : UIViewController
	{
		[Outlet ("imageView")]
		UIImageView ImageView { get; set; }

		[Outlet ("pinView")]
		UIImageView PinView { get; set; }

		[Outlet ("radiusView")]
		UIImageView RadiusView { get; set; }

		CLLocationManager locationManager;
		Tuple<GeoAnchor, GeoAnchor> anchorPair;

		CoordinateConverter coordinateConverter;

		nfloat displayScale;
		CGPoint displayOffset;

		public MainViewController (IntPtr handle)
			: base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			locationManager = new CLLocationManager ();
			locationManager.AuthorizationChanged += OnAuthorizationChanged;
			locationManager.LocationsUpdated += OnLocationsUpdated;
			locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			locationManager.ActivityType = CLActivityType.Other;

			// We setup a pair of anchors that will define how the floorplan image, maps to geographic co-ordinates
			var anchor1 = new GeoAnchor {
				LatitudeLongitude = new CLLocationCoordinate2D (37.770511, -122.465810),
				Pixel = new CGPoint (12, 18)
			};

			var anchor2 = new GeoAnchor {
				LatitudeLongitude = new CLLocationCoordinate2D (37.769125, -122.466356),
				Pixel = new CGPoint (481, 815)
			};

			anchorPair = new Tuple<GeoAnchor, GeoAnchor> (anchor1, anchor2);

			// Initialize the coordinate system converter with two anchor points.
			coordinateConverter = new CoordinateConverter (anchorPair);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			SetScaleAndOffset ();
			StartTrackingLocation ();
		}

		void SetScaleAndOffset ()
		{
			CGSize imageViewFrameSize = ImageView.Frame.Size;
			CGSize imageSize = ImageView.Image.Size;

			// Calculate how much we'll be scaling the image to fit on screen.
			nfloat wRatio = imageViewFrameSize.Width / imageSize.Width;
			nfloat hRatio = imageViewFrameSize.Height / imageSize.Height;
			displayScale = NMath.Min (wRatio, hRatio);
			Console.WriteLine ("Scale Factor: {0}", displayScale);

			// Depending on whether we're constrained by width or height,
			// figure out how much our floorplan pixels need to be offset to adjust for the image being centered
			if (wRatio < hRatio) {
				Console.WriteLine ("Constrained by width");
				displayOffset = new CGPoint (0, (imageViewFrameSize.Height - imageSize.Height * displayScale) / 2);
			} else {
				Console.WriteLine ("Constrained by height");
				displayOffset = new CGPoint ((imageViewFrameSize.Width - imageSize.Width * displayScale) / 2, 0);
			}

			Console.WriteLine ("Offset: x={0}, y={1}", displayOffset.X, displayOffset.Y);
		}

		void StartTrackingLocation ()
		{
			CLAuthorizationStatus status = CLLocationManager.Status;
			if (status == CLAuthorizationStatus.NotDetermined) {
				locationManager.RequestWhenInUseAuthorization ();
			} else if (status == CLAuthorizationStatus.AuthorizedWhenInUse || status == CLAuthorizationStatus.AuthorizedAlways) {
				locationManager.StartUpdatingLocation ();
			}
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			switch (e.Status) {
				case CLAuthorizationStatus.AuthorizedAlways:
				case CLAuthorizationStatus.AuthorizedWhenInUse:
					Console.WriteLine ("Got authorization, start tracking location");
					StartTrackingLocation ();
					break;

				case CLAuthorizationStatus.NotDetermined:
					locationManager.RequestWhenInUseAuthorization ();
					break;

				default:
					break;
			}
		}

		void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			// Pass location updates to the map view.
			foreach (var l in e.Locations) {
				Console.WriteLine ("Location (Floor {0}): {1}", l.Floor, l);
				UpdateViewWithLocation (l);
			}
		}

		void UpdateViewWithLocation (CLLocation location)
		{
			// We animate transition from one position to the next, this makes the dot move smoothly over the map
			UIView.Animate (0.75, () => {
				// Call the converter to find these coordinates on our floorplan.
				CGPoint pointOnImage = coordinateConverter.Convert (location.Coordinate);

				// These coordinates need to be scaled based on how much the image has been scaled
				var x = pointOnImage.X * displayScale + displayOffset.X;
				var y = pointOnImage.Y * displayScale + displayOffset.Y;
				var scaledPoint = new CGPoint (x, y);

				// Calculate and set the size of the radius
				nfloat radiusFrameSize = (nfloat)location.HorizontalAccuracy * coordinateConverter.PixelsPerMeter * 2;
				RadiusView.Frame = new CGRect (0, 0, radiusFrameSize, radiusFrameSize);

				// Move the pin and radius to the user's location
				PinView.Center = scaledPoint;
				RadiusView.Center = scaledPoint;
			});
		}

		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			SetScaleAndOffset ();
		}
	}
}

