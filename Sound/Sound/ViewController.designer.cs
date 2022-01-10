// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Sound
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lenghtButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton playButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton startButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel statusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton stopButton { get; set; }

        [Action ("PlayRecorded:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PlayRecorded (UIKit.UIButton sender);

        [Action ("StartRecording:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void StartRecording (UIKit.UIButton sender);

        [Action ("StopRecording:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void StopRecording (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (lenghtButton != null) {
                lenghtButton.Dispose ();
                lenghtButton = null;
            }

            if (playButton != null) {
                playButton.Dispose ();
                playButton = null;
            }

            if (startButton != null) {
                startButton.Dispose ();
                startButton = null;
            }

            if (statusLabel != null) {
                statusLabel.Dispose ();
                statusLabel = null;
            }

            if (stopButton != null) {
                stopButton.Dispose ();
                stopButton = null;
            }
        }
    }
}