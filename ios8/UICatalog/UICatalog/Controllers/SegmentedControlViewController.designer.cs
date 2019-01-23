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
        UIKit.UISegmentedControl customSegmentControl { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (customSegmentControl != null) {
                customSegmentControl.Dispose ();
                customSegmentControl = null;
            }
        }
    }
}