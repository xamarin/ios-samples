using System;
using CoreLocation;
using UIKit;
using CoreGraphics;
using MapKit;
using Foundation;

namespace PrivacyPrompts
{
	public class LocationPrivacyViewController : PrivacyDetailViewController
	{
		CLLocationManager locationManager;
		UILabel locationMessage;
		MKMapView mapView;

		public LocationPrivacyViewController ()
		{
			locationManager = new CLLocationManager ();

			// If previously allowed, start location manager
			if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
				locationManager.StartUpdatingLocation();

			locationManager.Failed += OnFailed;
			locationManager.LocationsUpdated += OnLocationsUpdated;
			locationManager.AuthorizationChanged += OnAuthorizationChanged;
		}

		void OnFailed (object sender, NSErrorEventArgs e)
		{
			locationManager.StopUpdatingLocation ();
		}

		void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			locationMessage.Text = locationManager.Location.ToString();

			//MapView
			MKCoordinateRegion region = new MKCoordinateRegion(locationManager.Location.Coordinate, new MKCoordinateSpan(0.1, 0.1));
			mapView.SetRegion(region, true);
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			AccessStatus.Text = e.Status.ToString();
			if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
			{
				mapView.ShowsUserLocation = true;
				locationManager.StartUpdatingLocation ();
			}
		}

		protected override string CheckAccess ()
		{
			return CLLocationManager.Status.ToString ();
		}

		void AddLocationMessage()
		{
			locationMessage = new UILabel (CGRect.Empty);
			locationMessage.TranslatesAutoresizingMaskIntoConstraints = false;
			locationMessage.Lines = 0;
			locationMessage.Font = UIFont.SystemFontOfSize (UIFont.SmallSystemFontSize);

			this.View.AddSubview (locationMessage);

			locationMessage.AddConstraints (new [] {
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 56),
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.Width, 1, 300),
			});

			this.View.AddConstraints (new[] {
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, TitleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, RequestAccessButton, NSLayoutAttribute.Bottom, 1, 40),
			});
		}

		void AddMap()
		{
			mapView = new MKMapView ();
			mapView.TranslatesAutoresizingMaskIntoConstraints = false;

			this.View.AddSubview (mapView);

			var dict = new NSMutableDictionary ();
			dict.Add ((NSString) "mainView", this.View);
			dict.Add ((NSString) "mapView", mapView);
			dict.Add ((NSString) "locationMessage", locationMessage);

			this.View.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-[mapView]-|",	NSLayoutFormatOptions.AlignAllTop, null, dict));
			this.View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:[locationMessage]-[mapView]-|", NSLayoutFormatOptions.AlignAllCenterX, null, dict));
			this.View.AddConstraints (new[] {
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, TitleLabel, NSLayoutAttribute.CenterX, 1, 0),
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AddLocationMessage ();
			AddMap ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (locationManager != null) {
				locationManager.StopUpdatingLocation ();
			}
		}

		protected override void RequestAccess ()
		{
			//Also note that info.plist has the NSLocationWhenInUseUsageDescription key
			//This call is asynchronous
			locationManager.RequestWhenInUseAuthorization ();
		}
	}
}

