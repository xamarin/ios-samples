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

namespace Localization.WatchAppExtension
{
    [Register ("DetailController")]
    partial class DetailController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceImage DisplayImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel DisplayText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel DisplayTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DisplayImage != null) {
                DisplayImage.Dispose ();
                DisplayImage = null;
            }

            if (DisplayText != null) {
                DisplayText.Dispose ();
                DisplayText = null;
            }

            if (DisplayTime != null) {
                DisplayTime.Dispose ();
                DisplayTime = null;
            }
        }
    }
}