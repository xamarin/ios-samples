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

namespace tvText
{
    [Register ("FirstViewController")]
    partial class FirstViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LoginButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UserId { get; set; }

        [Action ("LoginButton_PrimaryActionTriggered:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LoginButton_PrimaryActionTriggered (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (LoginButton != null) {
                LoginButton.Dispose ();
                LoginButton = null;
            }

            if (Password != null) {
                Password.Dispose ();
                Password = null;
            }

            if (UserId != null) {
                UserId.Dispose ();
                UserId = null;
            }
        }
    }
}