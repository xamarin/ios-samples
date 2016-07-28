using System;

using UIKit;
using CoreGraphics;

using static CoreGraphics.CGAffineTransform;

namespace SpeedSketch
{
	public class StrokeSample
	{
		public double Timestamp { get; set; }
		public CGPoint Location { get; set; }

		// 3D Touch or Pencil.
		// TODO: do we need nullable here?
		public nfloat? Force { get; set; }

		// Pencil only.
		public UITouchProperties EstimatedProperties { get; set; } = new UITouchProperties ();
		public UITouchProperties EstimatedPropertiesExpectingUpdates { get; set; } = new UITouchProperties ();
		public nfloat? Altitude { get; set; }
		public nfloat? Azimuth { get; set; }

		// Values for debug display.
		public bool Coalesced { get; set; }
		public bool Predicted { get; set; }

		public CGVector GetAzimuthUnitVector ()
		{
			return new CGVector (1, 1).Apply (MakeRotation (Azimuth.Value));
		}

		// Convenience accessor returns a non-nullable (Default: 1)
		public nfloat ForceWithDefault ()
		{
			var f = Force;
			return f.HasValue ? f.Value : 1;
		}

		// Returns the force perpendicular to the screen. The regular stylus force is along the pencil axis.
		public nfloat PerpendicularForce ()
		{
			var force = ForceWithDefault ();
			var altitude = Altitude;
			return altitude.HasValue
				? force / NMath.Sin (altitude.Value)
				: force;
		}
	}
}
