// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("CKReferenceViewController")]
	partial class CKReferenceViewController
	{
		[Outlet]
		UIKit.UIButton AddButton { get; set; }

		[Outlet]
		UIKit.UITextField nameTextField { get; set; }

		[Action ("Add:")]
		partial void Add (UIKit.UIButton sender);

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
