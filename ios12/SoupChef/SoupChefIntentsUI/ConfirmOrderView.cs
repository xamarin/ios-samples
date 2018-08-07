using Foundation;
using System;
using UIKit;

namespace SoupChefIntentsUI
{
    public partial class ConfirmOrderView : UIView
    {
        public UIImageView ImageView => imageView;
        public UILabel ItemNameLabel => itemNameLabel;
        public UILabel TimeLabel => timeLabel;
        public UILabel TotalPriceLabel => totalPriceLabel;

        public ConfirmOrderView (IntPtr handle) : base (handle)
        {
        }
    }
}