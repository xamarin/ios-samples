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

namespace WatchBackgroundRefresh.WatchBackgroundRefreshWatchAppExtension
{
    [Register ("InterfaceController")]
    partial class InterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel timeDisplayLabel { get; set; }

        [Action ("ScheduleRefreshButtonTapped")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ScheduleRefreshButtonTapped ();

        void ReleaseDesignerOutlets ()
        {
            if (timeDisplayLabel != null) {
                timeDisplayLabel.Dispose ();
                timeDisplayLabel = null;
            }
        }
    }
}