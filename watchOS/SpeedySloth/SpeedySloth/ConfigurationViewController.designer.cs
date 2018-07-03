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

namespace SpeedySloth
{
    [Register ("ConfigurationViewController")]
    partial class ConfigurationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView ActivityTypePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView LocationTypePicker { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ActivityTypePicker != null) {
                ActivityTypePicker.Dispose ();
                ActivityTypePicker = null;
            }

            if (LocationTypePicker != null) {
                LocationTypePicker.Dispose ();
                LocationTypePicker = null;
            }
        }
    }
}