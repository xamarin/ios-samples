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

namespace PickerControl
{
    [Register ("PickerViewController")]
    partial class PickerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel personLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView personPicker { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (personLabel != null) {
                personLabel.Dispose ();
                personLabel = null;
            }

            if (personPicker != null) {
                personPicker.Dispose ();
                personPicker = null;
            }
        }
    }
}