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
    [Register ("DatePickerController")]
    partial class DatePickerController
    {
        [Outlet]
        UIKit.UILabel dateLabel { get; set; }


        [Outlet]
        UIKit.UIDatePicker datePicker { get; set; }


        [Action ("PickerValurChanged:")]
        partial void PickerValurChanged (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (dateLabel != null) {
                dateLabel.Dispose ();
                dateLabel = null;
            }

            if (datePicker != null) {
                datePicker.Dispose ();
                datePicker = null;
            }
        }
    }
}