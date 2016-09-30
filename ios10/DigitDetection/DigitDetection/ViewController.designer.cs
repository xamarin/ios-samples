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

namespace DigitDetection
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel accuracyLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        DigitDetection.DrawView DigitView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel predictionLabel { get; set; }

        [Action ("tappedClear:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void tappedClear (UIKit.UIButton sender);

        [Action ("tappedDeepButton:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void tappedDeepButton (UIKit.UIButton sender);

        [Action ("tappedDetectDigit:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void tappedDetectDigit (UIKit.UIButton sender);

        [Action ("tappedTestSet:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void tappedTestSet (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (accuracyLabel != null) {
                accuracyLabel.Dispose ();
                accuracyLabel = null;
            }

            if (DigitView != null) {
                DigitView.Dispose ();
                DigitView = null;
            }

            if (predictionLabel != null) {
                predictionLabel.Dispose ();
                predictionLabel = null;
            }
        }
    }
}