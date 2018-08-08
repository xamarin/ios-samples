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
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIVisualEffectView blurView { get; set; }


        [Outlet]
        ScanningAndDetecting3DObjects.FlashlightButton flashlightButton { get; set; }


        [Outlet]
        ScanningAndDetecting3DObjects.MessageLabel instructionLabel { get; set; }


        [Outlet]
        UIKit.UILabel instructionView { get; set; }


        [Outlet]
        ScanningAndDetecting3DObjects.RoundedButton loadModelButton { get; set; }


        [Outlet]
        UIKit.UINavigationBar navigationBar { get; set; }


        [Outlet]
        UIKit.UIButton nextButton { get; set; }


        [Outlet]
        ARKit.ARSCNView sceneView { get; set; }


        [Outlet]
        UIKit.UILabel sessionInfoLabel { get; set; }


        [Outlet]
        UIKit.UIVisualEffectView sessionInfoView { get; set; }


        [Outlet]
        ScanningAndDetecting3DObjects.RoundedButton toggleInstructionsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ARKit.ARSCNView scnView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITapGestureRecognizer tapGestureRecognizer { get; set; }


        [Action ("toggleInstructionsButtonTapped:")]
        partial void ToggleInstructionsButtonTapped (Foundation.NSObject sender);


        [Action ("leftButtonTouchAreaTapped:")]
        partial void LeftButtonTouchAreaTapped (Foundation.NSObject sender);


        [Action ("loadModelButtonTapped:")]
        partial void LoadModelButtonTouched (Foundation.NSObject sender);

        [Action ("didLongPress:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void didLongPress (UIKit.UILongPressGestureRecognizer sender);

        [Action ("didOneFingerPan:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void didOneFingerPan (UIKit.UIPanGestureRecognizer sender);

        [Action ("didPinch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void didPinch (ScanningAndDetecting3DObjects.ThresholdPinchGestureRecognizer sender);

        [Action ("didRotate:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void didRotate (ScanningAndDetecting3DObjects.ThresholdRotationGestureRecognizer sender);

        [Action ("didTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void didTap (UIKit.UITapGestureRecognizer sender);

        [Action ("didTwoFingerPan:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void didTwoFingerPan (ScanningAndDetecting3DObjects.ThresholdPanGestureRecognizer sender);

        [Action ("NextButtonPressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void NextButtonPressed (ScanningAndDetecting3DObjects.RoundedButton sender);

        [Action ("ToggleFlashlightButtonTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ToggleFlashlightButtonTapped (ScanningAndDetecting3DObjects.FlashlightButton sender);

        [Action ("toggleInstructionsButtonTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void toggleInstructionsButtonTapped (ScanningAndDetecting3DObjects.RoundedButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (blurView != null) {
                blurView.Dispose ();
                blurView = null;
            }

            if (flashlightButton != null) {
                flashlightButton.Dispose ();
                flashlightButton = null;
            }

            if (instructionLabel != null) {
                instructionLabel.Dispose ();
                instructionLabel = null;
            }

            if (instructionView != null) {
                instructionView.Dispose ();
                instructionView = null;
            }

            if (loadModelButton != null) {
                loadModelButton.Dispose ();
                loadModelButton = null;
            }

            if (navigationBar != null) {
                navigationBar.Dispose ();
                navigationBar = null;
            }

            if (nextButton != null) {
                nextButton.Dispose ();
                nextButton = null;
            }

            if (sceneView != null) {
                sceneView.Dispose ();
                sceneView = null;
            }

            if (scnView != null) {
                scnView.Dispose ();
                scnView = null;
            }

            if (sessionInfoLabel != null) {
                sessionInfoLabel.Dispose ();
                sessionInfoLabel = null;
            }

            if (sessionInfoView != null) {
                sessionInfoView.Dispose ();
                sessionInfoView = null;
            }

            if (tapGestureRecognizer != null) {
                tapGestureRecognizer.Dispose ();
                tapGestureRecognizer = null;
            }

            if (toggleInstructionsButton != null) {
                toggleInstructionsButton.Dispose ();
                toggleInstructionsButton = null;
            }
        }
    }
}