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
    [Register ("SegmentedControlViewController")]
    partial class SegmentedControlViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl customBackgroundSegmentedControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl customSegmentsSegmentedControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl defaultSegmentedControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl tintedSegmentedControl { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (customBackgroundSegmentedControl != null) {
                customBackgroundSegmentedControl.Dispose ();
                customBackgroundSegmentedControl = null;
            }

            if (customSegmentsSegmentedControl != null) {
                customSegmentsSegmentedControl.Dispose ();
                customSegmentsSegmentedControl = null;
            }

            if (defaultSegmentedControl != null) {
                defaultSegmentedControl.Dispose ();
                defaultSegmentedControl = null;
            }

            if (tintedSegmentedControl != null) {
                tintedSegmentedControl.Dispose ();
                tintedSegmentedControl = null;
            }
        }
    }
}