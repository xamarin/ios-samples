// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ActivityRings.ActivityRingsWatchAppExtension
{
    [Register ("InterfaceController")]
    partial class InterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel activeEnergyBurnedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceButton workoutButton { get; set; }

        [Action ("ToggleWorkout")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ToggleWorkout ();

        void ReleaseDesignerOutlets ()
        {
            if (activeEnergyBurnedLabel != null) {
                activeEnergyBurnedLabel.Dispose ();
                activeEnergyBurnedLabel = null;
            }

            if (workoutButton != null) {
                workoutButton.Dispose ();
                workoutButton = null;
            }
        }
    }
}