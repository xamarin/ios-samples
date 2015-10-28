// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("TextFieldsViewController")]
	partial class TextFieldsViewController
	{
		[Outlet]
		UIKit.UITextField EmailKeyboardTextField { get; set; }

		[Outlet]
		UIKit.UITextField NumberPadTextField { get; set; }

		[Outlet]
		UIKit.UITextField NumbersAndPunctuationTextField { get; set; }

		[Outlet]
		UIKit.UITextField RegularKeyboardTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (RegularKeyboardTextField != null) {
				RegularKeyboardTextField.Dispose ();
				RegularKeyboardTextField = null;
			}

			if (EmailKeyboardTextField != null) {
				EmailKeyboardTextField.Dispose ();
				EmailKeyboardTextField = null;
			}

			if (NumberPadTextField != null) {
				NumberPadTextField.Dispose ();
				NumberPadTextField = null;
			}

			if (NumbersAndPunctuationTextField != null) {
				NumbersAndPunctuationTextField.Dispose ();
				NumbersAndPunctuationTextField = null;
			}
		}
	}
}
