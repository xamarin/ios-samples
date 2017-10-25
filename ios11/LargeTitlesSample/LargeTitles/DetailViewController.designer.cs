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

namespace largetitles
{
    [Register ("DetailViewController")]
    partial class DetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LargeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LargeLabel != null) {
                LargeLabel.Dispose ();
                LargeLabel = null;
            }
        }
    }
}