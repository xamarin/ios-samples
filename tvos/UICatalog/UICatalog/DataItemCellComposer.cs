using System;
using System.Collections.Generic;

using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace UICatalog {
	public class DataItemCellComposer {
		readonly static NSCache processedImageCache = new NSCache ();

		readonly Dictionary <DataItemCollectionViewCell, NSOperationQueue> operationQueues = new Dictionary<DataItemCollectionViewCell, NSOperationQueue> ();

		public void ComposeCell (DataItemCollectionViewCell cell, DataItem dataItem)
		{
			var operationQueue = OperationQueueForCell (cell);
			operationQueue.CancelAllOperations ();

			cell.RepresentedDataItem = dataItem;
			cell.Label.Text = dataItem.Title;
			cell.ImageView.Alpha = 1f;
			cell.ImageView.Image = (UIImage)processedImageCache.ObjectForKey ((NSString)dataItem.Identifier);

			if (cell.ImageView.Image != null)
				return;

			var processImageOperation = new NSBlockOperation ();
			processImageOperation.AddExecutionBlock (() => {
				if (processImageOperation.IsCancelled)
					return;

				UIImage image = null;
				DispatchQueue.MainQueue.DispatchSync (() => {
					image = ProcessImageNamed (dataItem.ImageName);
				});

				if (image == null)
					return;

				processedImageCache.SetObjectforKey (image, (NSString)dataItem.Identifier);
				NSOperationQueue.MainQueue.AddOperation (() => {
					if (cell.RepresentedDataItem == null)
						return;

					cell.ImageView.Alpha = 0f;
					cell.ImageView.Image = image;
					UIView.Animate (0.25, () => cell.ImageView.Alpha = 1f);
				});
			});

			operationQueue.AddOperation (processImageOperation);
		}

		NSOperationQueue OperationQueueForCell (DataItemCollectionViewCell cell)
		{
			NSOperationQueue queue;
			if (operationQueues.TryGetValue (cell, out queue))
				return queue;

			queue = new NSOperationQueue ();
			operationQueues.Add (cell, queue);
			return queue;
		}

		static UIImage ProcessImageNamed (string imageName)
		{
			var image = UIImage.FromBundle (imageName);

			if (image == null)
				return null;

			if (imageName.Contains (".jpg"))
				return image;

			UIImage resultingImage;

			using (var colorSpace = CGColorSpace.CreateDeviceRGB ()) {
				// Create a bitmap context of the same size as the image.
				var imageWidth = (int)image.Size.Width;
				var imageHeight = (int)image.Size.Height;

				using (var bitmapContext = new CGBitmapContext (null, imageWidth, imageHeight, 8, imageHeight * 4,
										colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Little)) {

					// Draw the image into the graphics context.
					if (image.CGImage == null)
						throw new Exception ("Unable to get a CGImage from a UIImage.");

					bitmapContext.DrawImage (new CGRect (CGPoint.Empty, image.Size), image.CGImage);
					using (var newImageRef = bitmapContext.ToImage ()) {
						resultingImage = new UIImage (newImageRef);
					}
				}
			}

			return resultingImage;
		}
	}
}

