
namespace XamarinShot.Utils
{
    using SceneKit;
    using System;
    using System.Collections.Generic;
    using UIKit;

    public static class SimdExtensions
    {
        public static SCNMatrix4 SetTranslation(this SCNMatrix4 matrix, OpenTK.Vector3 value)
        {
            // only for column-major matrixes
            matrix.M14 = value.X;
            matrix.M24 = value.Y;
            matrix.M34 = value.Z;
            matrix.M44 = 1f;

            return matrix;
        }

        public static SCNVector3 GetTranslation(this OpenTK.NMatrix4 matrix)
        {
            // only for column-major matrixes
            return matrix.Column3.Xyz;
        }

        public static SCNVector3 GetTranslation(this SCNMatrix4 matrix)
        {
            // only for column-major matrixes
            return matrix.Column3.Xyz;
        }

        public static bool HasNaN(this SCNVector4 self)
        {
            return float.IsNaN(self.X) || float.IsNaN(self.Y) || float.IsNaN(self.Z) || float.IsNaN(self.W);
        }

        public static bool HasNaN(this SCNVector3 vector)
        {
            return float.IsNaN(vector.X) || float.IsNaN(vector.Y) || float.IsNaN(vector.Z);
        }

        public static bool AlmostEqual(this SCNVector3 self, SCNVector3 value, float tolerance)
        {
            return (self - value).Length <= tolerance;
        }

        public static bool AlmostEqual(this SCNVector4 self, SCNVector4 value, float tolerance)
        {
            return (self - value).Length <= tolerance;
        }

        public static SCNMatrix4 CreateFromScale(SCNVector3 vector)
        {
            return new SCNMatrix4(new SCNVector4(vector.X, 0, 0, 0),
                                  new SCNVector4(0, vector.Y, 0, 0),
                                  new SCNVector4(0, 0, vector.Z, 0),
                                  SCNVector4.UnitW);
        }

        public static SCNMatrix4 CreateFromScale(float factor)
        {
            return SimdExtensions.CreateFromScale(new SCNVector3(factor, factor, factor));
        }

        public static SCNVector4 CreateVector4(UIColor color)
        {
            color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);
            return new SCNVector4((float)red, (float)green, (float)blue, (float)alpha);
        }

        public static SCNMatrix4 Normalize(this SCNMatrix4 matrix)
        {
            // for row-major matrixes only
            var normalized = matrix;

            var row0 = SCNVector4.Normalize(matrix.Row0);
            var row1 = SCNVector4.Normalize(matrix.Row1);
            var row2 = SCNVector4.Normalize(matrix.Row2);

            normalized.Row0 = row0;
            normalized.Row1 = row1;
            normalized.Row2 = row2;

            return normalized;
        }

        public static SCNVector3 Reduce(this IList<SCNVector3> items, SCNVector3 initialResult)
        {
            foreach (var item in items)
            {
                initialResult += item;
            }

            return initialResult;
        }

        public static SCNMatrix4 ToSCNMatrix4(this OpenTK.NMatrix4 self)
        {
            return new SCNMatrix4(self.Row0, self.Row1, self.Row2, self.Row3);
        }

        public static OpenTK.NMatrix4 ToNMatrix4(this SCNMatrix4 self)
        {
            return new OpenTK.NMatrix4((OpenTK.Vector4)self.Row0, (OpenTK.Vector4)self.Row1, (OpenTK.Vector4)self.Row2, (OpenTK.Vector4)self.Row3);
        }

        public static SCNVector3 ToSCNVector3(this OpenTK.NVector3 self)
        {
            return new SCNVector3(self.X, self.Y, self.Z);
        }

        public static SCNVector3 Mix(SCNVector3 x, SCNVector3 y, SCNVector3 t)
        {
            return x + SCNVector3.Multiply(t, y - x);
        }

        public static SCNVector4 Multiply(this SCNMatrix4 matrix, SCNVector4 vector)
        {
            var x = matrix.Column0.X * vector.X;
            var y = matrix.Column0.Y * vector.X;
            var z = matrix.Column0.Z * vector.X;
            var w = matrix.Column0.W * vector.X;

            x += matrix.Column1.X * vector.Y;
            y += matrix.Column1.Y * vector.Y;
            z += matrix.Column1.Z * vector.Y;
            w += matrix.Column1.W * vector.Y;

            x += matrix.Column2.X * vector.Z;
            y += matrix.Column2.Y * vector.Z;
            z += matrix.Column2.Z * vector.Z;
            w += matrix.Column2.W * vector.Z;

            x += matrix.Column3.X * vector.W;
            y += matrix.Column3.Y * vector.W;
            z += matrix.Column3.Z * vector.W;
            w += matrix.Column3.W * vector.W;

            return new SCNVector4(x, y, z, w);
        }

        public static SCNQuaternion Divide(this SCNQuaternion left, SCNQuaternion right)
        {
            return SCNQuaternion.Multiply(left, SCNQuaternion.Invert(right));
        }

        public static OpenTK.Quaternion ToQuaternion(this SCNQuaternion self)
        {
            return new OpenTK.Quaternion(self.X, self.Y, self.Z, self.W);
        }

        public static SCNVector4 GetVector(this SCNQuaternion self)
        {
            return new SCNVector4(self.Xyz, self.W);
        }

        public static SCNVector3 Act(this SCNQuaternion self, SCNVector3 vector)
        {
            // Calculate the resulting vector using the Hamilton product
            // P' = RPR'
            // P  = [0, p1, p2, p2] < -- vector
            // R  = [w, x, y, z] < -- rotation
            // R' = [w, -x, -y, -z]

            var p = new SCNQuaternion(vector, 0f);
            var r = self;
            var rt = r;
            rt.Conjugate();

            return SCNQuaternion.Multiply(SCNQuaternion.Multiply(r, p), rt).Xyz;
        }

        public static SCNQuaternion CreateQuaternion(SCNVector3 v1, SCNVector3 v2)
        {
            var a = SCNVector3.Cross(v1, v2);
            var w = (float)Math.Sqrt(v1.LengthSquared * v2.LengthSquared) + SCNVector3.Dot(v1, v2);
            var result = new SCNQuaternion(a.X, a.Y, a.Z, w);
            result.Normalize();

            return result;
        }

        public static SCNQuaternion CreateQuaternion(SCNMatrix4 matrix)
        {
            //http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
            float w, x, y, z;

            var t = matrix.M11 + matrix.M22 + matrix.M33 + 1f;
            if (t > 0)
            {
                var s = 0.5f / (float)Math.Sqrt(t);
                w = 0.25f / s;
                x = (matrix.M32 - matrix.M23) * s;
                y = (matrix.M13 - matrix.M31) * s;
                z = (matrix.M21 - matrix.M12) * s;
            }
            else
            {
                /*var tr = matrix.M11 + matrix.M22 + matrix.M33;
                if (tr > 0)
                {
                    var s = (float)Math.Sqrt(tr + 1) * 2f;
                    w = 0.25f / s;
                    x = (matrix.M32 - matrix.M23) * s;
                    y = (matrix.M13 - matrix.M31) * s;
                    z = (matrix.M21 - matrix.M12) * s;
                }
                else*/
                if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33)
                {
                    var s = (float)Math.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33) * 2f; // S=4*qx 
                    w = (matrix.M32 - matrix.M23) / s;
                    x = 0.25f * s;
                    y = (matrix.M12 + matrix.M21) / s;
                    z = (matrix.M13 + matrix.M31) / s;
                }
                else if (matrix.M22 > matrix.M33)
                {
                    var s = (float)Math.Sqrt(1f + matrix.M22 - matrix.M11 - matrix.M33) * 2f; // S=4*qy
                    w = (matrix.M13 - matrix.M31) / s;
                    x = (matrix.M12 + matrix.M21) / s;
                    y = 0.25f * s;
                    z = (matrix.M23 + matrix.M32) / s;
                }
                else
                {
                    var s = (float)Math.Sqrt(1f + matrix.M33 - matrix.M11 - matrix.M22) * 2f; // S=4*qz
                    w = (matrix.M21 - matrix.M12) / s;
                    x = (matrix.M13 + matrix.M31) / s;
                    y = (matrix.M23 + matrix.M32) / s;
                    z = 0.25f * s;
                }
            }

            return new SCNQuaternion(x, y, z, w);
        }
    }
}