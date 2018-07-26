using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using ImageIO;
using MapKit;
using CoreLocation;

namespace CoreLocation
{
	public static class CLLocationExtensions
	{
		public static void LoadMapItem(this CLLocation self, MapItemLoadCompletionHandler completionHandler) {
			var geocoder = new CLGeocoder();

			geocoder.ReverseGeocodeLocation(self, (placemarks, error) => {
                if (placemarks == null)
                {
                    completionHandler(null, error); 
                    return;
                }
                if (placemarks.Length == 0)
                {
                    completionHandler(null, error);
                    return;
                }
				var mkPlacement = new MKPlacemark(placemarks[0].Location.Coordinate);
				completionHandler(new MKMapItem(mkPlacement), null);
			});
		}
	}

	public delegate void MapItemLoadCompletionHandler(MKMapItem mapItem, NSError error);
}
