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

        [Action ("beginScrubbing:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void beginScrubbing (UIKit.UISlider sender);

        [Action ("endScrubbing:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void endScrubbing (UIKit.UISlider sender);

        [Action ("exportToMovie:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void exportToMovie (UIKit.UIBarButtonItem sender);

        [Action ("handleTapGesture:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleTapGesture (UIKit.UITapGestureRecognizer sender);

        [Action ("scrub:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void scrub (UIKit.UISlider sender);

        [Action ("togglePlayPause:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void togglePlayPause (UIKit.UIBarButtonItem sender);

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