
namespace SoupChef
{
    using Foundation;
    using SoupKit.Data;
    using System;
    using System.Collections.Generic;
    using UIKit;

    /// <summary>
    /// This view controller allows you to enable and disable menu items from the active menu.
    /// `SoupMenuViewController` will display the active menu.When a menu item is disabled, any
    /// donated actions associated with the menu item are deleted from the system.
    /// </summary>
    partial class ConfigureMenuTableViewController : UITableViewController
    {
        private SoupOrderDataManager soupOrderDataManager;

        private List<SectionModel> sectionData;

        public ConfigureMenuTableViewController(IntPtr handle) : base(handle) { }

        public SoupMenuManager SoupMenuManager { get; set; }

        public SoupOrderDataManager SoupOrderDataManager
        {
            get
            {
                return soupOrderDataManager;
            }
            set
            {
                soupOrderDataManager = value;
                SoupMenuManager.OrderManager = soupOrderDataManager;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ReloadData();
        }

        private void ReloadData()
        {
            sectionData = new List<SectionModel> 
            {
                new SectionModel(SectionType.RegularItems,
                                 "Regular Menu Items",
                                 "Uncheck a row to delete any donated shortcuts associated with the menu item.",
                                 SoupMenuManager.RegularItems),
                new SectionModel(SectionType.SpecialItems,
                                 "Daily Special Menu Items",
                                 "Check a row in this section to provide a relevant shortcut.",
                                 SoupMenuManager.DailySpecialItems),
            };

            TableView.ReloadData();
        }

        #region TableViewDataSource

        public override nint NumberOfSections(UITableView tableView)
        {
            return sectionData.Count;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return sectionData[(int)section].RowContent.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell("Basic Cell", indexPath);
            var menuItem = sectionData[indexPath.Section].RowContent[indexPath.Row];
            cell.TextLabel.Text = menuItem.ItemName;
            cell.Accessory = menuItem.IsAvailable ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

            return cell;
        }

        #endregion

        #region Table Delegate

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return sectionData[(int)section].SectionHeaderText;
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            return sectionData[(int)section].SectionFooterText;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var sectionModel = sectionData[indexPath.Section];
            var currentMenuItem = sectionModel.RowContent[indexPath.Row];

            var newMenuItem = currentMenuItem.Clone();
            newMenuItem.IsAvailable = !newMenuItem.IsAvailable;

            SoupMenuManager.ReplaceMenuItem(currentMenuItem, newMenuItem);
            ReloadData();
        }

        #endregion

        /* helpers */

        enum SectionType
        {
            RegularItems,
            SpecialItems
        };

        struct SectionModel
        {
            public SectionModel(SectionType sectionType, string sectionHeaderText, string sectionFooterText, List<MenuItem> rowContent)
            {
                SectionType = sectionType;
                SectionHeaderText = sectionHeaderText;
                SectionFooterText = sectionFooterText;
                RowContent = rowContent;
            }

            public SectionType SectionType { get; set; }
            public string SectionHeaderText { get; set; }
            public string SectionFooterText { get; set; }
            public List<MenuItem> RowContent { get; set; }
        }
    }
}