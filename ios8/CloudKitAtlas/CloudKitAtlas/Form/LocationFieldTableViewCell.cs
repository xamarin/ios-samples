using System;

using UIKit;
using Foundation;
using CoreLocation;

namespace CloudKitAtlas
{
	public partial class LocationFieldTableViewCell : FormFieldTableViewCell
	{
		[Outlet]
		public UIButton LookUpButton { get; set; }

		[Outlet]
		public UITextField LatitudeField { get; set; }

		[Outlet]
		public UITextField LongitudeField { get; set; }

		[Outlet]
		public UIActivityIndicatorView Spinner { get; set; }

		[Outlet]
		public UILabel ErrorLabel { get; set; }

		public LocationInput LocationInput { get; set; }

		public LocationFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		public LocationFieldTableViewCell (NSCoder coder)
			: base (coder)
		{
		}

		public void SetCoordinate (CLLocationCoordinate2D coordinate)
		{
			var latitude = coordinate.Latitude;
			var longitude = coordinate.Longitude;

			LocationInput.Latitude = (int)latitude;
			LocationInput.Longitude = (int)longitude;

			LatitudeField.Text = LocationInput.Latitude.ToString ();
			LongitudeField.Text = LocationInput.Longitude.ToString ();

			LatitudeField.LayoutIfNeeded ();
			LongitudeField.LayoutIfNeeded ();
		}
	}
}
