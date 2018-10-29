using Foundation;
using System;
using UIKit;

namespace SoupChef
{
    public partial class OrderConfirmedView : UIView
    {
        public OrderConfirmedView (IntPtr handle) : base (handle) { }

        [Outlet]
        internal UIKit.UIImageView ImageView { get; set; }

        [Outlet]
        internal UIKit.UILabel ItemNameLabel { get; set; }

        [Outlet]
        internal UIKit.UILabel TimeLabel { get; set; }
    }
}