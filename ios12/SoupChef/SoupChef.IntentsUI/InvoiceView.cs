using Foundation;
using System;
using UIKit;

namespace SoupChef
{
    public partial class InvoiceView : UIView
    {
        public InvoiceView (IntPtr handle) : base (handle) { }

        [Outlet]
        internal UIKit.UIImageView ImageView { get; set; }

        [Outlet]
        internal UIKit.UILabel ItemNameLabel { get; set; }

        [Outlet]
        internal UIKit.UILabel OptionsLabel { get; set; }

        [Outlet]
        internal UIKit.UILabel TotalPriceLabel { get; set; }

        [Outlet]
        internal UIKit.UILabel UnitPriceLabel { get; set; }
    }
}