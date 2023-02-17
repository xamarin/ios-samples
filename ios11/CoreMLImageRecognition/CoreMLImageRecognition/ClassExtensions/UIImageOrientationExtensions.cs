using System;
using UIKit;
using CoreGraphics;
using CoreImage;
using ImageIO;

namespace CoreMLImageRecognition {
	public static class UIImageOrientationExtensions {
		public static CIImageOrientation ToCIImageOrientation (this UIImageOrientation self)
		{

			// Take action based on value
			switch (self) {
			case UIImageOrientation.Up:
				return CIImageOrientation.TopLeft;
			case UIImageOrientation.UpMirrored:
				return CIImageOrientation.TopRight;
			case UIImageOrientation.Down:
				return CIImageOrientation.BottomLeft;
			case UIImageOrientation.DownMirrored:
				return CIImageOrientation.BottomRight;
			case UIImageOrientation.Left:
				return CIImageOrientation.LeftTop;
			case UIImageOrientation.LeftMirrored:
				return CIImageOrientation.LeftBottom;
			case UIImageOrientation.Right:
				return CIImageOrientation.RightTop;
			case UIImageOrientation.RightMirrored:
				return CIImageOrientation.RightBottom;
			}

			// Default to up
			return CIImageOrientation.TopLeft;
		}

	}
}
