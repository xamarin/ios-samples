using System;
using System.Collections.Generic;

using CoreGraphics;

namespace TouchCanvas {
	public enum PointType : ulong {
		Standard = 0,
		Coalesced,
		Predicted,
		NeedsUpdate,
		Updated,
		Cancelled,
		Finger
	};

	public class LinePoint {

		public nfloat Force { get; set; }

		public CGPoint PreciseLocation { get; set; }

		public double Timestamp { get; set; }

		public CGPoint Location { get; set; }

		public List<PointType> Properties { get; private set; }

		public nfloat Magnitude => NMath.Max (Force, 0.025f);

		public LinePoint (double timestamp, nfloat force, CGPoint location, CGPoint preciseLocation, PointType type)
		{
			Force = force;
			PreciseLocation = preciseLocation;
			Timestamp = timestamp;
			Location = location;

			Properties = new List<PointType> { type };
		}
	}
}