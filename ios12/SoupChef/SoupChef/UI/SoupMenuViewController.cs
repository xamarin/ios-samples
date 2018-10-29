using System;
using System.Collections.Generic;
using Foundation;
using Intents;
using SoupChef.Support;
using SoupKit.Data;
using UIKit;
using static SoupChef.OrderHistoryTableViewController;

namespace SoupChef
{
    /// <summary>
    /// This view controller displays the list of active menu items to the user.
    /// </summary>
    public partial class SoupMenuViewController : UITableViewController
    {
        private const string CellReuseIdentifier = "SoupMenuItemDetailCell";

        public SoupMenuViewController(IntPtr handle) : base(handle) { }

        public List<MenuItem> MenuItems { get; set; } = new SoupMenuManager().AvailableRegularItems;

        public override NSUserActivity UserActivity 
        { 
            get => base.UserActivity;
            set
            {
                base.UserActivity = value;
                if (base.UserActivity?.ActivityType == "OrderSoupIntent")
                {
                    PerformSegue(SegueIdentifiers.NewOrder, base.UserActivity);
                }
            }
        }

        #region Navigation
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == SegueIdentifiers.NewOrder)
            {
                if (segue.DestinationViewController is OrderDetailViewController destination)
                {
                    Order order = null;

                    if (sender is UITableViewCell && TableView.IndexPathForSelectedRow != null)
                    {
                        order = new Order(new NSDate(), new NSUuid(), 1, MenuItems[TableView.IndexPathForSelectedRow.Row], new List<MenuItemOption>());
                    } 
                    else if (sender is NSUserActivity activity &&
                             activity.GetInteraction()?.Intent is OrderSoupIntent orderIntent)
                    {
                        order = Order.FromOrderSoupIntent(orderIntent);
                    }

                    if (order != null)
                    {
                        // Pass the represented menu item to OrderDetailTableConfiguration.
                        var orderType = new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.New);
                        destination.Configure(orderType, order);
                    }
                }
            }
        }

        #endregion

        #region UITableViewDataSource

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return MenuItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(SoupMenuItemDetailCell.CellIdentifier, indexPath) as SoupMenuItemDetailCell;
            var menuItem = MenuItems[indexPath.Row];

            cell.ImageView.Image = UIImage.FromBundle(menuItem.IconImageName);
            cell.ImageView.ApplyRoundedCorners();
            cell.TextLabel.Text = MenuItems[indexPath.Row].ItemName;
            cell.TextLabel.Lines = 0;

            return cell;
        }

        #endregion
    }
}