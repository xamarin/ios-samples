
namespace AVCamBarcode.Extensions {
	using CoreGraphics;

	public static class CGRectExtensions {
		public static CGPoint CornerTopLeft (this CGRect rect)
		{
			return rect.Location;
		}

		public static CGPoint CornerTopRight (this CGRect rect)
		{
			rect.X += rect.Width;
			return rect.Location;
		}

		public static CGPoint CornerBottomRight (this CGRect rect)
		{
			return new CGPoint (rect.GetMaxX (), rect.GetMaxY ());
		}

		public static CGPoint CornerBottomLeft (this CGRect rect)
		{
			return new CGPoint (rect.GetMinX (), rect.GetMaxY ());
		}
	}
}
