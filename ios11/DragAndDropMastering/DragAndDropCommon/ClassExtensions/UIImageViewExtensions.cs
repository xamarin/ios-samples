using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using ImageIO;
using MapKit;
using CoreLocation;
using CoreImage;

namespace UIKit {
	public static class UIImageViewExtensions {
		/**
	     Converts a rect in image coordinates to a rect in the
	     image view's coordinate space. Does not take clipping
	     into account.
	     */
		public static CGRect ConvertFromImageRect (this UIImageView self, CGRect rect)
		{

			if (self.Image == null) return CGRect.Empty;
			var widthScale = self.Bounds.Width / self.Image.Size.Width;
			var heightScale = self.Bounds.Height / self.Image.Size.Height;
			return new CGRect (rect.GetMinX () * widthScale,
							  rect.GetMinY () * heightScale,
							  rect.Width * widthScale,
							  rect.Height * heightScale
							 );
		}
	}
}
