using Foundation;
using System;
using UIKit;

namespace SoupChefIntentsUI
{
    public partial class InvoiceView : UIView
    {
        public UIImageView ImageView => imageView;
        public UILabel ItemNameLabel => itemNameLabel;
        public UILabel OptionsLabel => optionsLabel;
        public UILabel TotalPriceLabel => totalPriceLabel;
        public UILabel UnitPriceLabel => unitPriceLabel;

        public InvoiceView (IntPtr handle) : base (handle)
        {
        }
    }
}