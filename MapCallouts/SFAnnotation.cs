using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace MapCallouts
{
	public class SFAnnotation : MKAnnotation
	{
		public UIImage Image { get; set; }
		public double Latitute { get; set; }
		public double Longitude { get; set; }
		
		public override string Title { get { return "San Francisco"; } }
		public override string Subtitle { get { return "Founded: June 29, 1776"; } }

		public override CoreLocation.CLLocationCoordinate2D Coordinate {
			get {
				CLLocationCoordinate2D theCoordinate;
				theCoordinate.Latitude = 37.786996;
				theCoordinate.Longitude = -122.419281;
				return theCoordinate; 
			}
			set {
			}
		}
	}
}

