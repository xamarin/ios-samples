// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SimpleWatchConnectivity
{
    [Register ("MainViewController")]
    partial class MainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView LogView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NoteLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ReachableLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TableContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TablePlaceholderView { get; set; }

        [Action ("Clear:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Clear (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (LogView != null) {
                LogView.Dispose ();
                LogView = null;
            }

            if (NoteLabel != null) {
                NoteLabel.Dispose ();
                NoteLabel = null;
            }

            if (ReachableLabel != null) {
                ReachableLabel.Dispose ();
                ReachableLabel = null;
            }

            if (TableContainerView != null) {
                TableContainerView.Dispose ();
                TableContainerView = null;
            }

            if (TablePlaceholderView != null) {
                TablePlaceholderView.Dispose ();
                TablePlaceholderView = null;
            }
        }
    }
}