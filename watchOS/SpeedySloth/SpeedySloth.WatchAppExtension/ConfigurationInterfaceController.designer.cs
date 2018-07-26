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
    [Register ("ConfigurationInterfaceController")]
    partial class ConfigurationInterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfacePicker ActivityTypePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfacePicker LocationTypePicker { get; set; }

        [Action ("ActivityTypePickerSelectedItemChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActivityTypePickerSelectedItemChanged (System.nint sender);

        [Action ("DidTapStartButton")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DidTapStartButton ();

        [Action ("LocationTypePickerSelectedItemChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LocationTypePickerSelectedItemChanged (System.nint sender);

        [Action ("SlothifyWorkouts")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SlothifyWorkouts ();

        void ReleaseDesignerOutlets ()
        {
            if (ActivityTypePicker != null) {
                ActivityTypePicker.Dispose ();
                ActivityTypePicker = null;
            }

            if (LocationTypePicker != null) {
                LocationTypePicker.Dispose ();
                LocationTypePicker = null;
            }
        }
    }
}