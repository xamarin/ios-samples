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

namespace SoupChef
{
    [Register ("QuantityCell")]
    partial class QuantityCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel QuantityLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper Stepper { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (QuantityLabel != null) {
                QuantityLabel.Dispose ();
                QuantityLabel = null;
            }

            if (Stepper != null) {
                Stepper.Dispose ();
                Stepper = null;
            }
        }
    }
}