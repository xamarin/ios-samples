// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace AsyncDownloadImage
{
	[Register ("AsyncDownloadImageViewController")]
	partial class AsyncDownloadImageViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton clickButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton downloadButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIProgressView downloadProgress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel infoLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (clickButton != null) {
				clickButton.Dispose ();
				clickButton = null;
			}

			if (downloadButton != null) {
				downloadButton.Dispose ();
				downloadButton = null;
			}

			if (downloadProgress != null) {
				downloadProgress.Dispose ();
				downloadProgress = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (infoLabel != null) {
				infoLabel.Dispose ();
				infoLabel = null;
			}
		}
	}
}
