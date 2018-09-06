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

namespace RedGreenNotifications
{
    [Register ("ManageNotificationsViewController")]
    partial class ManageNotificationsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GreenNotificationsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch GreenNotificationsSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ManageNotificationsStatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RedNotificationsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch RedNotificationsSwitch { get; set; }

        [Action ("HandleGreenNotificationsSwitchValueChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleGreenNotificationsSwitchValueChange (UIKit.UISwitch sender);

        [Action ("HandleRedNotificationsSwitchValueChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleRedNotificationsSwitchValueChange (UIKit.UISwitch sender);

        void ReleaseDesignerOutlets ()
        {
            if (GreenNotificationsLabel != null) {
                GreenNotificationsLabel.Dispose ();
                GreenNotificationsLabel = null;
            }

            if (GreenNotificationsSwitch != null) {
                GreenNotificationsSwitch.Dispose ();
                GreenNotificationsSwitch = null;
            }

            if (ManageNotificationsStatusLabel != null) {
                ManageNotificationsStatusLabel.Dispose ();
                ManageNotificationsStatusLabel = null;
            }

            if (RedNotificationsLabel != null) {
                RedNotificationsLabel.Dispose ();
                RedNotificationsLabel = null;
            }

            if (RedNotificationsSwitch != null) {
                RedNotificationsSwitch.Dispose ();
                RedNotificationsSwitch = null;
            }
        }
    }
}