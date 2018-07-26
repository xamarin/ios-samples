using System;
using Foundation;
using SceneKit;
using ARKit;
using OpenTK;

namespace ARKitSample
{
	public static class Matrix4Extensions
	{
		public static SCNMatrix4 ToSCNMatrix4(this NMatrix4 self)
		{
			var row0 = new SCNVector4(self.M11, self.M12, self.M13, self.M14);
			var row1 = new SCNVector4(self.M21, self.M22, self.M23, self.M24);
			var row2 = new SCNVector4(self.M31, self.M32, self.M33, self.M34);
			var row3 = new SCNVector4(self.M41, self.M42, self.M43, self.M44);
			return new SCNMatrix4(row0, row1, row2, row3);
		}
	}
}
