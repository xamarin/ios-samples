using System;
using CoreGraphics;

namespace AVCamBarcode
{
	public static class CGRectExtensions
	{
		public static CGPoint CornerTopLeft (this CGRect rect)
		{
			return rect.Location;
		}

		public static CGPoint CornerTopRight (this CGRect rect)
		{
			return rect.Location.WithOffsetX (rect.Width);
		}

		public static CGPoint CornerBottomRight (this CGRect rect)
		{
			return new CGPoint (rect.GetMaxX (), rect.GetMaxY ());
		}

		public static CGPoint CornerBottomLeft (this CGRect rect)
		{
			return new CGPoint (rect.GetMinX (), rect.GetMaxY ());
		}

		public static CGRect WithX (this CGRect rect, nfloat x)
		{
			rect.X = x;
			return rect;
		}

		public static CGRect WithY (this CGRect rect, nfloat y)
		{
			rect.Y = y;
			return rect;
		}

		public static CGRect WithWidth (this CGRect rect, nfloat width)
		{
			rect.Width = width;
			return rect;
		}

		public static CGRect WithHeight (this CGRect rect, nfloat height)
		{
			rect.Height = height;
			return rect;
		}

		public static CGRect WithLocation (this CGRect rect, CGPoint location)
		{
			rect.Location = location;
			return rect;
		}
	}

	public static class CGPointExtensions
	{
		public static CGPoint WithOffsetX (this CGPoint rect, nfloat dx)
		{
			rect.X += dx;
			return rect;
		}

		public static CGPoint WithOffsetY(this CGPoint rect, nfloat dy)
		{
			rect.Y += dy;
			return rect;
		}
	}
}
