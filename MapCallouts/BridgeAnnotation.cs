using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace MapCallouts
{
	public class BridgeAnnotation : MKAnnotation
	{
		public override string Title { get { return "Golden Gate Bridge"; } }
		public override string Subtitle { get { return "Opened: May 27, 1937"; } }

		public override CoreLocation.CLLocationCoordinate2D Coordinate {
			get {
				CLLocationCoordinate2D theCoordinate;
				theCoordinate.Latitude = 37.810000;
				theCoordinate.Longitude = -122.477989;
				return theCoordinate; 
			}
			set {
			}
		}
	}
}

