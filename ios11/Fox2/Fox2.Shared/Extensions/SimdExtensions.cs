
namespace Fox2.Extensions
{
    using OpenTK;
    using SceneKit;

    public static class SimdExtensions
    {
        public static bool AllZero(this SCNVector3 vector3)
        {
            return vector3.X == 0f && vector3.Y == 0f && vector3.Z == 0f;
        }

        public static bool AllZero(this Vector2 vector2)
        {
            return vector2.X == 0f && vector2.Y == 0f;
        }

        public static SCNVector3 GetPosition(this SCNMatrix4 vector3)
        {
            return vector3.Column3.Xyz;
        }

        public static void SetPosition(ref SCNMatrix4 matrix, SCNVector3 value)
        {
            matrix.M41 = value.X;
            matrix.M42 = value.Y;
            matrix.M43 = value.Z;
        }
    }
}