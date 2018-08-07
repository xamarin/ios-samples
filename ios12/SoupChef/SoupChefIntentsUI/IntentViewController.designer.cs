// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SoupChefIntentsUI
{
    [Register ("IntentViewController")]
    partial class IntentViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SoupChefIntentsUI.ConfirmOrderView confirmationView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SoupChefIntentsUI.InvoiceView invoiceView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (confirmationView != null) {
                confirmationView.Dispose ();
                confirmationView = null;
            }

            if (invoiceView != null) {
                invoiceView.Dispose ();
                invoiceView = null;
            }
        }
    }
}