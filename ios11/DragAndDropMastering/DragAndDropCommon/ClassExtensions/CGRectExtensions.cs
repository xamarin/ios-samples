using System;
using CoreGraphics;

namespace CoreGraphics {
	public static class CGRectExtensions {
		public static float Area (this CGRect self)
		{
			return (float) (self.Width * self.Height);
		}
	}
}
