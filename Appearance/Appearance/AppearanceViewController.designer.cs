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

namespace Appearance
{
    [Register ("AppearanceViewController")]
    partial class AppearanceViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonBlack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonBlue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel overriddenLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView progress1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView progress2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider slider1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider slider2 { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (buttonBlack != null) {
                buttonBlack.Dispose ();
                buttonBlack = null;
            }

            if (buttonBlue != null) {
                buttonBlue.Dispose ();
                buttonBlue = null;
            }

            if (overriddenLabel != null) {
                overriddenLabel.Dispose ();
                overriddenLabel = null;
            }

            if (progress1 != null) {
                progress1.Dispose ();
                progress1 = null;
            }

            if (progress2 != null) {
                progress2.Dispose ();
                progress2 = null;
            }

            if (slider1 != null) {
                slider1.Dispose ();
                slider1 = null;
            }

            if (slider2 != null) {
                slider2.Dispose ();
                slider2 = null;
            }
        }
    }
}