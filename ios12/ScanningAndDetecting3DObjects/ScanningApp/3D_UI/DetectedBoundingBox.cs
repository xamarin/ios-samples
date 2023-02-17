using System;
using System.Collections.Generic;
using OpenTK;
using SceneKit;
using UIKit;

namespace ScanningAndDetecting3DObjects {
	// A simple visualization of a 3D bounding box, used when testing detection of a scanned object
	internal class DetectedBoundingBox : SCNNode {
		internal DetectedBoundingBox (NVector3 [] points, double scale, UIColor color = null) : base ()
		{
			// Cannot use asset-group defined color as default in args (since not compile-time constant), 
			// so assign it now if default
			if (color == null) {
				color = Utilities.AppYellow;
			}

			var localMin = new SCNVector3 (float.MaxValue, float.MaxValue, float.MaxValue);
			var localMax = new SCNVector3 (float.MinValue, float.MinValue, float.MinValue);

			foreach (var point in points) {
				var scnVector = point.ToSCNVector3 ();
				localMin = localMin.Min (scnVector);
				localMax = localMax.Max (scnVector);
			}

			Position = Position + (localMax + localMin) / 2;
			var extent = localMax - localMin;
			var wireframe = new Wireframe (extent.ToNVector3 (), color, scale);
			AddChildNode (wireframe);
		}
	}
}
