using System;
using UIKit;
using CoreGraphics;

namespace CoreMLVision
{
	public static class CGRectExtensions
	{
		public static CGRect Scaled(this CGRect self, CGSize size) {
			return new CGRect(self.X * size.Width, 
			                  self.Y * size.Height, 
			                  self.Size.Width * size.Width, 
			                  self.Size.Height * size.Height);
		}
	}
}
