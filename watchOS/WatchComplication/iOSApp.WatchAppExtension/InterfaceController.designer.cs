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

namespace iOSApp.WatchAppExtension
{
    [Register ("InterfaceController")]
    partial class InterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel MessageText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceButton SetMessageButton { get; set; }

        [Action ("SetMessageClicked")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SetMessageClicked ();

        [Action ("UpdateClicked")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UpdateClicked ();

        void ReleaseDesignerOutlets ()
        {
            if (MessageText != null) {
                MessageText.Dispose ();
                MessageText = null;
            }

            if (SetMessageButton != null) {
                SetMessageButton.Dispose ();
                SetMessageButton = null;
            }
        }
    }
}