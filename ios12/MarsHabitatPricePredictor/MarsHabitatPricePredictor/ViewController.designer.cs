// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MarsHabitatPricePredictor
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView pickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel priceLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (pickerView != null) {
                pickerView.Dispose ();
                pickerView = null;
            }

            if (priceLabel != null) {
                priceLabel.Dispose ();
                priceLabel = null;
            }
        }
    }
}