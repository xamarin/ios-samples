using ARKit;
using CoreGraphics;
using SceneKit;
using System.Linq;

namespace EnvironmentTexturing {
	public static class CGPointExtensions {
		/// <summary>
		/// Extracts the screen space point from a vector returned by SCNView.projectPoint(_:).
		/// </summary>
		public static CGPoint Create (this SCNVector3 vector)
		{
			return new CGPoint (vector.X, vector.Y);
		}
	}

	public static class ARSCNViewExtensions {
		public static ARHitTestResult SmartHitTest (this ARSCNView self, CGPoint point)
		{
			// Perform the hit test.
			var results = self.HitTest (point, ARHitTestResultType.ExistingPlaneUsingGeometry);

			// 1. Check for a result on an existing plane using geometry.
			var existingPlaneUsingGeometryResult = results.FirstOrDefault (result => result.Type == ARHitTestResultType.ExistingPlaneUsingGeometry);
			if (existingPlaneUsingGeometryResult != null) {
				return existingPlaneUsingGeometryResult;
			}

			// 2. Check for a result on an existing plane, assuming its dimensions are infinite.
			var infinitePlaneResults = self.HitTest (point, ARHitTestResultType.ExistingPlane);

			var infinitePlaneResult = infinitePlaneResults.FirstOrDefault ();
			if (infinitePlaneResult != null) {
				return infinitePlaneResult;
			}

			// 3. As a final fallback, check for a result on estimated planes.
			return results.FirstOrDefault (result => result.Type == ARHitTestResultType.EstimatedHorizontalPlane);
		}
	}

	public static class NNodeExtensions {
		public static SCNVector3 GetExtents (this SCNNode self)
		{
			var min = default (SCNVector3);
			var max = default (SCNVector3);
			self.GetBoundingBox (ref min, ref max);

			return max - min;
		}
	}

	public static class NMatrix4Extensions {
		public static OpenTK.NMatrix4 CreateTranslation (SCNVector3 vector)
		{
			return new OpenTK.NMatrix4 (new OpenTK.Vector4 (1, 0, 0, vector.X),
									   new OpenTK.Vector4 (0, 1, 0, vector.Y),
									   new OpenTK.Vector4 (0, 0, 1, vector.Z),
									   new OpenTK.Vector4 (0, 0, 0, 1));
		}

		public static SCNVector3 GetTranslation (this OpenTK.NMatrix4 self)
		{
			var translation = self.Column3;
			return new SCNVector3 (translation.X, translation.Y, translation.Z);
			//return new SCNVector3(self.Column0.Z, self.Column1.Z, self.Column2.Z);
		}
	}
}
