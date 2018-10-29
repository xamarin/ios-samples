
namespace SoupChef
{
    using Foundation;
    using Intents;
    using SoupChef.Support;
    using SoupChef.Data;
    using System;
    using System.Collections.Generic;
    using UIKit;

    /// <summary>
    /// This view controller displays the list of active menu items to the user.
    /// </summary>
    public partial class SoupMenuViewController : UITableViewController
    {
        private const string NewOrderSegueIdentifier = "Show New Order Detail Segue";
        private const string CellReuseIdentifier = "SoupMenuItemDetailCell";

        private readonly List<MenuItem> menuItems = new SoupMenuManager().AvailableRegularItems;

        public SoupMenuViewController(IntPtr handle) : base(handle) { }

        public override NSUserActivity UserActivity 
        { 
            get => base.UserActivity;
            set
            {
                base.UserActivity = value;
                if (base.UserActivity?.ActivityType == "OrderSoupIntent")
                {
                    PerformSegue(NewOrderSegueIdentifier, base.UserActivity);
                }
            }
        }

        #region Navigation

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == NewOrderSegueIdentifier)
            {
                if (segue.DestinationViewController is OrderDetailViewController destination)
                {
                    Order order = null;

                    if (sender is UITableViewCell && TableView.IndexPathForSelectedRow != null)
                    {
                        order = new Order(1, menuItems[TableView.IndexPathForSelectedRow.Row], new List<MenuItemOption>());
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
            return menuItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(CellReuseIdentifier, indexPath);
            var menuItem = menuItems[indexPath.Row];

            cell.ImageView.Image = UIImage.FromBundle(menuItem.IconImageName);
            cell.ImageView.ApplyRoundedCorners();
            cell.TextLabel.Text = menuItems[indexPath.Row].ItemName;
            cell.TextLabel.Lines = 0;

            return cell;
        }

        #endregion
    }
}