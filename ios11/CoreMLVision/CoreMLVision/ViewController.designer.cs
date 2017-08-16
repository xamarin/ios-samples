// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CoreMLVision
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem CameraButton { get; set; }

		[Outlet]
		UIKit.UILabel ClassificationLabel { get; set; }

		[Outlet]
		UIKit.UIImageView CorrectedImageView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem GalleryButton { get; set; }

		[Outlet]
		UIKit.UIImageView ImageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (CorrectedImageView != null) {
				CorrectedImageView.Dispose ();
				CorrectedImageView = null;
			}

			if (ClassificationLabel != null) {
				ClassificationLabel.Dispose ();
				ClassificationLabel = null;
			}

			if (CameraButton != null) {
				CameraButton.Dispose ();
				CameraButton = null;
			}

			if (GalleryButton != null) {
				GalleryButton.Dispose ();
				GalleryButton = null;
			}
		}
	}
}
