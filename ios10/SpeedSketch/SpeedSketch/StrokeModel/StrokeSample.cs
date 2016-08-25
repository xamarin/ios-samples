using System;

using UIKit;
using CoreGraphics;

using static CoreGraphics.CGAffineTransform;

namespace SpeedSketch
{
	public class StrokeSample
	{
		// From the Apple's doc:
		// A touch object persists throughout a multi-touch sequence.
		// Never retain a touch object when handling an event.
		// If you need to keep information about a touch from one touch phase to another, copy that information from the touch

		#region UITouch properties

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

		#endregion

		// Values for debug display.
		public bool Coalesced { get; set; }
		public bool Predicted { get; set; }

		public CGVector GetAzimuthUnitVector ()
		{
			var unitVector = new CGVector (1, 1);
			return unitVector.Apply (MakeRotation (Azimuth.Value));
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
