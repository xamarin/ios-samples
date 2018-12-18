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
    [Register ("ProgressViewController")]
    partial class ProgressViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView barStyleProgressView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView defaultStyleProgressView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView tintedProgressView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (barStyleProgressView != null) {
                barStyleProgressView.Dispose ();
                barStyleProgressView = null;
            }

            if (defaultStyleProgressView != null) {
                defaultStyleProgressView.Dispose ();
                defaultStyleProgressView = null;
            }

            if (tintedProgressView != null) {
                tintedProgressView.Dispose ();
                tintedProgressView = null;
            }
        }
    }
}