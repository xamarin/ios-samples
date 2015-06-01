// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace DocPicker
{
	[Register ("DocPickerViewController")]
	partial class DocPickerViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem ActionButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView DocumentText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem SaveButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ActionButton != null) {
				ActionButton.Dispose ();
				ActionButton = null;
			}
			if (DocumentText != null) {
				DocumentText.Dispose ();
				DocumentText = null;
			}
			if (SaveButton != null) {
				SaveButton.Dispose ();
				SaveButton = null;
			}
		}
	}
}
