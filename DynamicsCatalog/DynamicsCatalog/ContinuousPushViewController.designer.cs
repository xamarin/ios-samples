// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace DynamicsCatalog
{
    [Register ("ContinuousPushViewController")]
    partial class ContinuousPushViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView redSquare { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView square { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (redSquare != null) {
                redSquare.Dispose ();
                redSquare = null;
            }

            if (square != null) {
                square.Dispose ();
                square = null;
            }
        }
    }
}