
namespace SoupChef
{
    using Foundation;
    using System;
    using UIKit;

    public partial class QuantityCell : UITableViewCell
    {
        public const string CellIdentifier = "Quantity Cell";

        public QuantityCell(IntPtr handle) : base(handle) { }

        [Outlet]
        internal UILabel QuantityLabel { get; private set; }

        [Outlet]
        internal UIStepper Stepper { get; private set; }
    }
}