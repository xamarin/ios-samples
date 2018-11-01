// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using WatchKit;

namespace SimpleWatchConnectivity.WatchAppExtension
{
    [Register ("MainInterfaceController")]
    partial class MainInterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceButton CommandButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceGroup StatusGroup { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel StatusLabel { get; set; }

        [Action ("CommandAction")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CommandAction ();

        [Action ("StatusAction")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void StatusAction ();

        void ReleaseDesignerOutlets ()
        {
            if (CommandButton != null) {
                CommandButton.Dispose ();
                CommandButton = null;
            }

            if (StatusGroup != null) {
                StatusGroup.Dispose ();
                StatusGroup = null;
            }

            if (StatusLabel != null) {
                StatusLabel.Dispose ();
                StatusLabel = null;
            }
        }
    }
}