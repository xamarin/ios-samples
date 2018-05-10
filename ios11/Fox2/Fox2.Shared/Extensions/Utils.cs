
namespace Fox2.Extensions
{
    using SceneKit;

    public static class Utils
    {
        /// <summary>
        /// Returns plane / ray intersection distance from ray origin.
        /// </summary>
#if !__OSX__
        public static float PlaneIntersect(SCNVector3 planeNormal, float planeDist, SCNVector3 rayOrigin, SCNVector3 rayDirection)
#else
        public static System.nfloat PlaneIntersect(SCNVector3 planeNormal, System.nfloat planeDist, SCNVector3 rayOrigin, SCNVector3 rayDirection)
#endif
        {
            return (planeDist - SCNVector3.Dot(planeNormal, rayOrigin)) / SCNVector3.Dot(planeNormal, rayDirection);
        }
    }
}