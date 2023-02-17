using Foundation;
using UIKit;

namespace ScanningAndDetecting3DObjects {
	// Customized share sheet for exporting scanned AR reference objects.
	internal class ShareScanViewController : UIActivityViewController {
		internal ShareScanViewController (UIView sourceView, NSObject any)
			: base (new [] { any }, null)
		{
			// Set up popover presentation style
			ModalPresentationStyle = UIModalPresentationStyle.Popover;
			PopoverPresentationController.SourceView = sourceView;
			PopoverPresentationController.SourceRect = sourceView.Bounds;

			ExcludedActivityTypes = new []
			{
				UIActivityType.MarkupAsPdf,
				UIActivityType.OpenInIBooks,
				UIActivityType.Message,
				UIActivityType.Print,
				UIActivityType.AddToReadingList,
				UIActivityType.SaveToCameraRoll,
				UIActivityType.AssignToContact,
				UIActivityType.CopyToPasteboard,
				UIActivityType.PostToTencentWeibo,
				UIActivityType.PostToWeibo,
				UIActivityType.PostToVimeo,
				UIActivityType.PostToFlickr,
				UIActivityType.PostToTwitter,
				UIActivityType.PostToFacebook
			};
		}
	}
}
