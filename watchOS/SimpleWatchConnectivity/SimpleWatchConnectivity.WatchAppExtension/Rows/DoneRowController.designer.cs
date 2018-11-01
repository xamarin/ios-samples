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
    [Register ("DoneRowController")]
    partial class DoneRowController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel DoneLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DoneLabel != null) {
                DoneLabel.Dispose ();
                DoneLabel = null;
            }
        }
    }
}