using System;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreImage;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public static class UIImageExtensions
	{
		public static UIImage Inverted(this UIImage image) {

			// Convert to Core Image
			var ciImage = new CIImage(image);
			if (ciImage == null)
			{
				return null;
			}

			// Apply filter
			ciImage = ciImage.CreateByFiltering("CIColorInvert", new NSDictionary());

			// Return results
			return new UIImage(ciImage);
		}

		public static UIImage ComposeButtonImage(string imageName, float alpha = 1.0f) {
			return ComposeButtonImage(UIImage.FromBundle(imageName), alpha);
		}

		public static UIImage ComposeButtonImage(UIImage thumbImage, float alpha = 1.0f) {

			// Get images and masks
			var maskImage = thumbImage;
			var invertedImage = thumbImage.Inverted();
			var thumbnailImage = thumbImage;
			if (invertedImage != null) thumbnailImage = invertedImage;

			// Compose a button image based on a white background and the inverted thumbnail image.
			UIGraphics.BeginImageContextWithOptions(maskImage.Size, false, 0.0f);

			var maskDrawRect = new CGRect(CGPoint.Empty, maskImage.Size);
			var thumbDrawRect = new CGRect(CGPointExtensions.FromSize(maskImage.Size.Subtract(thumbImage.Size).Divide(2f)), thumbImage.Size);
		
			maskImage.Draw(maskDrawRect, CGBlendMode.Normal, alpha);
			thumbnailImage.Draw(thumbDrawRect, CGBlendMode.Normal, alpha);

			var composedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			// Return results
			return composedImage;
		}
	}
}
