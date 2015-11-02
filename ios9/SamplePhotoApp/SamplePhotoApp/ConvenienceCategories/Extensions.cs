using CoreGraphics;
using Foundation;
using UIKit;
using CoreImage;
using OpenGLES;

namespace SamplePhotoApp {
	public static class Extensions {
		public static NSIndexPath[] GetIndexPaths (this NSIndexSet indexSet, int section)
		{
			var indexPaths = new NSIndexPath [indexSet.Count];
			for (int i = 0; i < indexPaths.Length; i++)
				indexPaths [i] = NSIndexPath.FromItemSection (i, section);

			return indexPaths;
		}

		public static NSIndexPath[] GetIndexPaths (this UICollectionView collectionView, CGRect rect)
		{
			var allLayoutAttributes = collectionView.CollectionViewLayout.LayoutAttributesForElementsInRect (rect);
			if (allLayoutAttributes.Length == 0)
				return null;

			var indexPaths = new NSIndexPath[allLayoutAttributes.Length];
			for (int i = 0; i < allLayoutAttributes.Length; i++) {
				var indexPath = allLayoutAttributes [i].IndexPath;
				indexPaths [i] = indexPath;
			}

			return indexPaths;
		}

		static CIContext ciContext;
		public static NSData GetJpegRepresentation (this CIImage image, float compressionQuality)
		{
			if (ciContext == null) {
				var eaglContext = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
				ciContext = CIContext.FromContext (eaglContext);
			}

			using (CGImage outputImageRef = ciContext.CreateCGImage (image, image.Extent)) {
				using (UIImage uiImage = UIImage.FromImage (outputImageRef, 1f, UIImageOrientation.Up)) {
					return uiImage.AsJPEG (compressionQuality);
				}
			}
		}
	}
}

