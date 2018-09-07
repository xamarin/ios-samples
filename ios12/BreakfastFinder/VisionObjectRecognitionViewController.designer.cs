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

namespace BreakfastFinder
{
    [Register ("VisionObjectRecognitionViewController")]
    partial class VisionObjectRecognitionViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PreviewView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (PreviewView != null) {
                PreviewView.Dispose ();
                PreviewView = null;
            }
        }
    }
}