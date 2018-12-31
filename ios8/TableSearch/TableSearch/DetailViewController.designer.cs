// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TableSearch
{
    [Register ("DetailViewController")]
    partial class DetailViewController
    {
        [Outlet]
        UIKit.UILabel priceLabel { get; set; }


        [Outlet]
        UIKit.UILabel yearsLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (priceLabel != null) {
                priceLabel.Dispose ();
                priceLabel = null;
            }

            if (yearsLabel != null) {
                yearsLabel.Dispose ();
                yearsLabel = null;
            }
        }
    }
}