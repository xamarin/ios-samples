// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ARMultiuser
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel mappingStatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ARKit.ARSCNView sceneView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ARMultiuser.RoundedButton sendMapButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel sessionInfoLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIVisualEffectView sessionInfoView { get; set; }

        [Action ("handleSceneTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleSceneTap (UIKit.UITapGestureRecognizer sender);

        [Action ("resetTracking:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void resetTracking (UIKit.UIButton sender);

        [Action ("shareSession:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void shareSession (ARMultiuser.RoundedButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (mappingStatusLabel != null) {
                mappingStatusLabel.Dispose ();
                mappingStatusLabel = null;
            }

            if (sceneView != null) {
                sceneView.Dispose ();
                sceneView = null;
            }

            if (sendMapButton != null) {
                sendMapButton.Dispose ();
                sendMapButton = null;
            }

            if (sessionInfoLabel != null) {
                sessionInfoLabel.Dispose ();
                sessionInfoLabel = null;
            }

            if (sessionInfoView != null) {
                sessionInfoView.Dispose ();
                sessionInfoView = null;
            }
        }
    }
}