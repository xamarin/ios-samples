// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MediaNotes
{
	[Register ("PhotoViewController")]
	partial class PhotoViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView placeHolderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView placeHolderActivityView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel placeHolderLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView photoImageView { get; set; }

		[Action ("NextPhoto:")]
		partial void NextPhoto (MonoTouch.UIKit.UIBarButtonItem sender);

		[Action ("PreviousPhoto:")]
		partial void PreviousPhoto (MonoTouch.UIKit.UIBarButtonItem sender);
		
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
