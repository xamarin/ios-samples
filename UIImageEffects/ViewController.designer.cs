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

namespace UIImageEffects
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel effectLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (effectLabel != null) {
                effectLabel.Dispose ();
                effectLabel = null;
            }

            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }
        }
    }
}