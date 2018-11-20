// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace RosyWriter
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel errorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        RosyWriter.RosyWriterPreview previewView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem recordButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        RosyWriter.StatisticView statisticView { get; set; }


        [Action ("OnRecordButtonClicked:")]
        partial void OnRecordButtonClicked (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (errorLabel != null) {
                errorLabel.Dispose ();
                errorLabel = null;
            }

            if (previewView != null) {
                previewView.Dispose ();
                previewView = null;
            }

            if (recordButton != null) {
                recordButton.Dispose ();
                recordButton = null;
            }

            if (statisticView != null) {
                statisticView.Dispose ();
                statisticView = null;
            }
        }
    }
}