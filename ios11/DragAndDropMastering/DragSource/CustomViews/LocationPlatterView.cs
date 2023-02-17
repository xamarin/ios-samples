using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;

namespace DragSource {
	/**
	 A LocationPlatterView is a view of fixed size
	 that is capable of representing a given image
	 and location.
	 */
	public class LocationPlatterView : UIView {
		#region Constructors
		public LocationPlatterView ()
		{
		}

		public LocationPlatterView (NSCoder coder) : base (coder)
		{
		}

		public LocationPlatterView (UIImage image, MKMapItem item) : base (new CGRect (0, 0, 200, 180))
		{
			var imageView = new UIImageView (image);

			NSLayoutConstraint.ActivateConstraints (new NSLayoutConstraint [] {
				imageView.WidthAnchor.ConstraintEqualTo(120),
				imageView.HeightAnchor.ConstraintEqualTo(120)
			});
			imageView.Layer.CornerRadius = 60;
			imageView.Layer.MasksToBounds = true;

			var cityNameLabel = new UILabel () {
				Text = (item.Placemark.Locality == null) ? $"Longitude {item.Placemark.Coordinate.Longitude}" : item.Placemark.Locality,
				Font = UIFont.BoldSystemFontOfSize (16),
				Lines = 1
			};

			var countryNameLabel = new UILabel () {
				Text = (item.Placemark.Country == null) ? $"Latitude {item.Placemark.Coordinate.Latitude}" : item.Placemark.Country,
				Font = UIFont.BoldSystemFontOfSize (14),
				Lines = 1
			};

			var container = new UIStackView (new UIView [] { imageView, cityNameLabel, countryNameLabel }) {
				Alignment = UIStackViewAlignment.Center,
				Axis = UILayoutConstraintAxis.Vertical,
				ClipsToBounds = true,
				Frame = Bounds
			};

			AddSubview (container);

		}
		#endregion
	}
}
