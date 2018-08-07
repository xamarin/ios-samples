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

namespace SoupChefIntentsUI
{
    [Register ("InvoiceView")]
    partial class InvoiceView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel itemNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel optionsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel totalPriceLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel unitPriceLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }

            if (itemNameLabel != null) {
                itemNameLabel.Dispose ();
                itemNameLabel = null;
            }

            if (optionsLabel != null) {
                optionsLabel.Dispose ();
                optionsLabel = null;
            }

            if (totalPriceLabel != null) {
                totalPriceLabel.Dispose ();
                totalPriceLabel = null;
            }

            if (unitPriceLabel != null) {
                unitPriceLabel.Dispose ();
                unitPriceLabel = null;
            }
        }
    }
}