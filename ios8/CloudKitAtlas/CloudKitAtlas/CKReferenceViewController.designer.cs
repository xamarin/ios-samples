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

namespace CloudKitAtlas
{
	[Register ("CKReferenceViewController")]
	partial class CKReferenceViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AddButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField nameTextField { get; set; }

		[Action ("add:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void add (UIButton sender);

		[Action ("Add:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Add (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}
			if (nameTextField != null) {
				nameTextField.Dispose ();
				nameTextField = null;
			}
		}
	}
}
