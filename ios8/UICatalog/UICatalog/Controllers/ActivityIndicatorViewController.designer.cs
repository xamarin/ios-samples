// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace UICatalog
{
    [Register ("ActivityIndicatorViewController")]
    partial class ActivityIndicatorViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView tintedActivityIndicatorView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tintedActivityIndicatorView != null) {
                tintedActivityIndicatorView.Dispose ();
                tintedActivityIndicatorView = null;
            }
        }
    }
}