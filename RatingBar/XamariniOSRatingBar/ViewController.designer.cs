// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace XamariniOSRatingBar
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn4 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn5 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRating { get; set; }

        [Action ("UIButton13_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton13_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton14_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton14_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton15_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton15_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton16_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton16_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton17_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton17_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btn1 != null) {
                btn1.Dispose ();
                btn1 = null;
            }

            if (btn2 != null) {
                btn2.Dispose ();
                btn2 = null;
            }

            if (btn3 != null) {
                btn3.Dispose ();
                btn3 = null;
            }

            if (btn4 != null) {
                btn4.Dispose ();
                btn4 = null;
            }

            if (btn5 != null) {
                btn5.Dispose ();
                btn5 = null;
            }

            if (lblRating != null) {
                lblRating.Dispose ();
                lblRating = null;
            }
        }
    }
}