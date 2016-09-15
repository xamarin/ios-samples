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

namespace Watchkit2Extension
{
    [Register ("CrownDetailController")]
    partial class CrownDetailController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfacePicker pickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel stateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel velocityLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (pickerView != null) {
                pickerView.Dispose ();
                pickerView = null;
            }

            if (stateLabel != null) {
                stateLabel.Dispose ();
                stateLabel = null;
            }

            if (velocityLabel != null) {
                velocityLabel.Dispose ();
                velocityLabel = null;
            }
        }
    }
}