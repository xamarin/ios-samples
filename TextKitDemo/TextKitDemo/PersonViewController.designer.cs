// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TextKitDemo
{
	[Register ("PersonViewController")]
	partial class PersonViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView descriptionTextView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel labelView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView shortDescriptionTextView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (descriptionTextView != null) {
				descriptionTextView.Dispose ();
				descriptionTextView = null;
			}
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
		}
	}
}
