// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace PrintBanner
{
    [Register ("PrintBannerViewController")]
    partial class PrintBannerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl colorSelection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl fontSelection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton printButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textField { get; set; }

        [Action ("Print:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Print (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (colorSelection != null) {
                colorSelection.Dispose ();
                colorSelection = null;
            }

            if (fontSelection != null) {
                fontSelection.Dispose ();
                fontSelection = null;
            }

            if (printButton != null) {
                printButton.Dispose ();
                printButton = null;
            }

            if (textField != null) {
                textField.Dispose ();
                textField = null;
            }
        }
    }
}