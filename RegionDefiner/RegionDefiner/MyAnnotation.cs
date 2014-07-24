using System;
using CoreGraphics;

using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace RegionDefiner
{
	public class MyAnnotation : MKAnnotation
	{
		string title;
		string subtitle ;

		public MyAnnotation (CLLocationCoordinate2D coordinate, string _title, string _subtitle)
		{
			Coordinate = coordinate;
			title = _title;
			subtitle = _subtitle;
		}
	
		public override string Title {
			get {
				return title;
			}
		}

		public override string Subtitle {
			get {
				return subtitle;
			}
		}

		public override CLLocationCoordinate2D Coordinate { get; set; }
	}
}

