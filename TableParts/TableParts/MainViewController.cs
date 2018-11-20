using Foundation;
using TableParts.Models;
using System;
using System.Collections.Generic;
using UIKit;

namespace TableParts
{
    public partial class MainViewController : UITableViewController
    {
        private const string CellIdentifier = "TableCell";

        private readonly List<TableItemGroup> items = new List<TableItemGroup>();

        public MainViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CreateTableItems();
        }

        protected void CreateTableItems()
        {
            // Section 1
            var group = new TableItemGroup() { Name = "Section 0 Header", Footer = "Section 0 Footer" };
            group.Items.Add("Row 0");
            group.Items.Add("Row 1");
            group.Items.Add("Row 2");
            items.Add(group);

            // Section 2
            group = new TableItemGroup() { Name = "Section 1 Header", Footer = "Section 1 Footer" };
            group.Items.Add("Row 0");
            group.Items.Add("Row 1");
            group.Items.Add("Row 2");
            items.Add(group);

            // Section 3
            group = new TableItemGroup() { Name = "Section 2 Header", Footer = "Section 2 Footer" };
            group.Items.Add("Row 0");
            group.Items.Add("Row 1");
            group.Items.Add("Row 2");
            items.Add(group);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return items.Count;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return items[(int)section].Items.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return items[(int)section].Name;
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            return items[(int)section].Footer;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var alert = UIAlertController.Create("Row Selected", items[indexPath.Section].Items[indexPath.Row], UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(alert, true, null);
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine($"Row {indexPath.Row} deselected");
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine($"Calling 'GetCell', 'isEditing': {tableView.Editing}");

            var cell = tableView.DequeueReusableCell(CellIdentifier);
            cell.TextLabel.Text = items[indexPath.Section].Items[indexPath.Row];

            return cell;
        }
    }
}