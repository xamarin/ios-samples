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
    [Register ("UserInfoTransferRowController")]
    partial class UserInfoTransferRowController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel DeleteLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DeleteLabel != null) {
                DeleteLabel.Dispose ();
                DeleteLabel = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}