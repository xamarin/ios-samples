// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DisablingPullingDownASheet
{
	[Register ("EditViewController")]
	partial class EditViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem cancelButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem saveButton { get; set; }

		[Outlet]
		UIKit.UITextView textView { get; set; }

		[Action ("Cancel:")]
		partial void Cancel (Foundation.NSObject sender);

		[Action ("Save:")]
		partial void Save (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (saveButton != null) {
				saveButton.Dispose ();
				saveButton = null;
			}

			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}
		}
	}
}
