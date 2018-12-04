// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Reachability
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView internetConnectionImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField internetConnectionStatusField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView remoteHostImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel remoteHostLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField remoteHostStatusField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel summaryLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (internetConnectionImageView != null) {
                internetConnectionImageView.Dispose ();
                internetConnectionImageView = null;
            }

            if (internetConnectionStatusField != null) {
                internetConnectionStatusField.Dispose ();
                internetConnectionStatusField = null;
            }

            if (remoteHostImageView != null) {
                remoteHostImageView.Dispose ();
                remoteHostImageView = null;
            }

            if (remoteHostLabel != null) {
                remoteHostLabel.Dispose ();
                remoteHostLabel = null;
            }

            if (remoteHostStatusField != null) {
                remoteHostStatusField.Dispose ();
                remoteHostStatusField = null;
            }

            if (summaryLabel != null) {
                summaryLabel.Dispose ();
                summaryLabel = null;
            }
        }
    }
}