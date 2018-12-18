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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint textViewBottomLayoutGuideConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (textView != null) {
                textView.Dispose ();
                textView = null;
            }

            if (textViewBottomLayoutGuideConstraint != null) {
                textViewBottomLayoutGuideConstraint.Dispose ();
                textViewBottomLayoutGuideConstraint = null;
            }

            if (view != null) {
                view.Dispose ();
                view = null;
            }
        }
    }
}