// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("TextViewController")]
	partial class TextViewController
	{
		[Outlet]
		UIKit.NSLayoutConstraint textBottomConstraint { get; set; }

		[Outlet]
		UIKit.UITextView textView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}

			if (textBottomConstraint != null) {
				textBottomConstraint.Dispose ();
				textBottomConstraint = null;
			}
		}
	}
}
