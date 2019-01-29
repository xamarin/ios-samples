using Foundation;
using PeekPopNavigation.Models;
using System;
using UIKit;

namespace PeekPopNavigation
{
    public class ColorsViewControllerBase : UITableViewController
    {
        public ColorsViewControllerBase(IntPtr handle) : base(handle) { }

        protected static ColorData ColorData { get; } = new ColorData();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NSNotificationCenter.DefaultCenter.AddObserver(ColorItem.ColorItemUpdated, OnColorItemUpdated);
            NSNotificationCenter.DefaultCenter.AddObserver(ColorItem.ColorItemDeleted, OnColorItemDeleted);
        }

        #region Table view data source

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return ColorData.Colors.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("BasicCell", indexPath);

            var colorItem = ColorData.Colors[indexPath.Row];

            cell.TextLabel.Text = colorItem.Name;
            cell.ImageView.Image = colorItem.Starred ? UIImage.FromBundle("StarFilled") : UIImage.FromBundle("StarOutline");
            cell.ImageView.TintColor = colorItem.Color;

            return cell;
        }

        #endregion

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.DestinationViewController is ColorItemViewController colorItemViewController)
            {
                var index = base.TableView.IndexPathForSelectedRow ?? base.TableView.IndexPathForCell(sender as UITableViewCell);
                // Pass over a reference to the ColorData object and the specific ColorItem being viewed.
                colorItemViewController.ColorData = ColorData;
                colorItemViewController.ColorItem = ColorData.Colors[index.Row];
            }
        }

        private void OnColorItemDeleted(NSNotification notification)
        {
            // As there are two instances of colorData between `ColorsViewControllerStoryboard` and
            // `ColorsViewControllerCode`, this method must only process notification callbacks when
            // the instances of colorData match.
            if (notification.Object is ColorData colorData)
            {
                // Grab the index of the deleted object from the userInfo dictionary
                if (notification.UserInfo != null &&
                    notification.UserInfo.TryGetValue(new NSString("index"), out NSObject @object) &&
                    @object is NSNumber number)
                {
                    var indexPath = NSIndexPath.FromRowSection(number.Int32Value, 0);
                    base.TableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
                }
            }
        }

        private void OnColorItemUpdated(NSNotification notification)
        {
            // As there are two instances of colorData between `ColorsViewControllerStoryboard` and
            // `ColorsViewControllerCode`, this method must only process notification callbacks when
            // the object for this notification exists in *this* view controller's colorData array.
            if (notification.Object is ColorItem colorItem)
            {
                var arrayIndex = ColorData.Colors.IndexOf(colorItem);
                if (arrayIndex != -1)
                {
                    var indexPath = NSIndexPath.FromRowSection(arrayIndex, 0);
                    base.TableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
                }
            }
        }
    }
}