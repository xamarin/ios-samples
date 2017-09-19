using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;
using OpenTK;

namespace PlacingObjects
{
	public static class SCNVector3Extensions
	{
		public static SCNVector3 One() {
			return new SCNVector3(1.0f, 1.0f, 1.0f);
		}

		public static SCNVector3 Uniform(float value)
		{
			return new SCNVector3(value, value, value);
		}

        public static CGPoint ToCGPoint(this SCNVector3 self)
        {
            return new CGPoint(self.X, self.Y);
        }

		public static void SetLength(this SCNVector3 vector3, float length){
			vector3.Normalize();
			vector3 *= length;
		}

		public static void SetMaximumLength(this SCNVector3 vector3, float maxLength)
		{
			if (vector3.LengthFast > maxLength) {
				vector3.SetLength(maxLength);
			}
		}

		public static void Normalize(this SCNVector3 vector3){
			var normalizedVector = vector3.Normalized();
			vector3.X = normalizedVector.X;
			vector3.Y = normalizedVector.Y;
			vector3.Z = normalizedVector.Z;
		}

		public static SCNVector3 Normalized(this SCNVector3 vector3) {

			if (vector3.Length < Double.Epsilon) {
				return vector3;
			} else {
				return vector3 / vector3.Length;
			}
		}

		public static float Dot(this SCNVector3 vector3, SCNVector3 vec) {
			return (vector3.X * vec.X) + (vector3.Y * vec.Y) + (vector3.Z * vec.Z);
		}

		public static SCNVector3 Cross(this SCNVector3 vector3, SCNVector3 vec)
		{
			return new SCNVector3(vector3.Y * vec.Z - vector3.Z * vec.Y, vector3.Z * vec.X - vector3.X * vec.Z, vector3.X * vec.Y - vector3.Y * vec.X);
		}

		public static SCNVector3 PositionFromTransform(NMatrix4 transform){
			var pFromComponents = new SCNVector3(transform.M14, transform.M24, transform.M34);
			return pFromComponents;
		}

		public static SCNVector3 Add(this SCNVector3 vector3, SCNVector3 right) {
			return new SCNVector3(vector3.X + right.X, vector3.Y + right.Y, vector3.Z + right.Z);
		}

		public static SCNVector3 Subtract(this SCNVector3 vector3, SCNVector3 right)
		{
			return new SCNVector3(vector3.X - right.X, vector3.Y - right.Y, vector3.Z - right.Z);
		}

		public static SCNVector3 Multiply(this SCNVector3 vector3, SCNVector3 right)
		{
			return new SCNVector3(vector3.X * right.X, vector3.Y * right.Y, vector3.Z * right.Z);
		}

		public static SCNVector3 Divide(this SCNVector3 vector3, SCNVector3 right)
		{
			return new SCNVector3(vector3.X / right.X, vector3.Y / right.Y, vector3.Z / right.Z);
		}
	}
}
