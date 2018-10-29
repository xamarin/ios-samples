// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SoupChef
{
    [Register ("OrderDetailViewController")]
    partial class OrderDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView HeaderImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel HeaderLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TableFooterView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TableViewHeader { get; set; }

        [Action ("PlaceOrder:")]
        partial void PlaceOrder (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (HeaderImageView != null) {
                HeaderImageView.Dispose ();
                HeaderImageView = null;
            }

            if (HeaderLabel != null) {
                HeaderLabel.Dispose ();
                HeaderLabel = null;
            }

            if (TableFooterView != null) {
                TableFooterView.Dispose ();
                TableFooterView = null;
            }

            if (TableViewHeader != null) {
                TableViewHeader.Dispose ();
                TableViewHeader = null;
            }
        }
    }
}