using SoupKit.UI;
using System;
using UIKit;

namespace SoupChef
{
    public partial class SoupMenuItemDetailCell : UITableViewCell
    {
        public const string CellIdentifier = "SoupMenuItemDetailCell";

        public MenuItemView DetailView => detailView;

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public SoupMenuItemDetailCell(IntPtr handle) : base(handle) { }
        #endregion
    }
}