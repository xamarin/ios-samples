// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace EvenOddNumberDrag
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EvenNumbersLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NumberLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OddNumbersLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EvenNumbersLabel != null) {
                EvenNumbersLabel.Dispose ();
                EvenNumbersLabel = null;
            }

            if (NumberLabel != null) {
                NumberLabel.Dispose ();
                NumberLabel = null;
            }

            if (OddNumbersLabel != null) {
                OddNumbersLabel.Dispose ();
                OddNumbersLabel = null;
            }
        }
    }
}