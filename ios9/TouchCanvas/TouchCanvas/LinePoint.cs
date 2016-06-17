using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas {
	[Flags]
	public enum PointType {
		Standard = 0,
		Coalesced,
		Predicted,
		NeedsUpdate,
		Updated,
		Cancelled,
		Finger
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
				PointType = this.PointType.Add (PointType.NeedsUpdate);

			EstimationUpdateIndex = touch.EstimationUpdateIndex;
		}

		public bool UpdateWithTouch (UITouch touch)
		{
			NSNumber estimationUpdateIndex = touch.EstimationUpdateIndex;

			if (estimationUpdateIndex != EstimationUpdateIndex)
				return false;

			// An array of the touch properties that may be of interest.
			UITouchProperties[] touchProperties = {
				UITouchProperties.Location,
				UITouchProperties.Force,
				UITouchProperties.Altitude,
				UITouchProperties.Azimuth
			};

			// Iterate through possible properties.
			foreach (var expectedProperty in touchProperties) {
				// If an update to this property is not expected, continue to the next property.
				if (EstimatedPropertiesExpectingUpdates.Has (expectedProperty))
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

				if (!touch.EstimatedProperties.Has (expectedProperty)) {
					// Flag that this point now has a 'final' value for this property.
					EstimatedProperties = EstimatedProperties.Remove (expectedProperty);
				}

				if (!touch.EstimatedPropertiesExpectingUpdates.Has (expectedProperty)) {
					// Flag that this point is no longer expecting updates for this property.
					EstimatedPropertiesExpectingUpdates = EstimatedPropertiesExpectingUpdates.Remove (expectedProperty);

					if (EstimatedPropertiesExpectingUpdates == 0) {
						PointType = this.PointType.Remove (PointType.NeedsUpdate);
						PointType = this.PointType.Add (PointType.Updated);
					}
				}
			}

			return true;
		}
	}
}