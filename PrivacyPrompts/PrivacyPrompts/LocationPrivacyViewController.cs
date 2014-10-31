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

		public LocationPrivacyViewController () : base(null, null)
		{
			CheckAccess = LocationAccessStatus;
			RequestAccess = RequestLocationServicesAuthorization;

			locationManager = new CLLocationManager ();
			//If previously allowed, start location manager
			if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
			{
				locationManager.StartUpdatingLocation();
			}

			locationManager.Failed += delegate {
				locationManager.StopUpdatingLocation ();
			};
			locationManager.LocationsUpdated += delegate {
				var loc = locationManager.Location.ToString();
				locationMessage.Text = loc;

				//MapView
				MKCoordinateRegion region = new MKCoordinateRegion(locationManager.Location.Coordinate, new MKCoordinateSpan(0.1, 0.1));
				mapView.SetRegion(region, true);
			};
			locationManager.AuthorizationChanged += delegate (object sender, CLAuthorizationChangedEventArgs e) {
				accessStatus.Text = e.Status.ToString();
				if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
				{	
					mapView.ShowsUserLocation = true;
					locationManager.StartUpdatingLocation ();
				}
			};
		}

	 	string LocationAccessStatus() {
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
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, requestAccessButton, NSLayoutAttribute.Bottom, 1, 40),
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
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (locationMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, locationMessage, NSLayoutAttribute.Bottom, 1, 40),
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

		void RequestLocationServicesAuthorization ()
		{
			//Also note that info.plist has the NSLocationWhenInUseUsageDescription key 
			//This call is asynchronous
			locationManager.RequestWhenInUseAuthorization ();
		}
	}
}

