using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using ImageIO;
using MapKit;
using CoreLocation;
using CoreImage;

namespace UIKit
{
	public static class UIImageExtensions
	{
		public static UIImage[] ImagesForNames(string[] imageNames) {
			var images = new List<UIImage>();

			foreach (string name in imageNames)
			{
				images.Add(UIImage.FromBundle(name));
			}

			return images.ToArray();
		}

		public static UIImage CroppedImageInRect(this UIImage self, CGRect rect) {
			var cgImage = self.CGImage;
			if (cgImage == null) return null;
			var croppedImage = cgImage.WithImageInRect(rect); 
            return UIImage.FromImage(croppedImage, self.CurrentScale, self.Orientation);
		}

		public static (CGRect rect, string message)[] ComputeQRCodeRectsAndMessages(this UIImage self) {

			var image = new CIImage(self);
			var context = new CIContext(new CIContextOptions());
			var detector = CIDetector.CreateQRDetector(context, new CIDetectorOptions());
			var features = detector.FeaturesInImage(image);
			var results = new List<(CGRect, string)>();

			foreach (CIQRCodeFeature feature in features){
				var message = feature.MessageString;
				if (message != null) {
					// Flip about the x axis to convert from the coordinate space used by CoreGraphics.
					var bounds = feature.Bounds;
					bounds.Y = self.Size.Height - bounds.GetMaxY();
					results.Add((bounds, message));
				}
			}

			return results.ToArray();
		} 

		public static nfloat AspectRatio(this UIImage self) {
			return self.Size.Width / self.Size.Height;
		}
	}
}
