// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace AVCustomEdit
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel currentTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem exportButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView exportProgressView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        AVCustomEdit.PlayerView playerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem playPauseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider scrubber { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIToolbar toolbar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem transitionButton { get; set; }

        [Action ("BeginScrubbing:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BeginScrubbing (UIKit.UISlider sender);

        [Action ("EndScrubbing:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EndScrubbing (UIKit.UISlider sender);

        [Action ("ExportToMovie:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ExportToMovie (UIKit.UIBarButtonItem sender);

        [Action ("HandleTapGesture:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleTapGesture (UIKit.UITapGestureRecognizer sender);

        [Action ("Scrub:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Scrub (UIKit.UISlider sender);

        [Action ("TogglePlayPause:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TogglePlayPause (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (currentTimeLabel != null) {
                currentTimeLabel.Dispose ();
                currentTimeLabel = null;
            }

            if (exportButton != null) {
                exportButton.Dispose ();
                exportButton = null;
            }

            if (exportProgressView != null) {
                exportProgressView.Dispose ();
                exportProgressView = null;
            }

            if (playerView != null) {
                playerView.Dispose ();
                playerView = null;
            }

            if (playPauseButton != null) {
                playPauseButton.Dispose ();
                playPauseButton = null;
            }

            if (scrubber != null) {
                scrubber.Dispose ();
                scrubber = null;
            }

            if (toolbar != null) {
                toolbar.Dispose ();
                toolbar = null;
            }

            if (transitionButton != null) {
                transitionButton.Dispose ();
                transitionButton = null;
            }
        }
    }
}