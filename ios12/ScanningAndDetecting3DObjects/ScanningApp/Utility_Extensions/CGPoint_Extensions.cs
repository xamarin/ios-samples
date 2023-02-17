using System;
using CoreGraphics;

namespace ScanningAndDetecting3DObjects {
	internal static class CGPoint_Extensions {
		internal static double Length (this CGPoint self)
		{
			return Math.Sqrt (self.X * self.X + self.Y * self.Y);
		}

		// No operator overloading in extension methods until C# 8
		internal static CGPoint Add (this CGPoint self, CGPoint other) => new CGPoint (self.X + other.X, self.Y + other.Y);
	}
}
