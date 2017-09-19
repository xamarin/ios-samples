using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public static class CGSizeExtensions
	{
		public static CGSize FromCGPoint (CGPoint point) {

			// Create new size from point
			return new CGSize(point.X, point.Y);
		}

		public static string ToString(this CGSize size) {
			return $"({size.Width:#.00}, {size.Height:#.00})";
		}

		public static CGSize Add (this CGSize size, CGSize right){
			return new CGSize(size.Width + right.Width, size.Height + right.Height);
		}

		public static CGSize Add(this CGSize size, float value)
		{
			return new CGSize(size.Width + value, size.Height + value);
		}

		public static CGSize Subtract(this CGSize size, CGSize right)
		{
			return new CGSize(size.Width - right.Width, size.Height - right.Height);
		}

		public static CGSize Subtract(this CGSize size, float value)
		{
			return new CGSize(size.Width - value, size.Height - value);
		}

		public static CGSize Divide(this CGSize size, CGSize right)
		{
			return new CGSize(size.Width / right.Width, size.Height / right.Height);
		}

		public static CGSize Divide(this CGSize size, float value)
		{
			return new CGSize(size.Width / value, size.Height / value);
		}

		public static CGSize Multiply(this CGSize size, CGSize right)
		{
			return new CGSize(size.Width * right.Width, size.Height * right.Height);
		}

		public static CGSize Multiply(this CGSize size, float value)
		{
			return new CGSize(size.Width * value, size.Height * value);
		}
	}
}
