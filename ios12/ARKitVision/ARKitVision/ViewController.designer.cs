// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ARKitVision
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITapGestureRecognizer gestureRecognizers { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ARKit.ARSKView sceneView { get; set; }

        [Action ("placeLabelInLocationWithSender:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void placeLabelInLocationWithSender (UIKit.UITapGestureRecognizer sender);

        void ReleaseDesignerOutlets ()
        {
            if (gestureRecognizers != null) {
                gestureRecognizers.Dispose ();
                gestureRecognizers = null;
            }

            if (sceneView != null) {
                sceneView.Dispose ();
                sceneView = null;
            }
        }
    }
}