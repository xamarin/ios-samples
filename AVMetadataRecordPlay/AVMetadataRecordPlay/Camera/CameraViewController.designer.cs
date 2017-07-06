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

namespace AVMetadataRecordPlay.Camera
{
    [Register ("CameraViewController")]
    partial class CameraViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton cameraButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel cameraUnavailableLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem playerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        PreviewView previewView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton recordButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton resumeButton { get; set; }

        [Action ("ChangeCamera:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChangeCamera (UIKit.UIButton sender);

        [Action ("focusAndExposeTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void focusAndExposeTap (UIKit.UITapGestureRecognizer sender);

        [Action ("ResumeInterruptedSession:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ResumeInterruptedSession (UIKit.UIButton sender);

        [Action ("ToggleMovieRecording:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ToggleMovieRecording (UIKit.UIButton sender);

        [Action ("UIBarButtonItem156_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIBarButtonItem156_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (cameraButton != null) {
                cameraButton.Dispose ();
                cameraButton = null;
            }

            if (cameraUnavailableLabel != null) {
                cameraUnavailableLabel.Dispose ();
                cameraUnavailableLabel = null;
            }

            if (playerButton != null) {
                playerButton.Dispose ();
                playerButton = null;
            }

            if (previewView != null) {
                previewView.Dispose ();
                previewView = null;
            }

            if (recordButton != null) {
                recordButton.Dispose ();
                recordButton = null;
            }

            if (resumeButton != null) {
                resumeButton.Dispose ();
                resumeButton = null;
            }
        }
    }
}