// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SpeedySloth.WatchAppExtension
{
    [Register ("WorkoutInterfaceController")]
    partial class WorkoutInterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel CaloriesLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel DistanceLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel DurationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceButton MarkerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel MarkerLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceButton PauseResumeButton { get; set; }

        [Action ("DidTapMarkerButton")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DidTapMarkerButton ();

        [Action ("DidTapPauseResumeButton")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DidTapPauseResumeButton ();

        [Action ("DidTapStopButton")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DidTapStopButton ();

        void ReleaseDesignerOutlets ()
        {
            if (CaloriesLabel != null) {
                CaloriesLabel.Dispose ();
                CaloriesLabel = null;
            }

            if (DistanceLabel != null) {
                DistanceLabel.Dispose ();
                DistanceLabel = null;
            }

            if (DurationLabel != null) {
                DurationLabel.Dispose ();
                DurationLabel = null;
            }

            if (MarkerButton != null) {
                MarkerButton.Dispose ();
                MarkerButton = null;
            }

            if (MarkerLabel != null) {
                MarkerLabel.Dispose ();
                MarkerLabel = null;
            }

            if (PauseResumeButton != null) {
                PauseResumeButton.Dispose ();
                PauseResumeButton = null;
            }
        }
    }
}