// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace RedGreenNotificationsContentExtension
{
    [Register ("NotificationViewController")]
    partial class NotificationViewController
    {
        [Outlet]
        UIKit.UILabel label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView Xamagon { get; set; }

        [Action ("HandleDismissNotificationButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleDismissNotificationButtonTap (UIKit.UIButton sender);

        [Action ("HandleLaunchAppButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleLaunchAppButtonTap (UIKit.UIButton sender);

        [Action ("HandleRemoveNotificationButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleRemoveNotificationButtonTap (UIKit.UIButton sender);

        [Action ("HandleSliderValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleSliderValueChanged (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (Xamagon != null) {
                Xamagon.Dispose ();
                Xamagon = null;
            }
        }
    }
}