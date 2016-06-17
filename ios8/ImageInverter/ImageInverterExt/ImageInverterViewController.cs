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
		}

		[Export ("initWithCoder:")]
		public ImageInverterViewController (NSCoder coder)
			: base (coder)
		{
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
				UIImage img;

				// This is true when you call extension from Photo's ActivityViewController
				var url = image as NSUrl;
				if(url != null) {
					img = UIImage.LoadFromData(NSData.FromUrl(url));
					InitWithImage (img);
					return;
				}

				// This is true when you call extension from Main App
				img = image as UIImage;
				if (img != null) {
					InitWithImage(img);
					return;
				}
			});
		}

		void InitWithImage (UIImage image)
		{
			// Invert the image, enable the Done button
			InvokeOnMainThread (() => {
				// Invert the image
				UIImage invertedImage = Invert(image);

				// Set the inverted image in the UIImageView
				ImageView.Image = invertedImage;
				DoneButton.Enabled = true;
			});
		}

		static UIImage Invert(UIImage originalImage)
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
			var extensionItem = new NSExtensionItem {
				AttributedTitle = new NSAttributedString ("Inverted Image"),
				Attachments = new [] {
					new NSItemProvider (ImageView.Image, UTType.Image)
				}
			};
			ExtensionContext.CompleteRequest(new [] {
				extensionItem
			}, null);
		}
	}
}
