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

namespace SpeedySloth
{
    [Register ("WorkoutViewController")]
    partial class WorkoutViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WorkoutSessionState { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (WorkoutSessionState != null) {
                WorkoutSessionState.Dispose ();
                WorkoutSessionState = null;
            }
        }
    }
}