using Foundation;
using SoupKit.Data;
using System;
using UIKit;
using SoupKit.Support;
using System.Linq;
using SoupKit.UI;

namespace SoupChef
{
    public partial class SoupMenuViewController : UITableViewController
    {

        public MenuItem[] MenuItems { get; set; } = new SoupMenuManager().AvailableRegularItems;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UserActivity = NSUserActivityHelper.ViewMenuActivity;
        }

        public override void UpdateUserActivityState(NSUserActivity activity)
        {
            base.UpdateUserActivityState(activity);

            var keys = new NSString[] {
                (NSString)(NSUserActivityHelper.ActivityKeys.MenuItems),
                (NSString)(NSUserActivityHelper.ActivityKeys.SegueId)
            };

            // Creates an NSArray<NSString> of MenuItem.ItemNameKey values
            var menuItemNames = NSArray.FromNSObjects(
                MenuItems.Select<MenuItem, NSString>(
                    (menuItem) => (NSString)(menuItem.ItemNameKey)
                ).ToArray<NSString>()
            );

            var objects = new NSObject[] { menuItemNames, (NSString)"Soup Menu" };

            NSDictionary userInfo = NSDictionary.FromObjectsAndKeys(objects, keys);

            activity.AddUserInfoEntries(userInfo);

        }

        #region Navigation
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "Show New Order Detail Segue")
            {
                var destination = segue.DestinationViewController as OrderDetailViewController;
                if (destination is null) { return; }

                var indexPath = TableView.IndexPathForSelectedRow;
                if (indexPath is null) { return; }

                var orderType = new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.New);
                var newOrder = new Order(0, MenuItems[indexPath.Row], new NSMutableSet<MenuItemOption>());

                destination.Configure(orderType, newOrder, null);
            }
        }
        #endregion

        #region UITableViewDataSource
        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return MenuItems.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(SoupMenuItemDetailCell.CellIdentifier, indexPath) as SoupMenuItemDetailCell;
            if (cell is null)
            {
                Console.WriteLine("Failed to downcase UITableViewCell as SoupMenuItemDetailCell. Check Main.storyboard.");
                return new UITableViewCell();
            }

            var menuItem = MenuItems[indexPath.Row];
            cell.DetailView.ImageView.Image = UIImage.FromBundle(menuItem.IconImageName);
            cell.DetailView.TitleLabel.Text = menuItem.LocalizedString;
            return cell;

        }
        #endregion

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public SoupMenuViewController(IntPtr handle) : base(handle) { }
        #endregion
    }
}