using Foundation;
using TableEditing.Models;
using System;
using System.Collections.Generic;
using UIKit;

namespace TableEditing
{
    public partial class MainViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource
    {
        private const string CellIdentifier = "CellIdentifier";

        private readonly List<TableItemGroup> items = new List<TableItemGroup>();

        public MainViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            FillItems();

            doneButton.Enabled = false;

            tableView.Delegate = this;
            tableView.DataSource = this;
        }

        partial void EditClicked(UIBarButtonItem sender)
        {
            tableView.SetEditing(true, true);
            editButton.Enabled = false;
            doneButton.Enabled = true;
        }

        partial void DoneClicked(UIBarButtonItem sender)
        {
            tableView.SetEditing(false, true);
            editButton.Enabled = true;
            doneButton.Enabled = false;
        }

        private void FillItems()
        {
            //---- Section 1

            var tableGroup = new TableItemGroup { Name = "Places" };
            tableGroup.Items.Add(new TableItem
            {
                ImageName = "Beach.png",
                Heading = "Fiji",
                SubHeading = "A nice beach"
            });

            tableGroup.Items.Add(new TableItem
            {
                ImageName = "Shanghai.png",
                Heading = "Beijing",
                SubHeading = "AKA Shanghai"
            });

            items.Add(tableGroup);

            //---- Section 2

            tableGroup = new TableItemGroup { Name = "Other" };
            tableGroup.Items.Add(new TableItem
            {
                ImageName = "Seeds.png",
                Heading = "Seedlings",
                SubHeading = "Tiny Plants"
            });

            tableGroup.Items.Add(new TableItem
            {
                ImageName = "Plants.png",
                Heading = "Plants",
                SubHeading = "Green plants"
            });

            items.Add(tableGroup);
        }

        #region -= data binding/display methods =-

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            return items.Count;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return items[(int)section].Items.Count;
        }

        [Export("tableView:titleForHeaderInSection:")]
        public string TitleForHeader(UITableView tableView, nint section)
        {
            return items[(int)section].Name;
        }

        #endregion

        #region -= user interaction methods =-

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var alert = UIAlertController.Create("Row Selected", items[indexPath.Section].Items[indexPath.Row].Heading, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(alert, true, null);
        }

        [Export("tableView:didDeselectRowAtIndexPath:")]
        public void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine($"Row {indexPath.Row} deselected");
        }

        [Export("tableView:accessoryButtonTappedForRowWithIndexPath:")]
        public void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine($"Accessory for Section, {indexPath.Section} and Row, {indexPath.Row} tapped");
        }

        #endregion

        #region -= editing methods =-

        /// <summary>
        /// Called by the table view to determine whether or not the row is editable
        /// </summary>
        [Export("tableView:canEditRowAtIndexPath:")]
        public bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        /// <summary>
        /// Called by the table view to determine whether or not the row is moveable
        /// </summary>
        [Export("tableView:canMoveRowAtIndexPath:")]
        public bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        /// <summary>
        /// Called by the table view to determine whether the editing control should be an insert
        /// or a delete.
        /// </summary>
        [Export("tableView:editingStyleForRowAtIndexPath:")]
        public UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var result = UITableViewCellEditingStyle.Delete;
            // WARNING: SPECIAL HANDLING HERE FOR THE SECOND ROW
            // ALSO MEANS SWIPE-TO-DELETE DOESN'T WORK ON THAT ROW
            if (indexPath.Section == 0 && indexPath.Row == 1)
            {
                return UITableViewCellEditingStyle.Insert;
            }

            return result;
        }

        /// <summary>
        /// Custom text for delete button
        /// </summary>
        [Export("tableView:titleForDeleteConfirmationButtonForRowAtIndexPath:")]
        public string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
        {
            return "Trash"; // instead of 'Delete'
        }

        /// <summary>
        /// Should be called CommitEditingAction or something, is called when a user initiates a specific editing event
        /// </summary>
        [Export("tableView:commitEditingStyle:forRowAtIndexPath:")]
        public void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    //---- remove the item from the underlying data source
                    items[indexPath.Section].Items.RemoveAt(indexPath.Row);

                    //---- delete the row from the table
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;

                case UITableViewCellEditingStyle.Insert:
                    //---- create a new item and add it to our underlying data
                    items[indexPath.Section].Items.Insert(indexPath.Row, new TableItem { Heading = "(inserted)" });

                    //---- insert a new row in the table
                    tableView.InsertRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;

                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }
        }

        /// <summary>
        /// called by the table view when a row is moved.
        /// </summary>
        [Export("tableView:moveRowAtIndexPath:toIndexPath:")]
        public void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            //---- get a reference to the item
            var item = items[sourceIndexPath.Section].Items[sourceIndexPath.Row];
            int deleteAt = sourceIndexPath.Row;

            //---- if we're moving within the same section, and we're inserting it before
            if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row))
            {
                //---- add one to where we delete, because we're increasing the index by inserting
                deleteAt = sourceIndexPath.Row + 1;
            }

            //---- copy the item to the new location
            items[destinationIndexPath.Section].Items.Insert(destinationIndexPath.Row, item);

            //---- remove from the old
            items[sourceIndexPath.Section].Items.RemoveAt(deleteAt);
        }

        /// <summary>
        /// Called when the table goes into edit mode
        /// </summary>
        [Export("tableView:willBeginEditingRowAtIndexPath:")]
        public void WillBeginEditing(UITableView tableView, NSIndexPath indexPath)
        {
            //---- start animations
            tableView.BeginUpdates();

            //---- do something if you need

            //---- end animations
            tableView.EndUpdates();
        }

        /// <summary>
        /// Called when the table leaves edit mode
        /// </summary>
        [Export("tableView:didEndEditingRowAtIndexPath:")]
        public void DidEndEditing(UITableView tableView, NSIndexPath indexPath)
        {
            //---- start animations
            tableView.BeginUpdates();

            //---- do something if you need

            //---- finish animations
            tableView.EndUpdates();
        }

        #endregion

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier);
            var item = items[indexPath.Section].Items[indexPath.Row];

            cell.TextLabel.Text = item.Heading;
            cell.ImageView.Image = !string.IsNullOrEmpty(item.ImageName) ? UIImage.FromBundle(item.ImageName) : null;

            return cell;
        }
    }
}