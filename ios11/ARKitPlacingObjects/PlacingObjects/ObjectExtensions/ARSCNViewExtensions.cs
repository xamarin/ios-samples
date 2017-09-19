using System;
using System.Collections.Generic;
using System.Linq;
using ARKit;
using CoreGraphics;
using SceneKit;

namespace PlacingObjects
{
	public static class ARSCNViewExtensions
	{
		public static void Setup(this ARSCNView self)
		{
			self.AntialiasingMode = SCNAntialiasingMode.Multisampling4X;
			self.AutomaticallyUpdatesLighting = false;
			self.PreferredFramesPerSecond = 60;
			self.ContentScaleFactor = 1.3f;

			var camera = self.PointOfView.Camera;
			if (camera != null)
			{
				camera.WantsHdr = true;
				camera.WantsExposureAdaptation = true;
				camera.ExposureOffset = -1;
				camera.MinimumExposure = -1;
				camera.MaximumExposure = 3;
			}
		}

		public static FeatureHitTestResult HitTestFromOrigin(this ARSCNView self, SCNVector3 origin, SCNVector3 direction)
		{
			if (self.Session == null || ViewController.CurrentFrame == null)
			{
				return null;
			}

			var currentFrame = ViewController.CurrentFrame;

			var features = currentFrame.RawFeaturePoints;
			if (features == null)
			{
				return null;
			}

			var points = features.Points;

			// Determine the point from the whole point cloud which is closest to the hit test ray.
			var closestFeaturePoint = origin;
			var minDistance = float.MaxValue;

			for (int n = 0; n < (int)features.Count; ++n)
			{
				var feature = points[n];
				var featurePos = new SCNVector3(feature.X, feature.Y, feature.Z);

				var originVector = origin.Subtract(featurePos);
				var crossProduct = originVector.Cross(direction);
				var featureDistanceFromResult = crossProduct.Length;

				if (featureDistanceFromResult < minDistance)
				{
					closestFeaturePoint = featurePos;
					minDistance = featureDistanceFromResult;
				}
			}

			// Compute the point along the ray that is closest to the selected feature.
			var originToFeature = closestFeaturePoint.Subtract(origin);
			var hitTestResult = origin.Add(direction * direction.Dot(originToFeature));
			var hitTestResultDistance = hitTestResult.Subtract(origin).LengthFast;

			// Return result
			return new FeatureHitTestResult(hitTestResult, hitTestResultDistance, closestFeaturePoint, minDistance);
		}

		public static HitTestRay HitTestRayFromScreenPos(this ARSCNView self, CGPoint point)
		{
			if (self.Session == null || ViewController.CurrentFrame == null)
			{
				return null;
			}

			var frame = ViewController.CurrentFrame;
			if (frame == null || frame.Camera == null || frame.Camera.Transform == null)
			{
				return null;
			}

			var cameraPos = SCNVector3Extensions.PositionFromTransform(frame.Camera.Transform);

			// Note: z: 1.0 will unproject() the screen position to the far clipping plane.
			var positionVec = new SCNVector3((float)point.X, (float)point.Y, 1.0f);
			var screenPosOnFarClippingPlane = self.UnprojectPoint(positionVec);

			var rayDirection = screenPosOnFarClippingPlane.Subtract(cameraPos);
			rayDirection.Normalize();

			return new HitTestRay(cameraPos, rayDirection);
		}

		public static SCNVector3? HitTestWithInfiniteHorizontalPlane(this ARSCNView self, CGPoint point, SCNVector3 pointOnPlane)
		{
			if (self.Session == null || ViewController.CurrentFrame == null)
			{
				return null;
			}

			var currentFrame = ViewController.CurrentFrame;

			var ray = self.HitTestRayFromScreenPos(point);
			if (ray == null)
			{
				return null;
			};

			// Do not intersect with planes above the camera or if the ray is almost parallel to the plane.
			if (ray.Direction.Y > -0.03f)
			{
				return null;
			}

			return Utilities.RayIntersectionWithHorizontalPlane(ray.Origin, ray.Direction, pointOnPlane.Y);
		}

		public static FeatureHitTestResult[] HitTestWithFeatures(this ARSCNView self, CGPoint point, double coneOpeningAngleInDegrees, double minDistance = 0, double maxDistance = Double.MaxValue, int maxResults = 1)
		{
			var results = new List<FeatureHitTestResult>();

			if (self.Session == null || ViewController.CurrentFrame == null)
			{
				return results.ToArray();
			}
			var features = ViewController.CurrentFrame.RawFeaturePoints;
			if (features == null)
			{
				return results.ToArray();
			}

			var ray = self.HitTestRayFromScreenPos(point);
			if (ray == null)
			{
				return results.ToArray();
			}

			var maxAngleInDeg = Math.Min(coneOpeningAngleInDegrees, 360) / 2.0;
			var maxAngle = (maxAngleInDeg / 180) * Math.PI;

			foreach (var featurePos in features.Points)
			{
				var scnFeaturePos = new SCNVector3(featurePos.X, featurePos.Y, featurePos.Z);
				var originToFeature = scnFeaturePos - ray.Origin;
				var crossProduct = originToFeature.Cross(ray.Direction);
				var featureDistanceFromResult = crossProduct.LengthFast;

				var hitTestResult = ray.Origin + (ray.Direction * (ray.Direction.Dot(originToFeature)));
				var hitTestResultDistance = (hitTestResult - ray.Origin).LengthFast;

				if (hitTestResultDistance < minDistance || hitTestResultDistance > maxDistance)
				{
					// Skip this feature -- it's too close or too far
					continue;
				}

				var originToFeatureNormalized = originToFeature.Normalized();
				var angleBetweenRayAndFeature = Math.Acos(ray.Direction.Dot(originToFeatureNormalized));

				if (angleBetweenRayAndFeature > maxAngle)
				{
					// Skip this feature -- it's outside the cone 
					continue;
				}

				// All tests passed: Add the hit against this feature to the results.
				results.Add(new FeatureHitTestResult(hitTestResult, hitTestResultDistance, scnFeaturePos, featureDistanceFromResult));
			}

			// Sort the results by feature distance to the ray.
			results.Sort((a, b) => a.DistanceToRayOrigin.CompareTo(b.DistanceToRayOrigin));

			// Cap the list to maxResults.
			results.GetRange(0, Math.Min(results.Count(), maxResults));
			return results.ToArray();
		}

		public static FeatureHitTestResult[] HitTestWithFeatures(this ARSCNView self, CGPoint pt)
		{
			var results = new List<FeatureHitTestResult>();
			var ray = self.HitTestRayFromScreenPos(pt);
			if (ray == null)
			{
				return results.ToArray();
			}
			var result = self.HitTestFromOrigin(ray.Origin, ray.Direction);
			if (result != null)
			{
				results.Add(result);
			}
			return results.ToArray();
		}
	}
}
