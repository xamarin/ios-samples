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
    [Register ("InterfaceController")]
    partial class InterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceButton HelloButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel Subheading { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceImage WelcomeImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel WelcomeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (HelloButton != null) {
                HelloButton.Dispose ();
                HelloButton = null;
            }

            if (Subheading != null) {
                Subheading.Dispose ();
                Subheading = null;
            }

            if (WelcomeImage != null) {
                WelcomeImage.Dispose ();
                WelcomeImage = null;
            }

            if (WelcomeLabel != null) {
                WelcomeLabel.Dispose ();
                WelcomeLabel = null;
            }
        }
    }
}