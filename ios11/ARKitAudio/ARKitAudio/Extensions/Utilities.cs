
namespace ARKitAudio {
	using ARKit;
	using CoreGraphics;
	using SceneKit;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Utility functions and type extensions used throughout the projects.
	/// </summary>
	public static class Utilities {
		public static SCNVector3? GetAverage (this IList<SCNVector3> vectors)
		{
			SCNVector3? result = null;

			if (vectors != null && vectors.Count > 0) {
				var sum = vectors.Aggregate (SCNVector3.Zero, (vector1, vector2) => vector1 + vector2);
				result = sum / vectors.Count;
			}

			return result;
		}

		public static void KeepLast<T> (this List<T> list, int elementsToKeep)
		{
			if (list.Count > elementsToKeep) {
				list.RemoveRange (0, list.Count - elementsToKeep);
			}
		}

		/// <summary>
		/// Treats matrix as a (right-hand column-major convention) transform matrix
		/// and factors out the translation component of the transform.
		/// </summary>
		public static OpenTK.Vector3 GetTranslation (this OpenTK.NMatrix4 matrix)
		{
			var translation = matrix.Column3;
			return new OpenTK.Vector3 (translation.X, translation.Y, translation.Z);
		}

		#region Math

		public static SCNVector3? RayIntersectionWithHorizontalPlane (SCNVector3 rayOrigin, SCNVector3 direction, float planeY)
		{
			direction = SCNVector3.Normalize (direction);

			// Special case handling: Check if the ray is horizontal as well.
			if (direction.Y == 0) {
				if (rayOrigin.Y == planeY) {
					// The ray is horizontal and on the plane, thus all points on the ray intersect with the plane.
					// Therefore we simply return the ray origin.
					return rayOrigin;
				} else {
					// The ray is parallel to the plane and never intersects.
					return null;
				}
			}

			// The distance from the ray's origin to the intersection point on the plane is:
			//   (pointOnPlane - rayOrigin) dot planeNormal
			//  --------------------------------------------
			//          direction dot planeNormal

			// Since we know that horizontal planes have normal (0, 1, 0), we can simplify this to:
			var dist = (planeY - rayOrigin.Y) / direction.Y;

			// Do not return intersections behind the ray's origin.
			if (dist < 0) {
				return null;
			}

			// Return the intersection point.
			return rayOrigin + (direction * dist);
		}

		public static (SCNVector3?, ARPlaneAnchor, bool) WorldPositionFromScreenPosition (CGPoint position,
																						 ARSCNView sceneView,
																						 SCNVector3? objectPos,
																						 bool infinitePlane = false)
		{
			// -------------------------------------------------------------------------------
			// 1. Always do a hit test against existing plane anchors first.
			//    (If any such anchors exist & only within their extents.)

			var result = sceneView.HitTest (position, ARHitTestResultType.ExistingPlaneUsingExtent)?.FirstOrDefault ();
			if (result != null) {
				var planeHitTestPosition = result.WorldTransform.GetTranslation ();
				var planeAnchor = result.Anchor;

				// Return immediately - this is the best possible outcome.
				return (planeHitTestPosition, planeAnchor as ARPlaneAnchor, true);
			}

			// -------------------------------------------------------------------------------
			// 2. Collect more information about the environment by hit testing against
			//    the feature point cloud, but do not return the result yet.

			SCNVector3? featureHitTestPosition = null;
			var highQualityFeatureHitTestResult = false;

			var highQualityfeatureHitTestResults = sceneView.HitTestWithFeatures (position, 18f, 0.2f, 2f);
			if (highQualityfeatureHitTestResults.Any ()) {
				featureHitTestPosition = highQualityfeatureHitTestResults [0].Position;
				highQualityFeatureHitTestResult = true;
			}

			// -------------------------------------------------------------------------------
			// 3. If desired or necessary (no good feature hit test result): Hit test
			//    against an infinite, horizontal plane (ignoring the real world).

			if (infinitePlane || !highQualityFeatureHitTestResult) {
				if (objectPos.HasValue) {
					var pointOnInfinitePlane = sceneView.HitTestWithInfiniteHorizontalPlane (position, objectPos.Value);
					if (pointOnInfinitePlane.HasValue) {
						return (pointOnInfinitePlane, null, true);
					}
				}
			}

			// -------------------------------------------------------------------------------
			// 4. If available, return the result of the hit test against high quality
			//    features if the hit tests against infinite planes were skipped or no
			//    infinite plane was hit.

			if (highQualityFeatureHitTestResult) {
				return (featureHitTestPosition, null, false);
			}

			// -------------------------------------------------------------------------------
			// 5. As a last resort, perform a second, unfiltered hit test against features.
			//    If there are no features in the scene, the result returned here will be nil.

			var unfilteredFeatureHitTestResults = sceneView.HitTestWithFeatures (position);
			if (unfilteredFeatureHitTestResults.Any ()) {
				var first = unfilteredFeatureHitTestResults [0];
				return (first.Position, null, false);
			}

			return (null, null, false);
		}

		#endregion
	}
}
