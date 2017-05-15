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

namespace FilterDemoApp
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView auContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider cutoffSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField cutoffTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton playButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider resonanceSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField resonanceTextField { get; set; }

        [Action ("changedCutoff:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void changedCutoff (UIKit.UISlider sender);

        [Action ("changedCutoff:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void changedCutoff (UIKit.UITextField sender);

        [Action ("changedResonance:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void changedResonance (UIKit.UITextField sender);

        [Action ("changedResonance:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void changedResonance (UIKit.UISlider sender);

        [Action ("togglePlay:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void togglePlay (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (auContainerView != null) {
                auContainerView.Dispose ();
                auContainerView = null;
            }

            if (cutoffSlider != null) {
                cutoffSlider.Dispose ();
                cutoffSlider = null;
            }

            if (cutoffTextField != null) {
                cutoffTextField.Dispose ();
                cutoffTextField = null;
            }

            if (playButton != null) {
                playButton.Dispose ();
                playButton = null;
            }

            if (resonanceSlider != null) {
                resonanceSlider.Dispose ();
                resonanceSlider = null;
            }

            if (resonanceTextField != null) {
                resonanceTextField.Dispose ();
                resonanceTextField = null;
            }
        }
    }
}