// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
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
            if (textBottomConstraint != null) {
                textBottomConstraint.Dispose ();
                textBottomConstraint = null;
            }

            if (textView != null) {
                textView.Dispose ();
                textView = null;
            }
        }
    }
}