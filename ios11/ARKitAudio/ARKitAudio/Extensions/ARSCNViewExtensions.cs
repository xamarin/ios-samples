
namespace ARKitAudio
{
    using ARKit;
    using CoreGraphics;
    using OpenTK;
    using SceneKit;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Convenience extensions on ARSCNView for hit testing
    /// </summary>
    public static class ARSCNViewExtensions
    {
        public static HitTestRay? HitTestRayFromScreenPosition(this ARSCNView view, CGPoint point)
        {
            HitTestRay? result = null;
            using (var frame = view.Session.CurrentFrame)
            {
                if (frame != null)
                {
                    var cameraPosition = frame.Camera.Transform.GetTranslation();

                    // Note: z: 1.0 will unproject() the screen position to the far clipping plane.
                    var positionVector = new SCNVector3((float)point.X, (float)point.Y, 1f);

                    var screenPositionOnFarClippingPlane = view.UnprojectPoint(positionVector);
                    var rayDirection = SCNVector3.Normalize(screenPositionOnFarClippingPlane - cameraPosition);

                    result = new HitTestRay { Origin = cameraPosition, Direction = rayDirection };
                }
            }

            return result;
        }

        public static SCNVector3? HitTestWithInfiniteHorizontalPlane(this ARSCNView view, CGPoint point, SCNVector3 pointOnPlane)
        {
            SCNVector3? result = null;

            var ray = view.HitTestRayFromScreenPosition(point);
            if (ray.HasValue)
            {
                // Do not intersect with planes above the camera or if the ray is almost parallel to the plane.
                if (ray.Value.Direction.Y <= -0.03f)
                {
                    // Return the intersection of a ray from the camera through the screen position with a horizontal plane
                    // at height (Y axis).
                    result = Utilities.RayIntersectionWithHorizontalPlane(ray.Value.Origin, ray.Value.Direction, pointOnPlane.Y);
                }
            }

            return result;
        }

        public static IList<FeatureHitTestResult> HitTestWithFeatures(this ARSCNView view,
                                                                      CGPoint point,
                                                                      float coneOpeningAngleInDegrees,
                                                                      float minDistance = 0,
                                                                      float maxDistance = float.MaxValue,
                                                                      int maxResults = 1)
        {

            var results = new List<FeatureHitTestResult>();

            ARPointCloud features = null;
            using(var frame = view.Session.CurrentFrame)
            {
                features = frame?.RawFeaturePoints;
            }

            if (features != null)
            {   
                var ray = view.HitTestRayFromScreenPosition(point);
                if (ray.HasValue)
                {
                    var maxAngleInDegrees = Math.Min(coneOpeningAngleInDegrees, 360f) / 2f;
                    var maxAngle = (maxAngleInDegrees / 180f) * Math.PI;

                    var points = features.Points;
                    for (nuint j = 0; j < features.Count; j++)
                    {
                        var feature = points[j];

                        var featurePosition = new SCNVector3((Vector3)feature);
                        var originToFeature = featurePosition - ray.Value.Origin;

                        var crossProduct = SCNVector3.Cross(originToFeature, ray.Value.Direction);
                        var featureDistanceFromResult = crossProduct.Length;

                        var hitTestResult = ray.Value.Origin + (ray.Value.Direction * SCNVector3.Dot(ray.Value.Direction, originToFeature));
                        var hitTestResultDistance = (hitTestResult - ray.Value.Origin).Length;

                        if (hitTestResultDistance < minDistance || hitTestResultDistance > maxDistance)
                        {
                            // Skip this feature - it is too close or too far away.
                            continue;
                        }

                        var originToFeatureNormalized = SCNVector3.Normalize(originToFeature);
                        var angleBetweenRayAndFeature = Math.Acos(SCNVector3.Dot(ray.Value.Direction, originToFeatureNormalized));

                        if (angleBetweenRayAndFeature > maxAngle)
                        {
                            // Skip this feature - is outside of the hit test cone.
                            continue;
                        }

                        // All tests passed: Add the hit against this feature to the results.
                        results.Add(new FeatureHitTestResult
                        {
                            Position = hitTestResult,
                            DistanceToRayOrigin = hitTestResultDistance,
                            FeatureHit = featurePosition,
                            FeatureDistanceToHitResult = featureDistanceFromResult
                        });
                    }

                    // Sort the results by feature distance to the ray.
                    results = results.OrderBy(result => result.DistanceToRayOrigin).ToList();

                    // Cap the list to maxResults.
                    var cappedResults = new List<FeatureHitTestResult>();
                    var i = 0;

                    while (i < maxResults && i < results.Count)
                    {
                        cappedResults.Add(results[i]);
                        i += 1;
                    }

                    results = cappedResults;
                }
            }

            return results;
        }

        public static IList<FeatureHitTestResult> HitTestWithFeatures(this ARSCNView view, CGPoint point)
        {
            var results = new List<FeatureHitTestResult>();

            var ray = view.HitTestRayFromScreenPosition(point);
            if (ray.HasValue)
            {
                var result = view.HitTestFromOrigin(ray.Value.Origin, ray.Value.Direction);
                if (result.HasValue)
                {
                    results.Add(result.Value);
                }
            }

            return results;
        }

        public static FeatureHitTestResult? HitTestFromOrigin(this ARSCNView view, SCNVector3 origin, SCNVector3 direction)
        {
            FeatureHitTestResult? result = null;

            ARPointCloud features = null;
            using (var frame = view.Session.CurrentFrame)
            {
                features = frame?.RawFeaturePoints;
            }

            if (features != null)
            {
                var points = features.Points;

                // Determine the point from the whole point cloud which is closest to the hit test ray.
                var closestFeaturePoint = origin;
                var minDistance = float.MaxValue; // Float.greatestFiniteMagnitude

                for (nuint i = 0; i < features.Count; i++)
                {
                    var feature = points[i];

                    var featurePosition = new SCNVector3((Vector3)feature);
                    var originVector = origin - featurePosition;

                    var crossProduct = SCNVector3.Cross(originVector, direction);
                    var featureDistanceFromResult = crossProduct.Length;

                    if (featureDistanceFromResult < minDistance)
                    {
                        closestFeaturePoint = featurePosition;
                        minDistance = featureDistanceFromResult;
                    }
                }

                // Compute the point along the ray that is closest to the selected feature.
                var originToFeature = closestFeaturePoint - origin;
                var hitTestResult = origin + (direction * SCNVector3.Dot(direction, originToFeature));
                var hitTestResultDistance = (hitTestResult - origin).Length;

                result = new FeatureHitTestResult
                {
                    Position = hitTestResult,
                    DistanceToRayOrigin = hitTestResultDistance,
                    FeatureHit = closestFeaturePoint,
                    FeatureDistanceToHitResult = minDistance
                };
            }

            return result;
        }
    }

    public struct HitTestRay
    {
        public SCNVector3 Origin { get; set; }

        public SCNVector3 Direction { get; set; }
    }

    public struct FeatureHitTestResult
    {
        public SCNVector3 Position { get; set; }

        public float DistanceToRayOrigin { get; set; }

        public SCNVector3 FeatureHit { get; set; }

        public float FeatureDistanceToHitResult { get; set; }
    }
}