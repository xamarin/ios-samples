// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace VisionML
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel classificationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView correctedImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageView { get; set; }

        [Action ("ChooseImage:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChooseImage (UIKit.UIBarButtonItem sender);

        [Action ("TakePicture:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TakePicture (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (classificationLabel != null) {
                classificationLabel.Dispose ();
                classificationLabel = null;
            }

            if (correctedImageView != null) {
                correctedImageView.Dispose ();
                correctedImageView = null;
            }

            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }
        }
    }
}