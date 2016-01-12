using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas {
	public class Line : NSObject {
		readonly Dictionary<NSNumber,LinePoint> pointsWaitingForUpdatesByEstimationIndex = new Dictionary<NSNumber, LinePoint> ();

		public List<LinePoint> Points { get; private set; } = new List<LinePoint> ();

		public List<LinePoint> CommittedPoints { get; private set; } = new List<LinePoint> ();

		public bool IsComplete {
			get {
				return pointsWaitingForUpdatesByEstimationIndex.Count == 0;
			}
		}

		public KeyValuePair<bool, CGRect> UpdateWithTouch (UITouch touch)
		{
			var estimationUpdateIndex = touch.EstimationUpdateIndex;
			var point = pointsWaitingForUpdatesByEstimationIndex[estimationUpdateIndex];
			if (point != null) {
				var rect = UpdateRectForExistingPoint (point);
				var didUpdate = point.UpdateWithTouch (touch);
				if (didUpdate)
					rect = rect.UnionWith (UpdateRectForExistingPoint (point));

				if (point.EstimatedPropertiesExpectingUpdates == 0) {
					pointsWaitingForUpdatesByEstimationIndex.Remove (estimationUpdateIndex);
				}

				return new KeyValuePair<bool, CGRect> (didUpdate, rect);
			}

			return new KeyValuePair<bool, CGRect> (false, CGRect.Empty);
		}

		public CGRect AddPointOfType (PointType pointType, UITouch touch)
		{
			var previousPoint = Points.LastOrDefault ();
			var previousSequenceNumber = previousPoint != null ? previousPoint.SequenceNumber : -1;
			var point = new LinePoint (touch, previousSequenceNumber + 1, pointType);

			if (point.EstimationUpdateIndex != null && point.EstimatedPropertiesExpectingUpdates != 0)
				pointsWaitingForUpdatesByEstimationIndex [point.EstimationUpdateIndex] = point;

			Points.Add (point);
			return UpdateRectForLinePoint (point, previousPoint);
		}

		public CGRect RemovePointsWithType (PointType type)
		{
			var updateRect = CGRect.Empty;
			LinePoint priorPoint = null;
			var keepPoints = new List<LinePoint> ();

			foreach (var point in Points) {
				var keepPoint = !point.PointType.Has (type);

				if (!keepPoint) {
					var rect = UpdateRectForLinePoint (point);
					if (priorPoint != null)
						rect = rect.UnionWith (UpdateRectForLinePoint (priorPoint));
					updateRect = updateRect.UnionWith (rect);
				} else {
					keepPoints.Add (point);
				}

				priorPoint = point;
			}

			Points = keepPoints;
			return updateRect;
		}

		public CGRect Cancel ()
		{
			CGRect updateRect = CGRect.Empty;
			foreach (var point in Points) {
				point.PointType = point.PointType.Add<PointType> (PointType.Cancelled);
				updateRect = updateRect.UnionWith (UpdateRectForLinePoint (point));
			}

			return updateRect;
		}

		public void DrawInContext (CGContext context, bool isDebuggingEnabled, bool usePreciseLocation)
		{
			LinePoint maybePriorPoint = null;

			foreach (var point in Points) {
				if (maybePriorPoint == null) {
					maybePriorPoint = point;
					continue;
				}

				var priorPoint = maybePriorPoint;

				var color = UIColor.Black;
				var pointType = point.PointType;

				if (isDebuggingEnabled) {
					if (pointType.Has (PointType.Cancelled))
						color = UIColor.Red;
					else if (pointType.Has (PointType.NeedsUpdate))
						color = UIColor.Orange;
					else if (pointType.Has (PointType.Finger))
						color = UIColor.Purple;
					else if (pointType.Has (PointType.Coalesced))
						color = UIColor.Green;
					else if (pointType.Has (PointType.Predicted))
						color = UIColor.Blue;
				} else {
					if (pointType.Has (PointType.Cancelled))
						color = UIColor.Red;
					else if (pointType.Has (PointType.Finger))
						color = UIColor.Purple;
					
					if (pointType.Has (PointType.Predicted) && !pointType.Has (PointType.Cancelled))
						color = color.ColorWithAlpha (.5f);
				}

				var location = usePreciseLocation ? point.PreciseLocation : point.Location;
				var priorLocation = usePreciseLocation ? priorPoint.PreciseLocation : priorPoint.Location;

				context.SetStrokeColor (color.CGColor);
				context.BeginPath ();
				context.MoveTo (priorLocation.X, priorLocation.Y);
				context.AddLineToPoint (location.X, location.Y);
				context.SetLineWidth (point.Magnitude);
				context.StrokePath ();

				// Draw azimuith and elevation on all non-coalesced points when debugging.
				if (isDebuggingEnabled &&
					!point.PointType.Has (PointType.Coalesced) &&
					!point.PointType.Has (PointType.Predicted) &&
					!point.PointType.Has (PointType.Finger)) {
					context.BeginPath ();
					context.SetStrokeColor (UIColor.Red.CGColor);
					context.SetLineWidth (.5f);
					context.MoveTo (location.X, location.Y);
					var targetPoint = new CGPoint (.5f + 10f * NMath.Cos (point.AltitudeAngle), 0f);
					targetPoint = CGAffineTransform.MakeRotation (point.AzimuthAngle).TransformPoint (targetPoint);
					targetPoint.X += location.X;
					targetPoint.Y += location.Y;
					context.AddLineToPoint (targetPoint.X, targetPoint.Y);
					context.StrokePath ();
				}

				maybePriorPoint = point;
			}
		}

		public void DrawFixedPointsInContext (CGContext context, bool isDebuggingEnabled, bool usePreciseLocation, bool commitAll = false)
		{
			var allPoints = new List<LinePoint> (Points);
			var committing = new List<LinePoint> ();

			if (commitAll) {
				committing = allPoints;
				Points.Clear ();
			} else {
				for (int i = 0; i < allPoints.Count; i++) {
					var point = allPoints[i];
					if ((point.PointType.Has (PointType.NeedsUpdate) ||
						point.PointType.Has (PointType.Predicted)) && i > (allPoints.Count - 2)) {
						committing.Add (Points.First ());
						break;
					}

					if (i <= 0)
						continue;

					var removed = Points.First ();
					Points.Remove (removed);
					committing.Add (removed);
				}
			}

			if (committing.Count <= 1)
				return;

			var committedLine = new Line {
				Points = committing
			};

			committedLine.DrawInContext (context, isDebuggingEnabled, usePreciseLocation);

			if (CommittedPoints.Count > 0)
				CommittedPoints.Remove (CommittedPoints.Last ());

			// Store the points being committed for redrawing later in a different style if needed.
			CommittedPoints.AddRange (committing);
		}

		public void DrawCommitedPointsInContext (CGContext context, bool isDebuggingEnabled, bool usePreciseLocation)
		{
			var committedLine = new Line ();
			committedLine.Points = CommittedPoints;
			committedLine.DrawInContext (context, isDebuggingEnabled, usePreciseLocation);
		}

		static CGRect UpdateRectForLinePoint (LinePoint point)
		{
			var rect = new CGRect (point.Location, CGSize.Empty);

			// The negative magnitude ensures an outset rectangle
			var magnitude = -3 * point.Magnitude - 2;
			rect = rect.Inset (magnitude, magnitude);

			return rect;
		}

		static CGRect UpdateRectForLinePoint (LinePoint point, LinePoint previousPoint)
		{
			var rect = new CGRect (point.Location, CGSize.Empty);
			var pointMagnitude = point.Magnitude;

			if (previousPoint != null) {
				pointMagnitude = NMath.Max (pointMagnitude, previousPoint.Magnitude);
				rect = rect.UnionWith (new CGRect (previousPoint.Location, CGSize.Empty));
			}

			// The negative magnitude ensures an outset rectangle.
			var magnitude = -3 * pointMagnitude - 2;
			rect = rect.Inset (magnitude, magnitude);

			return rect;
		}

		CGRect UpdateRectForExistingPoint (LinePoint point)
		{
			var rect = UpdateRectForLinePoint (point);
			var arrayIndex = point.SequenceNumber - Points.First ().SequenceNumber;

			if (arrayIndex > 0 && arrayIndex + 1 < Points.Count)
				rect = rect.UnionWith (UpdateRectForLinePoint (point, Points[arrayIndex - 1]));

			if (arrayIndex + 1 < Points.Count && arrayIndex + 1 > 0)
				rect = rect.UnionWith (UpdateRectForLinePoint (point, Points[arrayIndex + 1]));

			return rect;
		}
	}
}