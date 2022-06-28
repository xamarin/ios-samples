// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace GroupedNotifications
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MessageWithFriendButton { get; set; }

        [Action ("ScheduleThreadedNotification:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ScheduleThreadedNotification (UIKit.UIButton sender);

        [Action ("ScheduleUnthreadedNotification:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ScheduleUnthreadedNotification (UIKit.UIButton sender);

        [Action ("UpdateThreadId:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UpdateThreadId (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (MessageWithFriendButton != null) {
                MessageWithFriendButton.Dispose ();
                MessageWithFriendButton = null;
            }
        }
    }
}