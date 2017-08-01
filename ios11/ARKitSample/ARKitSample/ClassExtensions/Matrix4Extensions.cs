using System;
using Foundation;
using SceneKit;
using ARKit;
using OpenTK;

namespace ARKitSample
{
	public static class Matrix4Extensions
	{
		public static SCNMatrix4 ToSCNMatrix4(this Matrix4 matrix) {
			return new SCNMatrix4(matrix.Row0, matrix.Row1, matrix.Row2, matrix.Row3);
		}
	}
}
