// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ScanningAndDetecting3DObjects
{
    [Register ("ThresholdRotationGestureRecognizer")]
    partial class ThresholdRotationGestureRecognizer
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ScanningAndDetecting3DObjects.ViewController @delegate { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (@delegate is not null) {
                @delegate.Dispose ();
                @delegate = null;
            }
        }
    }
}