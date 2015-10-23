using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas {
	public class Line : NSObject {
	
		List<LinePoint> points = new List<LinePoint> ();

		public bool IsComplete => points.FirstOrDefault (p => p.Properties.Contains (PointType.NeedsUpdate)) == null;

		public CGRect AddPointAtLocation (CGPoint location, CGPoint preciseLocation, nfloat force, double timestamp, PointType type)
		{
			var point = new LinePoint (timestamp, force, location, preciseLocation, type);
			var updateRect = UpdateRectForLinePoint (point);

			var last = points.LastOrDefault ();
			if (last != null) {
				var lastRect = UpdateRectForLinePoint (last);
				updateRect.UnionWith (lastRect);
			}

			points.Add (point);
			return updateRect;
		}

		public CGRect RemovePointsWithType (PointType type)
		{
			var updateRect = CGRect.Empty;
			LinePoint priorPoint = null;
			var keepPoints = new List<LinePoint> ();

			foreach (var point in points) {
				var keepPoint = !point.Properties.Contains (type);

				if (!keepPoint) {
					var rect = UpdateRectForLinePoint (point);

					if (priorPoint != null)
						rect.UnionWith (UpdateRectForLinePoint (priorPoint));

					updateRect.UnionWith (rect);
				}

				priorPoint = point;

				if (keepPoint)
					keepPoints.Add (point);
			}

			points = keepPoints;
			return updateRect;
		}

		public void DrawInContext (CGContext context, bool isDebuggingEnabled, bool usePreciseLocation)
		{
			LinePoint maybePriorPoint = null;

			foreach (var point in points) {
				if (maybePriorPoint == null) {
					maybePriorPoint = point;
					continue;
				}

				var priorPoint = maybePriorPoint;

				var color = UIColor.Black;

				if (isDebuggingEnabled) {
					if (point.Properties.Contains (PointType.Cancelled))
						color = UIColor.Red;
					else if (point.Properties.Contains (PointType.NeedsUpdate))
						color = UIColor.Orange;
					else if (point.Properties.Contains (PointType.Finger))
						color = UIColor.Purple;
					else if (point.Properties.Contains (PointType.Coalesced))
						color = UIColor.Green;
					else if (point.Properties.Contains (PointType.Predicted))
						color = UIColor.Blue;
				} else {
					if (point.Properties.Contains (PointType.Cancelled))
						color = UIColor.Clear;
					else if (point.Properties.Contains (PointType.Finger))
						color = UIColor.Purple;
					if (point.Properties.Contains (PointType.Predicted) && !point.Properties.Contains (PointType.Cancelled))
						color = color.ColorWithAlpha (0.5f);
				}

				var location = usePreciseLocation ? point.PreciseLocation : point.Location;
				var priorLocation = usePreciseLocation ? priorPoint.PreciseLocation : priorPoint.Location;

				context.SetStrokeColor (color.CGColor);
				context.BeginPath ();
				context.MoveTo (priorLocation.X, priorLocation.Y);
				context.AddLineToPoint (location.X, location.Y);
				context.SetLineWidth (point.Magnitude);
				context.StrokePath ();

				maybePriorPoint = point;
			}
		}

		public CGRect Cancel ()
		{
			CGRect updateRect = CGRect.Empty;
			foreach (var point in points) {
				point.Properties.Add (PointType.Cancelled);
				updateRect = updateRect.UnionWith (UpdateRectForLinePoint (point));
			}

			return updateRect;
		}

		public CGRect UpdatePointLocation (CGPoint location, CGPoint preciseLocation, nfloat force, double timestamp)
		{
			var updateRect = CGRect.Empty;

			foreach (var point in points) {
				if (updateRect != CGRect.Empty) {
					updateRect.UnionWith (UpdateRectForLinePoint (point));
					break;
				}

				if (Math.Abs (point.Timestamp - timestamp) < double.Epsilon) {
					var oldRect = UpdateRectForLinePoint (point);

					point.Location = location;
					point.PreciseLocation = preciseLocation;
					point.Force = force;
					point.Properties.Add (PointType.Updated);
					point.Properties.Remove (PointType.NeedsUpdate);

					var newRect = UpdateRectForLinePoint (point);
					updateRect = oldRect.UnionWith (newRect);
				}
			}

			return updateRect;
		}

		public void DrawFixedPointsInContext (CGContext context, bool isDebuggingEnabled, bool usePreciseLocation)
		{
			var allPointsCount = points.Count;
			var linePoints = new List<LinePoint> ();

			for (int i = 0; i < allPointsCount; i++) {
				var point = points[i];

				if (!point.Properties.Any (p => p == PointType.NeedsUpdate || p == PointType.Predicted)) {
					linePoints.Add (points.First ());
					break;
				}

				if (i > 0)
					continue;

				linePoints.Add (points.First ());
				points.RemoveAt (0);
			}

			if (linePoints.Count > 1)
				return;

			var committedLine = new Line {
				points = linePoints
			};

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
	}
}