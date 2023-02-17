using System;
using SceneKit;
using System.Collections;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using OpenTK;
using Foundation;

namespace ScanningAndDetecting3DObjects {
	internal static class PointCloud {
		internal static SCNGeometry CreateVisualization (NVector3 [] points, UIColor color, float size)
		{
			if (points.Length == 0) {
				return null;
			}

			unsafe {
				var stride = sizeof (float) * 3;

				// Pin the data down so that it doesn't move
				fixed (NVector3* pPoints = &points [0]) {
					// Grab a pointer to the data and treat it as a byte buffer of the appropriate length
					var intPtr = new IntPtr (pPoints);
					var pointData = NSData.FromBytes (intPtr, (System.nuint) (stride * points.Length));

					// Create a geometry source (factory) configured properly for the data (3 vertices)
					var source = SCNGeometrySource.FromData (
						pointData,
						SCNGeometrySourceSemantics.Vertex,
						points.Length,
						true,
						3,
						sizeof (float),
						0,
						stride
					);

					// Don't unpin yet, because geometry creation is lazy

					// Create geometry element
					// The null and bytesPerElement = 0 look odd, but this is just a template object
					var element = SCNGeometryElement.FromData (null, SCNGeometryPrimitiveType.Point, points.Length, 0);
					element.PointSize = 0.001F;
					element.MinimumPointScreenSpaceRadius = size;
					element.MaximumPointScreenSpaceRadius = size;

					// Stitch the data (source) together with the template (element) to create the new object
					var pointsGeometry = SCNGeometry.Create (new [] { source }, new [] { element });
					pointsGeometry.Materials = new [] { Utilities.Material (color) };
					return pointsGeometry;
				}
			}
		}
	}
}
