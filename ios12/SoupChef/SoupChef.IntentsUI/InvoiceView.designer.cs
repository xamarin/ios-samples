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
    [Register ("InvoiceView")]
    partial class InvoiceView
    {
        void ReleaseDesignerOutlets ()
        {
            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }

            if (ItemNameLabel != null) {
                ItemNameLabel.Dispose ();
                ItemNameLabel = null;
            }

            if (OptionsLabel != null) {
                OptionsLabel.Dispose ();
                OptionsLabel = null;
            }

            if (TotalPriceLabel != null) {
                TotalPriceLabel.Dispose ();
                TotalPriceLabel = null;
            }

            if (UnitPriceLabel != null) {
                UnitPriceLabel.Dispose ();
                UnitPriceLabel = null;
            }
        }
    }
}