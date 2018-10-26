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

namespace SoupChef
{
    [Register ("OrderDetailViewController")]
    partial class OrderDetailViewController
    {
        //[Outlet]
        //[GeneratedCode ("iOS Designer", "1.0")]
        //SoupKit.UI.MenuItemView SoupDetailView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TableViewHeader { get; set; }

        [Action ("PlaceOrder:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PlaceOrder (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            //if (SoupDetailView != null) {
            //    SoupDetailView.Dispose ();
            //    SoupDetailView = null;
            //}

            if (TableViewHeader != null) {
                TableViewHeader.Dispose ();
                TableViewHeader = null;
            }
        }
    }
}