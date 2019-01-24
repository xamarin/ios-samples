// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Calendars
{
    [Register ("MainViewController")]
    partial class MainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell calendarsCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell createReminderCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell modifyEventCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell newEventCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell remindersCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell saveEventsCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (calendarsCell != null) {
                calendarsCell.Dispose ();
                calendarsCell = null;
            }

            if (createReminderCell != null) {
                createReminderCell.Dispose ();
                createReminderCell = null;
            }

            if (modifyEventCell != null) {
                modifyEventCell.Dispose ();
                modifyEventCell = null;
            }

            if (newEventCell != null) {
                newEventCell.Dispose ();
                newEventCell = null;
            }

            if (remindersCell != null) {
                remindersCell.Dispose ();
                remindersCell = null;
            }

            if (saveEventsCell != null) {
                saveEventsCell.Dispose ();
                saveEventsCell = null;
            }
        }
    }
}