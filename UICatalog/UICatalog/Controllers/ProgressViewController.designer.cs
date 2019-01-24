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
        UIKit.UIProgressView barProgressView { get; set; }


        [Outlet]
        UIKit.UIProgressView defaultProgressView { get; set; }


        [Outlet]
        UIKit.UIProgressView tintedProgressView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (barProgressView != null) {
                barProgressView.Dispose ();
                barProgressView = null;
            }

            if (defaultProgressView != null) {
                defaultProgressView.Dispose ();
                defaultProgressView = null;
            }

            if (tintedProgressView != null) {
                tintedProgressView.Dispose ();
                tintedProgressView = null;
            }
        }
    }
}