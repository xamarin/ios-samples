using System;
using Foundation;
using UIKit;

namespace SoupChef
{
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