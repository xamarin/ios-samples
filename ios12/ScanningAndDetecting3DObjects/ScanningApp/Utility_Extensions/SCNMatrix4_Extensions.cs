using System;
using SceneKit;
using UIKit;

namespace ScanningAndDetecting3DObjects
{
	internal static class SCNMatrix4_Extensions
	{
		// Note that this just straight-across converts, does _not_ transpose, (see README.MD for discussion)
		internal static OpenTK.NMatrix4 ToNMatrix4(this SCNMatrix4 self)
		{
			var newMatrix = new OpenTK.NMatrix4(
				self.M11, self.M21, self.M31, self.M41,
				self.M12, self.M22, self.M32, self.M42,
				self.M13, self.M23, self.M33, self.M43,
				self.M14, self.M24, self.M34, self.M44
			);
			return newMatrix;
		}

		/// Matrix-Vector multiplication.  Keep in mind that matrix types are named
		/// `FloatNxM` where `N` is the number of *columns* and `M` is the number of
		/// *rows*, so we multiply a `Float3x2 * Float3` to get a `Float2`, for
		/// example.
		internal static SCNVector4 Times(this SCNMatrix4 self, SCNVector4 vec) 
		{
			Func<SCNVector4, float> elementSum = v => v.X + v.Y + v.Z + v.W;
			Func<SCNVector4, float, float> rowTimesVecEl = (row, s) => elementSum(SCNVector4.Multiply(row, s));
			var x = rowTimesVecEl(self.Row0, vec.X);
			var y = rowTimesVecEl(self.Row1, vec.Y);
			var z = rowTimesVecEl(self.Row2, vec.Z);
			var w = rowTimesVecEl(self.Row3, vec.W);
			return new SCNVector4(x, y, z, w);
		}
	}
}
