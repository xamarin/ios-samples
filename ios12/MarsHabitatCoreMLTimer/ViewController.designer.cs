// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MarsHabitatCoreMLTimer
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LargeTestButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MediumTestButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SmallTestButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView Spinner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StatusLabel { get; set; }

        [Action ("LargeTestButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LargeTestButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("MediumTestButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MediumTestButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SmallTestButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SmallTestButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (LargeTestButton != null) {
                LargeTestButton.Dispose ();
                LargeTestButton = null;
            }

            if (MediumTestButton != null) {
                MediumTestButton.Dispose ();
                MediumTestButton = null;
            }

            if (SmallTestButton != null) {
                SmallTestButton.Dispose ();
                SmallTestButton = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (StatusLabel != null) {
                StatusLabel.Dispose ();
                StatusLabel = null;
            }
        }
    }
}