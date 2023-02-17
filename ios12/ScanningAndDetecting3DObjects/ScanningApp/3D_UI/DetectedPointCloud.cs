using ARKit;
using OpenTK;
using SceneKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanningAndDetecting3DObjects {
	internal class DetectedPointCloud : SCNNode {
		private ARPointCloud ReferenceObjectPointCloud { get; }
		private SCNVector3 Center { get; }
		private SCNVector3 Extent { get; }
		internal DetectedPointCloud (ARPointCloud referenceObjectPointCloud, NVector3 center, NVector3 extent)
		{
			ReferenceObjectPointCloud = referenceObjectPointCloud;
			Center = center.ToSCNVector3 ();
			Extent = extent.ToSCNVector3 ();

			base.Init ();

			// Semitransparently visualize the reference object's points.
			var referenceObjectPoints = new SCNNode ();
			referenceObjectPoints.Geometry = PointCloud.CreateVisualization (referenceObjectPointCloud.Points, Utilities.AppYellow, 12);
			AddChildNode (referenceObjectPoints);
		}

		internal void UpdateVisualization (ARPointCloud currentPointCloud)
		{
			Func<double, double, double, bool> RangeContains = (low, high, val) => val >= low && val <= high;
			if (Hidden) {
				return;
			}

			var min = Position + Center - Extent / 2;
			var max = Position + Center + Extent / 2;
			var inlierPoints = new List<SCNVector3> ();

			foreach (var point in currentPointCloud.Points) {
				var localPoint = ConvertPositionFromNode (point.ToSCNVector3 (), null);
				if (RangeContains (min.X, max.X, localPoint.X)
				   && RangeContains (min.Y, max.Y, localPoint.Y)
					&& RangeContains (min.Z, max.Z, localPoint.Z)) {
					inlierPoints.Add (localPoint);
				}
			}

			var currentPointCloudInliers = inlierPoints.Select (s => s.ToNVector3 ()).ToArray ();
			Geometry?.Dispose ();
			Geometry = PointCloud.CreateVisualization (currentPointCloudInliers, Utilities.AppGreen, 12);
		}
	}
}
