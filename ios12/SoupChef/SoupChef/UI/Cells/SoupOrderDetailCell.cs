using System;
using UIKit;
//using SoupKit.UI;

namespace SoupChef
{
    public partial class SoupOrderDetailCell : UITableViewCell
    {
        //public const string CellIdentifier = "SoupOrderDetailCell";

        //public MenuItemView DetailView => detailView;

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public SoupOrderDetailCell(IntPtr handle) : base(handle) { }
        #endregion
    }
}