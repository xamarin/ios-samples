using System;
using System.Drawing;

using Foundation;
using UIKit;
using MobileCoreServices;
using CoreGraphics;

namespace ImageInverterExt
{
	public partial class ImageInverterViewController : UIViewController
	{
		public ImageInverterViewController (IntPtr handle)
			: base (handle)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("ImageInverterViewController called");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Enable the Cancel button, disable the Done button
			CancelButton.Enabled = true;
			DoneButton.Enabled = false;

			// Verify that we have a valid NSExtensionItem
			NSExtensionItem imageItem = ExtensionContext.InputItems [0];
			if(imageItem == null)
				return;

			// Verify that we have a valid NSItemProvider
			NSItemProvider imageItemProvider = imageItem.Attachments [0];
			if(imageItemProvider == null)
				return;

			// Look for an image inside the NSItemProvider
			if (!imageItemProvider.HasItemConformingTo (UTType.Image))
				return;

			imageItemProvider.LoadItem (UTType.Image, null, (NSObject image, NSError error) => {
				if (image == null)
					return;

				// Invert the image, enable the Done button
				InvokeOnMainThread (() => {
					// Invert the image
					UIImage invertedImage = InvertedImage((UIImage)image);

					// Set the inverted image in the UIImageView
					ImageView.Image = invertedImage;
					DoneButton.Enabled = true;
				});
			});
		}

		private UIImage InvertedImage(UIImage originalImage)
		{
			// Invert the image by applying an affine transformation
			UIGraphics.BeginImageContext (originalImage.Size);

			// Apply an affine transformation to the original image to generate a vertically flipped image
			CGContext context = UIGraphics.GetCurrentContext ();
			var affineTransformationInvert = new CGAffineTransform(1, 0, 0, -1, 0, originalImage.Size.Height);
			context.ConcatCTM(affineTransformationInvert);
			originalImage.Draw (PointF.Empty);

			UIImage invertedImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return invertedImage;
		}

		partial void OnCancelClicked (UIButton sender)
		{
			// Cancel the request
			ExtensionContext.CancelRequest(new NSError((NSString)"ImageInverterErrorDomain", 0));
		}

		partial void OnDoneClicked (UIBarButtonItem sender)
		{
			// Create the NSExtensionItem and NSItemProvider in which we return the image
			NSExtensionItem extensionItem = new NSExtensionItem {
				AttributedTitle = new NSAttributedString ("Inverted Image"),
				Attachments = new NSItemProvider[] {
					new NSItemProvider (ImageView.Image, UTType.Image)
				}
			};
			ExtensionContext.CompleteRequest(new NSExtensionItem[] {
				extensionItem
			}, null);
		}
	}
}
