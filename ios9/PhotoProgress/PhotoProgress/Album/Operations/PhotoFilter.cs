using CoreImage;
using Foundation;
using UIKit;

namespace PhotoProgress {
	public static class PhotoFilter {

		public static UIImage FilteredImage (UIImage image)
		{
			var progress = NSProgress.FromTotalUnitCount (-1);
			progress.Cancellable = false;
			progress.Pausable = false;

			UIImage outputImage;

			var filter = new CIPhotoEffectTransfer ();

			var cgImage = image.CGImage;
			var ciImage = CIImage.FromCGImage (cgImage);
			filter.SetValueForKey (ciImage, new NSString ("inputImage"));
			var outputCIImage = filter.OutputImage;
			var ciContext = CIContext.Create ();
			var outputCGImage = ciContext.CreateCGImage (outputCIImage, outputCIImage.Extent);
			outputImage = UIImage.FromImage (outputCGImage);

			outputCGImage.Dispose ();
			ciContext.Dispose ();
			outputCIImage.Dispose ();
			ciImage.Dispose ();
			cgImage.Dispose ();
			filter.Dispose ();

			progress.CompletedUnitCount = 1;
			progress.TotalUnitCount = 1;

			return outputImage;
		}
	}
}

