// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace KannadaKeyboard
{
	[Register ("KeyboardViewController")]
	partial class KeyboardViewController
	{
		[Outlet]
		UIKit.UIView Row1 { get; set; }

		[Outlet]
		UIKit.UIView Row2 { get; set; }

		[Outlet]
		UIKit.UIView Row3 { get; set; }

		[Outlet]
		UIKit.UIView Row4 { get; set; }

		[Outlet]
		UIKit.UIView Row5 { get; set; }

		[Outlet]
		UIKit.UIButton Shift { get; set; }

		[Action ("BackspacePressed:")]
		partial void BackspacePressed (Foundation.NSObject sender);

		[Action ("ChangeKeyboardPressed:")]
		partial void ChangeKeyboardPressed (Foundation.NSObject sender);

		[Action ("KeyPress:")]
		partial void KeyPress (Foundation.NSObject sender);

		[Action ("ReturnPressed:")]
		partial void ReturnPressed (Foundation.NSObject sender);

		[Action ("SpacePressed:")]
		partial void SpacePressed (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Row1 != null) {
				Row1.Dispose ();
				Row1 = null;
			}

			if (Row2 != null) {
				Row2.Dispose ();
				Row2 = null;
			}

			if (Row3 != null) {
				Row3.Dispose ();
				Row3 = null;
			}

			if (Row4 != null) {
				Row4.Dispose ();
				Row4 = null;
			}

			if (Row5 != null) {
				Row5.Dispose ();
				Row5 = null;
			}

			if (Shift != null) {
				Shift.Dispose ();
				Shift = null;
			}
		}
	}
}
