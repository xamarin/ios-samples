using System;
using CoreGraphics;
using UIKit;

namespace PhotoAlbum {
	public static class UIImageViewExtensions {
		/// <summary>
		/// Returns a rect that can be applied to the image view to clip to the image, assuming a scale aspect fit content mode
		/// </summary>
		public static CGRect ContentClippingRect (this UIImageView imageView)
		{
			if (imageView.ContentMode != UIViewContentMode.ScaleAspectFit) {
				return imageView.Bounds;
			}
			var image = imageView.Image;

			var imageWidth = image.Size.Width;
			var imageHeight = image.Size.Height;
			if (!(imageWidth > 0 && imageHeight > 0)) {
				return imageView.Bounds;
			}

			nfloat scale;
			if (imageWidth > imageHeight) {
				scale = imageView.Bounds.Size.Width / imageWidth;
			} else {
				scale = imageView.Bounds.Size.Height / imageHeight;
			}

			var clippingSize = new CGSize (imageWidth * scale, imageHeight * scale);
			var x = (imageView.Bounds.Size.Width - clippingSize.Width) / 2.0;
			var y = (imageView.Bounds.Size.Height - clippingSize.Height) / 2.0;

			return new CGRect (new CGPoint (x, y), clippingSize);
		}
	}
}
