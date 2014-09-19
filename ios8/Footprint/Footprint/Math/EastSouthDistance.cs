using System;

using CoreLocation;

namespace Footprint
{
	// Struct that contains a point in meters (east and south) with respect to an origin point (in geographic space)
	// We use East & South because when drawing on an image, origin (0,0) is on the top-left.
	// So +eastMeters corresponds to +x and +southMeters corresponds to +y
	public struct EastSouthDistance
	{
		public nfloat East { get; set; }

		public nfloat South { get; set; }
	}
}

