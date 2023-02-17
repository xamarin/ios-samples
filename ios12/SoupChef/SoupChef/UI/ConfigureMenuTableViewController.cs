/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This view controller allows you to enable and disable menu items from the active menu.
 `SoupMenuViewController` will display the active menu. When a menu item is disabled, any
 donated actions associated with the menu item are deleted from the system.
*/

using Foundation;
using System;
using UIKit;
using SoupKit.Data;
using System.Linq;

namespace SoupChef {
	public partial class ConfigureMenuTableViewController : UITableViewController {
		enum SectionType {
			RegularItems,
			SpecialItems
		};

		struct SectionModel {
			public SectionType SectionType { get; set; }
			public string SectionHeaderText { get; set; }
			public string SectionFooterText { get; set; }
			public MenuItem [] RowContent { get; set; }

			public SectionModel (SectionType sectionType, string sectionHeaderText, string sectionFooterText, MenuItem [] rowContent)
			{
				SectionType = sectionType;
				SectionHeaderText = sectionHeaderText;
				SectionFooterText = sectionFooterText;
				RowContent = rowContent;
			}
		}

		public SoupMenuManager SoupMenuManager { get; set; }

		SoupOrderDataManager _soupOrderDataManager;
		public SoupOrderDataManager SoupOrderDataManager {
			get {
				return _soupOrderDataManager;
			}
			set {
				_soupOrderDataManager = value;
				SoupMenuManager.OrderManager = _soupOrderDataManager;
			}
		}

		SectionModel [] SectionData;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ReloadData ();
		}

		void ReloadData ()
		{
			MenuItem [] SortedRegularMenuItems =
				SoupMenuManager.AllRegularItems.OrderBy (
					arg => arg.LocalizedString, StringComparer.CurrentCultureIgnoreCase
				).ToArray<MenuItem> ();

			SectionData = new SectionModel [] {
				new SectionModel(
					SectionType.RegularItems,
					"Regular Menu Items",
					"Uncheck a row to delete any donated shortcuts associated with the menu item.",
					SortedRegularMenuItems
				),
				new SectionModel(
					SectionType.SpecialItems,
					"Daily Special Menu Items",
					"Check a row in this section to provide a relevant shortcut.",
					SoupMenuManager.DailySpecialItems
				),
			};
			TableView.ReloadData ();
		}

		#region table view data source
		public override nint NumberOfSections (UITableView tableView)
		{
			return SectionData.Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return SectionData [section].RowContent.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell ("Basic Cell", indexPath);
			var menuItem = SectionData [indexPath.Section].RowContent [indexPath.Row];
			cell.TextLabel.Text = menuItem.LocalizedString;
			cell.Accessory = menuItem.IsAvailable ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			return cell;
		}
		#endregion

		#region table delegate
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return SectionData [section].SectionHeaderText;
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return SectionData [section].SectionFooterText;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var sectionModel = SectionData [indexPath.Section];
			var currentMenuItem = sectionModel.RowContent [indexPath.Row];
			var newMenuItem = currentMenuItem.Clone ();
			newMenuItem.IsAvailable = !newMenuItem.IsAvailable;

			SoupMenuManager.ReplaceMenuItem (currentMenuItem, newMenuItem);
			ReloadData ();
		}
		#endregion

		#region xamarin
		public ConfigureMenuTableViewController (IntPtr handle) : base (handle) { }
		#endregion
	}
}
