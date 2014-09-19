using System;
using CoreLocation;
using CoreGraphics;

namespace Footprint
{
	public struct GeoAnchor
	{
		public CLLocationCoordinate2D LatitudeLongitude { get; set; }

		public CGPoint Pixel { get; set; }
	}
}

