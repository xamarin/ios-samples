using System;

using Foundation;
using UIKit;
using MobileCoreServices;

namespace ImageInverter
{
	[Register ("ImageShareViewController")]
	public class ImageShareViewController : UIViewController
	{
		[Outlet("imageView")]
		public UIImageView ImageView { get; set; }

		[Outlet("shareItem")]
		public UIBarButtonItem ShareItem { get; set; }

		public ImageShareViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ImageShareViewController(NSCoder coder)
			: base (coder)
		{
		}

		[Action("share:")]
		void OnShareClicked(UIBarButtonItem button)
		{
			var activityViewController = new UIActivityViewController (new [] {
				ImageView.Image
			}, null);
			var popover = activityViewController.PopoverPresentationController;
			if (popover != null) {
				popover.BarButtonItem = ShareItem;
			}

			// Set a completion handler to handle what the UIActivityViewController returns
			activityViewController.SetCompletionHandler ((activityType, completed, returnedItems, error) => {
				if (returnedItems == null || returnedItems.Length == 0)
					return;

				NSExtensionItem extensionItem = returnedItems [0];
				NSItemProvider imageItemProvider = extensionItem.Attachments [0];

				if (!imageItemProvider.HasItemConformingTo(UTType.Image))
					return;

				imageItemProvider.LoadItem (UTType.Image, null, (item, loadError) => {
					if (item != null && loadError == null)
						InvokeOnMainThread (() => {
							ImageView.Image = (UIImage)item;
						});
				});
			});

			PresentViewController (activityViewController, true, null);
		}
	}
}