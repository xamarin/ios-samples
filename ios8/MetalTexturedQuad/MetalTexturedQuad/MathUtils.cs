using System;
using OpenTK;

namespace MetalTexturedQuad
{
	public static class MathUtils
	{
		public static Matrix4 SwapColumnsAndRows (this Matrix4 matrix)
		{
			return new Matrix4 (matrix.Column0, matrix.Column1, matrix.Column2, matrix.Column3);
		}

		public static Matrix4 Scale (float x, float y, float z)
		{
			return new  Matrix4 (x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f, z, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix4 Translate (Vector3 t)
		{
			Matrix4 m = Matrix4.Identity;
			m.M41 = t.X;
			m.M42 = t.Y;
			m.M43 = t.Z;
			return m;
		}

		public static Matrix4 Translate (float x, float y, float z)
		{
			return Translate (new Vector3 (x, y, z));
		}

		public static float RadiansOverPi (float degrees)
		{
			return degrees * 1f / 180f;
		}

		public static Matrix4 Rotate (float angle, Vector3 r)
		{
			float a = RadiansOverPi (angle);
			float c = (float)Math.Cos (a);
			float s = (float)Math.Sin (a);

			float k = 1.0f - c;
			r.Normalize ();
			Vector3 u = r;
			Vector3 v = s * u;
			Vector3 w = k * u;

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = w.X * u.X + c;
			P.Y = w.X * u.Y + v.Z;
			P.Z = w.X * u.Z - v.Y;

			Q.X = w.X * u.Y - v.Z;
			Q.Y = w.Y * u.Y + c;
			Q.Z = w.Y * u.Z + v.X;

			R.X = w.X * u.Z + v.Y;
			R.Y = w.Y * u.Z - v.X;
			R.Z = w.Z * u.Z + c;

			S.W = 1.0f;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 Rotate (float angle, float x, float y, float z)
		{
			return Rotate (angle, new Vector3 (x, y, z));
		}

		public static Matrix4 Perspective (float width, float height, float near, float far)
		{
			float zNear = 2.0f * near;
			float zFar = far / (far - near);

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = zNear / width;
			Q.Y = zNear / height;
			R.Z = zFar;
			R.W = 1f;
			S.Z = -near * zFar;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 PerspectiveFov (float fovy, float aspect, float near, float far)
		{
			float angle = Radians (0.5f * fovy);
			float yScale = 1f / (float)Math.Tan (angle);
			float xScale = yScale / aspect;
			float zScale = far / (far - near);

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = xScale;
			Q.Y = yScale;
			R.Z = zScale;
			R.W = 1f;
			S.Z = -near * zScale;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 PerspectiveFov (float fovy, float width, float height, float near, float far)
		{
			float aspect = width / height;
			return PerspectiveFov (fovy, aspect, near, far);
		}

		public static Matrix4 LookAt (Vector3 eye, Vector3 center, Vector3 up)
		{
			Vector3 E = eye * -1f;

			var normalized = (center + E);
			normalized.Normalize ();
			Vector3 N = normalized;

			normalized = Vector3.Cross (up, N);
			normalized.Normalize ();
			Vector3 U = normalized;

			Vector3 V = Vector3.Cross (N, U);

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = U.X;
			P.Y = V.X;
			P.Z = N.X;

			Q.X = U.Y;
			Q.Y = V.Y;
			Q.Z = N.Y;

			R.X = U.Z;
			R.Y = V.Z;
			R.Z = N.Z;

			S.X = Vector3.Dot (U, E);
			S.Y = Vector3.Dot (V, E);
			S.Z = Vector3.Dot (N, E);
			S.W = 1.0f;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 Ortho2d (float left, float right, float bottom, float top, float near, float far)
		{
			float sLength = 1.0f / (right - left);
			float sHeight = 1.0f / (top - bottom);
			float sDepth = 1.0f / (far - near);

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = 2.0f * sLength;
			Q.Y = 2.0f * sHeight;
			R.Z = sDepth;
			S.Z = -near * sDepth;
			S.W = 1.0f;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 Ortho2d (Vector3 origin, Vector3 size)
		{
			return Ortho2d (origin.X, origin.Y, origin.Z, size.X, size.Y, size.Z);
		}

		public static Matrix4 Ortho2dOc (float left, float right, float bottom, float top, float near, float far)
		{
			float sLength = 1.0f / (right - left);
			float sHeight = 1.0f / (top - bottom);
			float sDepth = 1.0f / (far - near);

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = 2.0f * sLength;
			Q.Y = 2.0f * sHeight;
			R.Z = sDepth;
			S.X = -sLength * (left + right);
			S.Y = -sHeight * (top + bottom);
			S.Z = -sDepth * near;
			S.W = 1.0f;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 Ortho2dOc (Vector3 origin, Vector3 size)
		{
			return Ortho2dOc (origin.X, origin.Y, origin.Z, size.X, size.Y, size.Z);
		}

		public static Matrix4 Frustrum (float fovH, float fovV, float near, float far)
		{
			float width = 1f / (float)Math.Tan (Radians (0.5f * fovH));
			float height = 1f / (float)Math.Tan (Radians (0.5f * fovV));
			float sDepth = far / (far - near);

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = width;
			Q.Y = height;
			R.Z = sDepth;
			R.W = 1.0f;
			S.Z = -sDepth * near;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 Frustrum (float left, float right, float bottom, float top, float near, float far)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;
			float sDepth = far / depth;

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = width;
			Q.Y = height;
			R.Z = sDepth;
			R.W = 1.0f;
			S.Z = -sDepth * near;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static Matrix4 FrustrumOc (float left, float right, float bottom, float top, float near, float far)
		{
			float sWidth = 1.0f / (right - left);
			float sHeight = 1.0f / (top - bottom);
			float sDepth = far / (far - near);
			float dNear = 2.0f * near;

			Vector4 P = Vector4.Zero;
			Vector4 Q = Vector4.Zero;
			Vector4 R = Vector4.Zero;
			Vector4 S = Vector4.Zero;

			P.X = dNear * sWidth;
			Q.Y = dNear * sHeight;
			R.X = -sWidth * (right + left);
			R.Y = -sHeight * (top + bottom);
			R.Z = sDepth;
			R.W = 1.0f;
			S.Z = -sDepth * near;

			return MakeResultMatrix (P, Q, R, S);
		}

		public static float Radians (float degrees)
		{
			return 1.0f / 180.0f * (float)Math.PI * degrees;
		}

		public static Matrix4 MakeResultMatrix (Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
		{
			return new Matrix4 (column0.X, column1.X, column2.X, column3.X,
				column0.Y, column1.Y, column2.Y, column3.Y,
				column0.Z, column1.Z, column2.Z, column3.Z,
				column0.W, column1.W, column2.W, column3.W);
		}
	}
}

