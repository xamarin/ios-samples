using System;
using UIKit;
using CoreGraphics;

namespace VisionFramework
{
	public static class CGPointExtensions
	{
		public static CGPoint Scaled(this CGPoint self, CGSize size) {
			return new CGPoint(self.X * size.Width, self.Y * size.Height);
		}
	}
}
