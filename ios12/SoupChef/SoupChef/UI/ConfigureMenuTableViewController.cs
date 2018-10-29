/*
 * This view controller allows you to enable and disable menu items from the active menu.
 * `SoupMenuViewController` will display the active menu. When a menu item is disabled, any
 * donated actions associated with the menu item are deleted from the system.
 */

namespace SoupChef
{
    using System;
    using System.Collections.Generic;
    using Foundation;
    using SoupKit.Data;
    using UIKit;

    partial class ConfigureMenuTableViewController : UITableViewController
    {
        enum SectionType
        {
            RegularItems,
            SpecialItems
        };

        struct SectionModel
        {
            public SectionType SectionType { get; set; }
            public string SectionHeaderText { get; set; }
            public string SectionFooterText { get; set; }
            public List<MenuItem> RowContent { get; set; }

            public SectionModel(SectionType sectionType, string sectionHeaderText, string sectionFooterText, List<MenuItem> rowContent)
            {
                SectionType = sectionType;
                SectionHeaderText = sectionHeaderText;
                SectionFooterText = sectionFooterText;
                RowContent = rowContent;
            }
        }

        public ConfigureMenuTableViewController(IntPtr handle) : base(handle) { }

        public SoupMenuManager SoupMenuManager { get; set; }

        SoupOrderDataManager _soupOrderDataManager;
        public SoupOrderDataManager SoupOrderDataManager
        {
            get
            {
                return _soupOrderDataManager;
            }
            set
            {
                _soupOrderDataManager = value;
                SoupMenuManager.OrderManager = _soupOrderDataManager;
            }
        }

        private List<SectionModel> SectionData;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ReloadData();
        }

        void ReloadData()
        {
            SectionData = new List<SectionModel> 
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

        #region TableView DataSource

        public override nint NumberOfSections(UITableView tableView)
        {
            return SectionData.Count;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return SectionData[(int)section].RowContent.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell("Basic Cell", indexPath);
            var menuItem = SectionData[indexPath.Section].RowContent[indexPath.Row];
            cell.TextLabel.Text = menuItem.ItemName;
            cell.Accessory = menuItem.IsAvailable ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            return cell;
        }

        #endregion

        #region Table Delegate

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return SectionData[(int)section].SectionHeaderText;
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            return SectionData[(int)section].SectionFooterText;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var sectionModel = SectionData[indexPath.Section];
            var currentMenuItem = sectionModel.RowContent[indexPath.Row];

            var newMenuItem = currentMenuItem.Clone();
            newMenuItem.IsAvailable = !newMenuItem.IsAvailable;

            SoupMenuManager.ReplaceMenuItem(currentMenuItem, newMenuItem);
            ReloadData();
        }

        #endregion
    }
}