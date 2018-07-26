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
    [Register ("DatePickerViewController")]
    partial class DatePickerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel dateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl datePickerMode { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIDatePicker datePickerView { get; set; }

        [Action ("DateModeValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DateModeValueChanged (UIKit.UISegmentedControl sender);

        [Action ("DateTimeChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DateTimeChanged (UIKit.UIDatePicker sender);

        void ReleaseDesignerOutlets ()
        {
            if (dateLabel != null) {
                dateLabel.Dispose ();
                dateLabel = null;
            }

            if (datePickerMode != null) {
                datePickerMode.Dispose ();
                datePickerMode = null;
            }

            if (datePickerView != null) {
                datePickerView.Dispose ();
                datePickerView = null;
            }
        }
    }
}