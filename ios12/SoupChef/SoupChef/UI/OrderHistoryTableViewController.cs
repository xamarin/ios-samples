/*
 * This class displays a list of previously placed orders.
 */

using Foundation;
using System;
using UIKit;
using System.Linq;
using SoupKit.Data;
using SoupKit.Support;
using SoupChef.Support;

namespace SoupChef
{
    public partial class OrderHistoryTableViewController : UITableViewController
    {
        private const string CellReuseIdentifier = "SoupOrderDetailCell";

        public static class SegueIdentifiers
        {
            public const string OrderDetails = "Order Details";
            public const string SoupMenu = "Soup Menu";
            public const string ConfigureMenu = "Configure Menu";
            public const string NewOrder = "Show New Order Detail Segue";
        }

        SoupMenuManager SoupMenuManager = new SoupMenuManager();
        SoupOrderDataManager SoupOrderDataManager = new SoupOrderDataManager();
        NSObject NotificationToken;

        Lazy<NSDateFormatter> DateFormatter => new Lazy<NSDateFormatter>(() =>
        {
            var formatter = new NSDateFormatter()
            {
                DateStyle = NSDateFormatterStyle.Long,
                TimeStyle = NSDateFormatterStyle.Long
            };
            return formatter;
        });

        public OrderHistoryTableViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var weakThis = new WeakReference<OrderHistoryTableViewController>(this);
            NotificationToken = NSNotificationCenter.DefaultCenter.AddObserver(
                NotificationKeys.DataChanged,
                SoupOrderDataManager as NSObject,
                NSOperationQueue.MainQueue,
                (notification) =>
                {
                    if (weakThis.TryGetTarget(out var orderHistoryViewController))
                    {
                        orderHistoryViewController.TableView.ReloadData();
                    }
                }
            );
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
                SoupOrderDataManager.PlaceOrder(source.Order);
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
                var orderID = activity.UserInfo?[NSUserActivityHelper.ActivityKeys.OrderId] as NSUuid;
                if(orderID != null)
                {
                    // An order was completed outside of the app and then continued as a user activity in the app
                    order = SoupOrderDataManager.Order(orderID);
                } 
                else if (sender is UITableViewCell && TableView.IndexPathsForSelectedRows != null)
                {
                    var selectedIndexPath = TableView.IndexPathsForSelectedRows.FirstOrDefault();

                    // An order was completed inside the app
                    order = SoupOrderDataManager.OrderHistory[selectedIndexPath.Row];
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
                    configureMenuTableViewController.SoupMenuManager = SoupMenuManager;
                    configureMenuTableViewController.SoupOrderDataManager = SoupOrderDataManager;
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

        /// Ensures this view controller is visible by popping pushed order history, and dismissing anything presented modally before starting segue.
        private void DriveContinueActivitySegue(string segueId, NSObject sender)
        {
            Action encapsulatedSegue = () => PerformSegue(segueId, sender);

            NavigationController?.PopToRootViewController(false);
            if (PresentedViewController != null)
            {
                DismissViewController(false, () => encapsulatedSegue());
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
            return SoupOrderDataManager.OrderHistory.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(CellReuseIdentifier, indexPath) as SoupOrderDetailCell;
            Order order = SoupOrderDataManager.OrderHistory[indexPath.Row];

            cell.ImageView.Image = UIImage.FromBundle(order.MenuItem.IconImageName);
            cell.ImageView.ApplyRoundedCorners();

            cell.TextLabel.Text = $"{order.Quantity} {order.MenuItem.ItemName}";
            cell.DetailTextLabel.Text = DateFormatter.Value.StringFor(order.Date);
            return cell;
        }

        #endregion
    }
}