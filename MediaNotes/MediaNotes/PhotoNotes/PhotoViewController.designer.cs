// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MediaNotes
{
	[Register ("PhotoViewController")]
	partial class PhotoViewController
	{
		[Outlet]
		UIKit.UIToolbar toolbar { get; set; }

		[Outlet]
		UIKit.UIView placeHolderView { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView placeHolderActivityView { get; set; }

		[Outlet]
		UIKit.UILabel placeHolderLabel { get; set; }

		[Outlet]
		UIKit.UIImageView photoImageView { get; set; }

		[Action ("NextPhoto:")]
		partial void NextPhoto (UIKit.UIBarButtonItem sender);

		[Action ("PreviousPhoto:")]
		partial void PreviousPhoto (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (placeHolderView != null) {
				placeHolderView.Dispose ();
				placeHolderView = null;
			}

			if (placeHolderActivityView != null) {
				placeHolderActivityView.Dispose ();
				placeHolderActivityView = null;
			}

			if (placeHolderLabel != null) {
				placeHolderLabel.Dispose ();
				placeHolderLabel = null;
			}

			if (photoImageView != null) {
				photoImageView.Dispose ();
				photoImageView = null;
			}
		}
	}
}
