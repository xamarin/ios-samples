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

namespace tvFocus
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BuyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MoreInfoButton { get; set; }

        [Action ("MoreInfoButton_PrimaryActionTriggered:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MoreInfoButton_PrimaryActionTriggered (UIKit.UIButton sender);

        [Action ("BuyButton_PrimaryActionTriggered:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BuyButton_PrimaryActionTriggered (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BuyButton != null) {
                BuyButton.Dispose ();
                BuyButton = null;
            }

            if (MoreInfoButton != null) {
                MoreInfoButton.Dispose ();
                MoreInfoButton = null;
            }
        }
    }
}