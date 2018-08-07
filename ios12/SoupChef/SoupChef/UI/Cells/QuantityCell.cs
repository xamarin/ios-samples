using System;
using UIKit;

namespace SoupChef
{
    public partial class QuantityCell : UITableViewCell
    {
        public const string CellIdentifier = "Quantity Cell";

        // Used by OrderDetailViewController
        public UILabel GetQuantityLabel()
        {
            return this.QuantityLabel;
        }

        // Used by OrderDetailViewController
        public UIStepper GetStepper()
        {
            return this.Stepper;
        }

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public QuantityCell(IntPtr handle) : base(handle) { }
        #endregion
    }
}