using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas {

	[Flags]
	public enum PointType {
		Standard = 0,
		Coalesced = 1 << 0,
		Predicted = 1 << 1,
		NeedsUpdate = 1 << 2,
		Updated = 1 << 3,
		Cancelled = 1 << 4,
		Finger = 1 << 5
	};

	public class LinePoint {

		public int SequenceNumber { get; private set; }

		public double Timestamp { get; set; }

		public nfloat Force { get; set; }

		public CGPoint Location { get; set; }

		public CGPoint PreciseLocation { get; set; }

		public PointType PointType { get; set; }

		public UITouchProperties EstimatedPropertiesExpectingUpdates { get; private set; }

		public UITouchProperties EstimatedProperties { get; private set; }

		public UITouchType Type { get; private set; }

		public nfloat AltitudeAngle { get; private set; }

		public nfloat AzimuthAngle { get; private set; }

		public NSNumber EstimationUpdateIndex { get; private set; }

		public nfloat Magnitude {
			get {
				return NMath.Max (Force, .025f);
			}
		}

		public LinePoint (UITouch touch, int sequenceNumber, PointType pointType)
		{
			SequenceNumber = sequenceNumber;
			Type = touch.Type;
			PointType = pointType;
			Timestamp = touch.Timestamp;

			var view = touch.View;
			Location = touch.LocationInView (view);
			PreciseLocation = touch.GetPreciseLocation (view);
			AzimuthAngle = touch.GetAzimuthAngle (view);

			EstimatedProperties = touch.EstimatedProperties;
			EstimatedPropertiesExpectingUpdates = touch.EstimatedPropertiesExpectingUpdates;
			AltitudeAngle = touch.AltitudeAngle;
			Force = (Type == UITouchType.Stylus || touch.Force > 0) ? touch.Force : 1f;

			if (EstimatedPropertiesExpectingUpdates != 0)
				PointType |= PointType.NeedsUpdate;

			EstimationUpdateIndex = touch.EstimationUpdateIndex;
		}

		public bool UpdateWithTouch (UITouch touch)
		{
			if (!touch.EstimationUpdateIndex.IsEqualTo (EstimationUpdateIndex))
				return false;

			// An array of the touch properties that may be of interest.
			UITouchProperties [] touchProperties = {
				UITouchProperties.Location,
				UITouchProperties.Force,
				UITouchProperties.Altitude,
				UITouchProperties.Azimuth
			};

			// Iterate through possible properties.
			foreach (var expectedProperty in touchProperties) {
				// If an update to this property is not expected, continue to the next property.
				if (EstimatedPropertiesExpectingUpdates.HasFlag (expectedProperty))
					continue;

				switch (expectedProperty) {
				case UITouchProperties.Force:
					Force = touch.Force;
					break;
				case UITouchProperties.Azimuth:
					AzimuthAngle = touch.GetAzimuthAngle (touch.View);
					break;
				case UITouchProperties.Altitude:
					AltitudeAngle = touch.AltitudeAngle;
					break;
				case UITouchProperties.Location:
					Location = touch.LocationInView (touch.View);
					PreciseLocation = touch.PreviousLocationInView (touch.View);
					break;
				}

				// Flag that this point now has a 'final' value for this property.
				if (!touch.EstimatedProperties.HasFlag (expectedProperty))
					EstimatedProperties &= ~expectedProperty;

				// Flag that this point is no longer expecting updates for this property.
				if (!touch.EstimatedPropertiesExpectingUpdates.HasFlag (expectedProperty)) {
					EstimatedPropertiesExpectingUpdates &= ~expectedProperty;

					if (EstimatedPropertiesExpectingUpdates == 0) {
						PointType &= ~PointType.NeedsUpdate;
						PointType |= PointType.Updated;
					}
				}
			}

			return true;
		}
	}
}
