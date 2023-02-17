using System;
using CoreGraphics;
using OpenTK;
using ARKit;
using SceneKit;

namespace ScanningAndDetecting3DObjects {
	internal static class ARSCNView_Extensions {
		internal static SCNVector3? UnprojectPointLocal (this ARSCNView self, CGPoint point, NMatrix4 planeTransform)
		{
			var result = self.Unproject (point, planeTransform);
			// Convert the result into the plane's local coordinate system. 
			var pt = new SCNVector4 (result.X, result.Y, result.Z, 1);
			var invertedPlane = planeTransform.ToSCNMatrix4 ();
			invertedPlane.Invert ();
			var localResult = invertedPlane.Times (pt);
			return localResult.Xyz;
		}

		internal static ARHitTestResult SmartHitTest (this ARSCNView self, CGPoint point)
		{
			var hitTestResults = self.HitTest (point, ARHitTestResultType.FeaturePoint);
			if (hitTestResults == null || hitTestResults.Length == 0) {
				return null;
			}

			foreach (var result in hitTestResults) {
				// Return the first result which is between 20 cm and 3 m away from the user.
				if (result.Distance > 0.2 && result.Distance < 3) {
					return result;
				}
			}
			return null;
		}
	}
}
