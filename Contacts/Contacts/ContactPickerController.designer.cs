// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Contacts
{
    [Register ("ContactPickerController")]
    partial class ContactPickerController
    {
        [Outlet]
        UIKit.UILabel descriptionLabel { get; set; }


        [Outlet]
        UIKit.UILabel headerLabel { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Action ("ShowPicker:")]
        partial void ShowPicker (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (descriptionLabel != null) {
                descriptionLabel.Dispose ();
                descriptionLabel = null;
            }

            if (headerLabel != null) {
                headerLabel.Dispose ();
                headerLabel = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }
        }
    }
}