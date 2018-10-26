/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This class displays a list of previously placed orders.
*/

using Foundation;
using System;
using UIKit;
//using SoupKit.Data;
using System.Linq;
//using SoupKit.UI;

namespace SoupChef
{
    public partial class OrderHistoryTableViewController : UITableViewController
    {
        public static class SegueIdentifiers
        {
            public const string OrderDetails = "Order Details";
            public const string SoupMenu = "Soup Menu";
            public const string ConfigureMenu = "Configure Menu";
        }

        //SoupMenuManager SoupMenuManager = new SoupMenuManager();
        //SoupOrderDataManager SoupOrderDataManager = new SoupOrderDataManager();
        //VoiceShortcutDataManager VoiceShortcutManager = new VoiceShortcutDataManager();
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


        #region View Controller Life Cycle
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var weakThis = new WeakReference<OrderHistoryTableViewController>(this);
            //NotificationToken = NSNotificationCenter.DefaultCenter.AddObserver(
            //    NotificationKeys.DataChanged,
            //    SoupOrderDataManager,
            //    NSOperationQueue.MainQueue,
            //    (notification) =>
            //    {
            //        if (weakThis.TryGetTarget(out var orderHistoryViewController))
            //        {
            //            orderHistoryViewController.TableView.ReloadData();
            //        }
            //    }
            //);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!(NavigationController is null))
            {
                NavigationController.ToolbarHidden = true;
            }
        }
        #endregion

        #region Target Action
        [Action("unwindToOrderHistoryWithSegue:")]
        public void UnwindToOrderHistory(UIStoryboardSegue segue) { }

        [Action("placeNewOrderWithSegue:")]
        public void PlaceNewOrder(UIStoryboardSegue segue)
        {
            var source = segue.SourceViewController as OrderDetailViewController;
            if (!(source is null))
            {
                //SoupOrderDataManager.PlaceOrder(source.Order);
            }
        }
        #endregion

        #region Navigation
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            if (segue.Identifier == SegueIdentifiers.OrderDetails)
            {
                NSIndexPath selectedIndexPath = TableView.IndexPathsForSelectedRows?.FirstOrDefault();
                if (selectedIndexPath is null) { return; }

                var destination = segue.DestinationViewController as OrderDetailViewController;
                if (destination is null) { return; }

                //destination.Configure(
                //    new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.Historical),
                //    SoupOrderDataManager.OrderHistory[(nuint)selectedIndexPath.Row],
                //    VoiceShortcutManager
                //);
            }
            else if (segue.Identifier == SegueIdentifiers.ConfigureMenu)
            {
                var navCon = segue.DestinationViewController as UINavigationController;
                var configureMenuTableViewController = navCon?.ViewControllers?.FirstOrDefault() as ConfigureMenuTableViewController;
                if (configureMenuTableViewController is null) { return; }

                //configureMenuTableViewController.SoupMenuManager = SoupMenuManager;
                //configureMenuTableViewController.SoupOrderDataManager = SoupOrderDataManager;
            }
        }
        #endregion

        #region UITableViewDataSource
        //public override nint RowsInSection(UITableView tableView, nint section)
        //{
        //    return (nint)SoupOrderDataManager.OrderHistory.Count;
        //}

        //public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        //{
        //    var cell = TableView.DequeueReusableCell(SoupOrderDetailCell.CellIdentifier, indexPath) as SoupOrderDetailCell;
        //    if (cell is null ) 
        //    {
        //        return new UITableViewCell();
        //    }
        //    Order order = SoupOrderDataManager.OrderHistory[(nuint)indexPath.Row];
        //    cell.DetailView.ImageView.Image = UIImage.FromBundle(order.MenuItem.IconImageName);
        //    cell.DetailView.TitleLabel.Text = $"{order.Quantity} {order.MenuItem.LocalizedString}";
        //    cell.DetailView.SubTitleLabel.Text = DateFormatter.Value.StringFor(order.Date);
        //    return cell;
        //}
        #endregion

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public OrderHistoryTableViewController(IntPtr handle) : base(handle) { }
        #endregion
    }
}