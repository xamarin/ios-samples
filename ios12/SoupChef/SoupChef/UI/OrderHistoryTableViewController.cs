
namespace SoupChef
{
    using CoreFoundation;
    using Foundation;
    using SoupChef.Support;
    using SoupChef.Data;
    using System;
    using System.Linq;
    using UIKit;

    /// <summary>
    /// This class displays a list of previously placed orders.
    /// </summary>
    public partial class OrderHistoryTableViewController : UITableViewController
    {
        private const string CellReuseIdentifier = "SoupOrderDetailCell";

        private SoupOrderDataManager soupOrderManager = new SoupOrderDataManager();

        private SoupMenuManager soupMenuManager = new SoupMenuManager();

        private NSObject notificationToken;

        public OrderHistoryTableViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            notificationToken = NSNotificationCenter.DefaultCenter.AddObserver(NotificationKeys.DataChanged,
                                                                                NSObject.FromObject(soupOrderManager),
                                                                                NSOperationQueue.MainQueue,
                                                                                (notification) => this.TableView.ReloadData());
        }

        protected override void Dispose(bool disposing)
        {
            notificationToken.Dispose();
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);

            base.Dispose(disposing);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null)
            {
                NavigationController.ToolbarHidden = true;
            }
        }

        #region Target Action

        [Action("unwindToOrderHistoryWithSegue:")]
        public void UnwindToOrderHistory(UIStoryboardSegue segue) { }

        [Action("placeNewOrderWithSegue:")]
        public void PlaceNewOrder(UIStoryboardSegue segue)
        {
            if (segue.SourceViewController is OrderDetailViewController source)
            {
                soupOrderManager.PlaceOrder(source.Order);
            }
        }
        #endregion

        #region Navigation

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == SegueIdentifiers.OrderDetails)
            {
                Order order = null;
                var activity = sender as NSUserActivity;
                var stringOrderId = activity?.UserInfo?[NSUserActivityHelper.ActivityKeys.OrderId] as NSString;
                if (stringOrderId != null && Guid.TryParse(stringOrderId, out Guid orderId))
                {
                    // An order was completed outside of the app and then continued as a user activity in the app
                    order = soupOrderManager.Order(orderId);
                }
                else if (sender is UITableViewCell && TableView.IndexPathsForSelectedRows != null)
                {
                    var selectedIndexPath = TableView.IndexPathsForSelectedRows.FirstOrDefault();

                    // An order was completed inside the app
                    order = soupOrderManager.OrderHistory[selectedIndexPath.Row];
                }

                if (segue.DestinationViewController is OrderDetailViewController destination && order != null)
                {
                    destination.Configure(new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.Historical), order);
                }
            }
            else if (segue.Identifier == SegueIdentifiers.ConfigureMenu)
            {
                if (segue.DestinationViewController is UINavigationController navController &&
                    navController.ViewControllers.FirstOrDefault() is ConfigureMenuTableViewController configureMenuTableViewController)
                {
                    configureMenuTableViewController.SoupMenuManager = soupMenuManager;
                    configureMenuTableViewController.SoupOrderDataManager = soupOrderManager;
                }
            }
            else if (segue.Identifier == SegueIdentifiers.SoupMenu)
            {
                if (segue.DestinationViewController is UINavigationController navController &&
                    navController.ViewControllers.FirstOrDefault() is SoupMenuViewController menuController)
                {

                    if (sender is NSUserActivity activity && activity.ActivityType == "OrderSoupIntent")
                    {
                        menuController.UserActivity = activity;
                    }
                    else
                    {
                        menuController.UserActivity = NSUserActivityHelper.ViewMenuActivity;
                    }
                }
            }
        }

        /// <summary>
        /// This method is called when a user activity is continued via the restoration handler
        /// in `UIApplicationDelegate application(_:continue:restorationHandler:)`
        /// </summary>
        public override void RestoreUserActivityState(NSUserActivity activity)
        {
            base.RestoreUserActivityState(activity);

            if (activity.ActivityType == NSUserActivityHelper.ViewMenuActivityType)
            {
                DriveContinueActivitySegue(SegueIdentifiers.SoupMenu, null);
            }
            else if (activity.ActivityType == NSUserActivityHelper.OrderCompleteActivityType &&
                     activity.UserInfo?[NSUserActivityHelper.ActivityKeys.OrderId] is NSUuid)
            {
                // Order complete, display the order history
                DriveContinueActivitySegue(SegueIdentifiers.OrderDetails, activity);
            }
            else if (activity.ActivityType == "OrderSoupIntent")
            {
                // Order not completed, allow order to be customized
                DriveContinueActivitySegue(SegueIdentifiers.SoupMenu, activity);
            }
        }

        /// <summary>
        /// Ensures this view controller is visible by popping pushed order history, and dismissing anything presented modally before starting segue.
        /// </summary>
        private void DriveContinueActivitySegue(string segueId, NSObject sender)
        {
            void encapsulatedSegue() => PerformSegue(segueId, sender);

            NavigationController?.PopToRootViewController(false);
            if (PresentedViewController != null)
            {
                DismissViewController(false, encapsulatedSegue);
            }
            else
            {
                encapsulatedSegue();
            }
        }

        #endregion

        #region UITableViewDataSource

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return soupOrderManager.OrderHistory.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(CellReuseIdentifier, indexPath);
            var order = soupOrderManager.OrderHistory[indexPath.Row];

            cell.ImageView.Image = UIImage.FromBundle(order.MenuItem.IconImageName);
            cell.ImageView.ApplyRoundedCorners();

            cell.TextLabel.Text = $"{order.Quantity} {order.MenuItem.ItemName}";
            cell.DetailTextLabel.Text = order.Date.ToString("r");
            return cell;
        }

        #endregion

        /* helpers */

        static class SegueIdentifiers
        {
            public const string OrderDetails = "Order Details";
            public const string SoupMenu = "Soup Menu";
            public const string ConfigureMenu = "Configure Menu";
        }
    }
}