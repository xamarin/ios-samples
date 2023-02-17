
namespace XamarinShot.Utils {
	using Foundation;
	using SceneKit;
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public static class SCNGeometrySourceExtensions {
		public static SCNGeometrySource Create (List<SCNVector4> colors)
		{
			SCNGeometrySource result = null;
			unsafe {
				var bytes = new List<byte> ();
				var colorsArray = colors.ToArray ();

				for (int i = 0; i < colors.Count; i++) {
					fixed (SCNVector4* point = &colorsArray [i]) {
						var intPtr = new IntPtr (point);
						var data = NSData.FromBytes (intPtr, (nuint) SCNVector4.SizeInBytes);

						bytes.AddRange (data.ToList ());
					}
				}

				var colorData = NSData.FromArray (bytes.ToArray ());
				result = SCNGeometrySource.FromData (colorData,
													SCNGeometrySourceSemantics.Color,
													colors.Count,
													true,
													4,
													sizeof (float),
													0,
													SCNVector4.SizeInBytes);
			}

			return result;
		}
	}
}
