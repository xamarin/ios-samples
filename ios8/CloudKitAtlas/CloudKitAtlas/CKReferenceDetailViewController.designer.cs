// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("CKReferenceDetailViewController")]
	partial class CKReferenceDetailViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AddButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField nameTextField { get; set; }

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
