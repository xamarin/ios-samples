using SceneKit;
using OpenTK;

namespace ScanningAndDetecting3DObjects {
	internal static class NMatrix4_Extensions {
		internal static SCNMatrix4 ToSCNMatrix4 (this NMatrix4 self)
		{
			var newMatrix = new SCNMatrix4 (
				self.M11, self.M21, self.M31, self.M41,
				self.M12, self.M22, self.M32, self.M42,
				self.M13, self.M23, self.M33, self.M43,
				self.M14, self.M24, self.M34, self.M44
			);

			return newMatrix;
		}

		// Important: This is transposed from SCNMatrix4! (See, for instance, Ray.DragPlaneTransform() functions)
		internal static Vector3 Position (this NMatrix4 self) => self.Column3.Xyz;
	}
}
