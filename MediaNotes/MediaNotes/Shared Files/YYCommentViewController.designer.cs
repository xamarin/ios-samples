// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MediaNotes
{
	[Register ("YYCommentViewController")]
	partial class YYCommentViewController
	{
		[Outlet]
		UIKit.UITextView textView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem shareButton { get; set; }

		[Outlet]
		UIKit.UIToolbar toolbar { get; set; }

		[Action ("enableTextEditing:")]
		partial void enableTextEditing (UIKit.UIBarButtonItem sender);

		[Action ("share:")]
		partial void share (UIKit.UIBarButtonItem sender);

		[Action ("shootPicture:")]
		partial void shootPicture (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}

			if (shareButton != null) {
				shareButton.Dispose ();
				shareButton = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}
}
