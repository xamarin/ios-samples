using System;
using OpenTK;

namespace OpenGLES20Example
{
	struct Color {
		public float red;
		public float green;
		public float blue;
		public float alpha;
	}

	struct TextureCoord {
		public float S;
		public float T;
	}

	public static class GLCommon
	{
		public static float radiansFromDegrees (float degrees)
		{
			return (float)Math.PI * degrees / 180.0f;
		}

		public static void Matrix3DSetRotationByRadians (ref float[] matrix, float radians, ref Vector3 vector)
		{
			float mag = (float) Math.Sqrt ((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));

			if (mag == 0.0f) {
				vector.X = 1.0f;
				vector.Y = 0.0f;
				vector.Z = 0.0f;
			} else if (mag != 1.0f) {
				vector.X /= mag;
				vector.Y /= mag;
				vector.Z /= mag;
			}

			float c = (float) Math.Cos (radians);
			float s = (float) Math.Sin (radians);
			matrix [3] = matrix [7] = matrix [11] = 0.0f;
			matrix [12] = matrix [13] = matrix [14] = 0.0f;
			matrix [15] = 1.0f;

			matrix [0] = (vector.X * vector.X) * (1 - c) + c;
			matrix [1] = (vector.Y * vector.X) * (1 - c) + (vector.Z * s);
			matrix [2] = (vector.X * vector.Z) * (1 - c) - (vector.Y * s);
			matrix [4] = (vector.X * vector.Y) * (1 - c) - (vector.Z * s);
			matrix [5] = (vector.Y * vector.Y) * (1 - c) + c;
			matrix [6] = (vector.Y * vector.Z) * (1 - c) + (vector.X * s);
			matrix [8] = (vector.X * vector.Z) * (1 - c) + (vector.Y * s);
			matrix [9] = (vector.Y * vector.Z) * (1 - c) - (vector.X * s);
			matrix [10] = (vector.Z * vector.Z) * (1 - c) + c;
		}

		public static void Matrix3DSetRotationByDegrees (ref float[] matrix, float degrees, Vector3 vector)
		{
			Matrix3DSetRotationByRadians (ref matrix, radiansFromDegrees (degrees), ref vector);
		}

		public static void Matrix3DSetIdentity (ref float[] matrix)
		{
			matrix [0] = matrix [5] = matrix [10] = matrix [15] = 1.0f;
			matrix [1] = matrix [2] = matrix [3] = matrix [4] = 0.0f;
			matrix [6] = matrix [7] = matrix [8] = matrix [9] = 0.0f;
			matrix [11] = matrix [12] = matrix [13] = matrix [14] = 0.0f;
		}

		public static void Matrix3DSetTranslation (ref float[] matrix, float xTranslate, float yTranslate, float zTranslate)
		{
			matrix [0] = matrix [5] = matrix [10] = matrix [15] = 1.0f;
			matrix [1] = matrix [2] = matrix [3] = matrix [4] = 0.0f;
			matrix [6] = matrix [7] = matrix [8] = matrix [9] = 0.0f;
			matrix [11] = 0.0f;
			matrix [12] = xTranslate;
			matrix [13] = yTranslate;
			matrix [14] = zTranslate;
		}

		public static void Matrix3DSetScaling (ref float[] matrix, float xScale, float yScale, float zScale)
		{
			matrix [1] = matrix [2] = matrix [3] = matrix [4] = 0.0f;
			matrix [6] = matrix [7] = matrix [8] = matrix [9] = 0.0f;
			matrix [11] = matrix [12] = matrix [13] = matrix [14] = 0.0f;
			matrix [0] = xScale;
			matrix [5] = yScale;
			matrix [10] = zScale;
			matrix [15] = 1.0f;
		}

		public static void Matrix3DSetUniformScaling (ref float[] matrix, float scale)
		{
			Matrix3DSetScaling (ref matrix, scale, scale, scale);
		}

		public static void Matrix3DSetZRotationUsingRadians (ref float[] matrix, float radians)
		{
			matrix [0] = (float)Math.Cos (radians);
			matrix [1] = (float)Math.Sin (radians);
			matrix [4] = -matrix [1];
			matrix [5] = matrix [0];
			matrix [2] = matrix [3] = matrix [6] = matrix [7] = matrix [8] = 0.0f;
			matrix [9] = matrix [11] = matrix [12] = matrix [13] = matrix [14] = 0.0f;
			matrix [10] = matrix [15] = 0;
		}

		public static void Matrix3DSetZRotationUsingDegrees (ref float[] matrix, float degrees)
		{
			Matrix3DSetZRotationUsingRadians (ref matrix, radiansFromDegrees (degrees));
		}

		public static void Matrix3DSetXRotationUsingRadians (ref float[] matrix, float radians)
		{
			matrix [0] = matrix [15] = 1.0f;
			matrix [1] = matrix [2] = matrix [3] = matrix [4] = 0.0f;
			matrix [7] = matrix [8] = 0.0f;
			matrix [11] = matrix [12] = matrix [13] = matrix [14] = 0.0f;
			matrix [5] = (float) Math.Cos (radians);
			matrix [6] = - (float)Math.Sin (radians);
			matrix [9] = - matrix [6];
			matrix [10] = matrix [5];
		}

		public static void Matrix3DSetXRotationUsingDegrees (ref float[] matrix, float degrees)
		{
			Matrix3DSetXRotationUsingRadians (ref matrix, radiansFromDegrees (degrees));
		}

		public static void Matrix3DSetYRotationUsingRadians (ref float[] matrix, float radians)
		{
			matrix [0] = (float)Math.Cos (radians);
			matrix [2] = (float)Math.Sin (radians);
			matrix [8] = - matrix [2];
			matrix [10] = matrix [0];
			matrix [1] = matrix [3] = matrix [4] = matrix [6] = matrix [7] = 0.0f;
			matrix [9] = matrix [11] = matrix [12] = matrix [13] = matrix [14] = 0.0f;
			matrix [5] = matrix [15] = 1.0f;
		}

		public static void Matrix3DSetYRotationUsingDegrees (ref float[] matrix, float degrees)
		{
			Matrix3DSetYRotationUsingRadians (ref matrix, radiansFromDegrees (degrees));
		}

		public static float[] Matrix3DMultiply (float[] m1, float[] m2)
		{
			float[] result = new float[16];

			result [0] = m1 [0] * m2 [0] + m1 [4] * m2 [1] + m1 [8] * m2 [2] + m1 [12] * m2 [3];
			result [1] = m1 [1] * m2 [0] + m1 [5] * m2 [1] + m1 [9] * m2 [2] + m1 [13] * m2 [3];
			result [2] = m1 [2] * m2 [0] + m1 [6] * m2 [1] + m1 [10] * m2 [2] + m1 [14] * m2 [3];
			result [3] = m1 [3] * m2 [0] + m1 [7] * m2 [1] + m1 [11] * m2 [2] + m1 [15] * m2 [3];

			result [4] = m1 [0] * m2 [4] + m1 [4] * m2 [5] + m1 [8] * m2 [6] + m1 [12] * m2 [7];
			result [5] = m1 [1] * m2 [4] + m1 [5] * m2 [5] + m1 [9] * m2 [6] + m1 [13] * m2 [7];
			result [6] = m1 [2] * m2 [4] + m1 [6] * m2 [5] + m1 [10] * m2 [6] + m1 [14] * m2 [7];
			result [7] = m1 [3] * m2 [4] + m1 [7] * m2 [5] + m1 [11] * m2 [6] + m1 [15] * m2 [7];

			result [8] = m1 [0] * m2 [8] + m1 [4] * m2 [9] + m1 [8] * m2 [10] + m1 [12] * m2 [11];
			result [9] = m1 [1] * m2 [8] + m1 [5] * m2 [9] + m1 [9] * m2 [10] + m1 [13] * m2 [11];
			result [10] = m1 [2] * m2 [8] + m1 [6] * m2 [9] + m1 [10] * m2 [10] + m1 [14] * m2 [11];
			result [11] = m1 [3] * m2 [8] + m1 [7] * m2 [9] + m1 [11] * m2 [10] + m1 [15] * m2 [11];

			result [12] = m1 [0] * m2 [12] + m1 [4] * m2 [13] + m1 [8] * m2 [14] + m1 [12] * m2 [15];
			result [13] = m1 [1] * m2 [12] + m1 [5] * m2 [13] + m1 [9] * m2 [14] + m1 [13] * m2 [15];
			result [14] = m1 [2] * m2 [12] + m1 [6] * m2 [13] + m1 [10] * m2 [14] + m1 [14] * m2 [15];
			result [15] = m1 [3] * m2 [12] + m1 [7] * m2 [13] + m1 [11] * m2 [14] + m1 [15] * m2 [15];

			return result;
		}

		public static void Matrix3DSetOrthoProjection (ref float[] matrix, float left, float right, float bottom, 
		                                   float top, float near, float far)
		{
			matrix [1] = matrix [2] = matrix [3] = matrix [4] = matrix [6] = 0.0f;
			matrix [7] = matrix [8] = matrix [9] = matrix [11] = 0.0f;
			matrix [0] = 2.0f / (right - left);
			matrix [5] = 2.0f / (top - bottom);
			matrix [10] = -2.0f / (far - near);
			matrix [12] = (right + left) / (right - left);
			matrix [13] = (top + bottom) / (top - bottom);
			matrix [14] = (far + near) / (far - near);
			matrix [15] = 1.0f;
		}

		public static void Matrix3DSetFrustumProjection (ref float[] matrix, float left, float right, float bottom,
		                                                 float top, float zNear, float zFar)
		{
			matrix [1] = matrix [2] = matrix [3] = matrix [4] = 0.0f;
			matrix [6] = matrix [7] = matrix [12] = matrix [13] = matrix [15] = 0.0f;

			matrix [0] = 2 * zNear / (right - left);
			matrix [5] = 2 * zNear / (top - bottom);
			matrix [8] = (right + left) / (right - left);
			matrix [9] = (top + bottom) / (top - bottom);
			matrix [10] = - (zFar + zNear) / (zFar - zNear);
			matrix [11] = - 1.0f;
			matrix [14] = - (2 * zFar * zNear) / (zFar - zNear);
		}

		public static void Matrix3DSetPerspectiveProjectionWithFieldOfView (ref float[] matrix, float fieldOfVision,
		                                                                    float near, float far, float aspectRatio)
		{
			float size = near * (float)Math.Tan (radiansFromDegrees (fieldOfVision) / 2.0f);
			Matrix3DSetFrustumProjection (ref matrix, -size, size, -size / aspectRatio, 
			                              size / aspectRatio, near, far);
		}
	}
}