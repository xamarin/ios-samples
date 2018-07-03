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
    [Register ("SummaryInterfaceController")]
    partial class SummaryInterfaceController
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
        WatchKit.WKInterfaceLabel WorkoutLabel { get; set; }

        [Action ("TapDoneTapped")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TapDoneTapped ();

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

            if (WorkoutLabel != null) {
                WorkoutLabel.Dispose ();
                WorkoutLabel = null;
            }
        }
    }
}