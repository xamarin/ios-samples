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