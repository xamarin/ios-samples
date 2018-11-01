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

namespace SimpleWatchConnectivity
{
    [Register ("FileTransfersViewController")]
    partial class FileTransfersViewController
    {
        [Action ("Dismiss:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Dismiss (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}