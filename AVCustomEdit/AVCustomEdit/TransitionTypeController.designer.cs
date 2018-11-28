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

namespace AVCustomEdit
{
    [Register ("TransitionTypeController")]
    partial class TransitionTypeController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell crossDissolveCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell diagonalWipeCell { get; set; }

        [Action ("transitionSelected:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void transitionSelected (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (crossDissolveCell != null) {
                crossDissolveCell.Dispose ();
                crossDissolveCell = null;
            }

            if (diagonalWipeCell != null) {
                diagonalWipeCell.Dispose ();
                diagonalWipeCell = null;
            }
        }
    }
}