// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SimpleWatchConnectivity.WatchAppExtension
{
    [Register ("FileTransferRowController")]
    partial class FileTransferRowController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel ProgressLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProgressLabel != null) {
                ProgressLabel.Dispose ();
                ProgressLabel = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}