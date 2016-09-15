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
    [Register ("GestureDetailController")]
    partial class GestureDetailController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel longPressLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel panLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceGroup swipeGroup { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel swipeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceGroup tapGroup { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel tapLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (longPressLabel != null) {
                longPressLabel.Dispose ();
                longPressLabel = null;
            }

            if (panLabel != null) {
                panLabel.Dispose ();
                panLabel = null;
            }

            if (swipeGroup != null) {
                swipeGroup.Dispose ();
                swipeGroup = null;
            }

            if (swipeLabel != null) {
                swipeLabel.Dispose ();
                swipeLabel = null;
            }

            if (tapGroup != null) {
                tapGroup.Dispose ();
                tapGroup = null;
            }

            if (tapLabel != null) {
                tapLabel.Dispose ();
                tapLabel = null;
            }
        }
    }
}