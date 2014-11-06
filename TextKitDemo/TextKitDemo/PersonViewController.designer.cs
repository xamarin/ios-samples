// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TextKitDemo
{
	[Register ("PersonViewController")]
	partial class PersonViewController
	{
		[Outlet]
		UIKit.UITextView descriptionTextView { get; set; }

		[Outlet]
		UIKit.UIImageView imageView { get; set; }

		[Outlet]
		UIKit.UILabel labelView { get; set; }

		[Outlet]
		UIKit.UITextView shortDescriptionTextView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (labelView != null) {
				labelView.Dispose ();
				labelView = null;
			}

			if (shortDescriptionTextView != null) {
				shortDescriptionTextView.Dispose ();
				shortDescriptionTextView = null;
			}

			if (descriptionTextView != null) {
				descriptionTextView.Dispose ();
				descriptionTextView = null;
			}
		}
	}
}
