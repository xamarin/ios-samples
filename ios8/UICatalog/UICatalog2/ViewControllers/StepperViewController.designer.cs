// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace UICatalog
{
    [Register ("StepperViewController")]
    partial class StepperViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper customStepper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel customStepperLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper defaultStepper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel defaultStepperLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper tintedStepper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel tintedStepperLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (customStepper != null) {
                customStepper.Dispose ();
                customStepper = null;
            }

            if (customStepperLabel != null) {
                customStepperLabel.Dispose ();
                customStepperLabel = null;
            }

            if (defaultStepper != null) {
                defaultStepper.Dispose ();
                defaultStepper = null;
            }

            if (defaultStepperLabel != null) {
                defaultStepperLabel.Dispose ();
                defaultStepperLabel = null;
            }

            if (tintedStepper != null) {
                tintedStepper.Dispose ();
                tintedStepper = null;
            }

            if (tintedStepperLabel != null) {
                tintedStepperLabel.Dispose ();
                tintedStepperLabel = null;
            }
        }
    }
}