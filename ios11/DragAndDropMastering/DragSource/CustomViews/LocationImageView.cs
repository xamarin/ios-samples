using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;

namespace DragSource {
	/**
	 A LocationImageView is a UIImageView that also contains
	 an MKMapItem. Upon initialization with a CLLocation, it
	 requests a map item via reverse geocoding.
	 */
	public class LocationImageView : UIImageView {
		#region Computed Properties
		public MKMapItem MapItem { get; set; }
		#endregion

		#region Constructors
		public LocationImageView ()
		{
		}

		public LocationImageView (NSCoder coder) : base (coder)
		{
		}

		public LocationImageView (UIImage image, CLLocation location) : base (image)
		{
			location.LoadMapItem ((mapItem, error) => {
				if (error == null) {
					MapItem = mapItem;
				}
			});
		}
		#endregion
	}
}
