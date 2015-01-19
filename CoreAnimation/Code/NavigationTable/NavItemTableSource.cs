using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Reflection;

namespace CoreAnimationExample
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class NavItemTableSource : UITableViewSource
	{
		static readonly NSString cellIdentifier = (NSString)"NavTableCellView";

		public event EventHandler<RowClickedEventArgs> RowClicked;

		List<NavItemGroup> navItems;

		public NavItemTableSource (List<NavItemGroup> items)
		{
			navItems = items;
		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override nint NumberOfSections (UITableView tableView)
		{
			return navItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return  navItems [(int)section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return navItems [(int)section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return navItems [(int)section].Footer;
		}

		/// <summary>
		/// Called by the TableView to actually build each cell.
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// declare vars
			NavItem navItem = navItems [indexPath.Section].Items [indexPath.Row];

			var cell = tableView.DequeueReusableCell (cellIdentifier);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
				cell.Tag = Environment.TickCount;
			}

			// set the cell properties
			cell.TextLabel.Text = navItems [indexPath.Section].Items [indexPath.Row].Name;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			// return the cell
			return cell;
		}

		/// <summary>
		/// Is called when a row is selected
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// get a reference to the nav item
			NavItem navItem = navItems [indexPath.Section].Items [indexPath.Row];

			if (RowClicked != null)
				RowClicked (this, new RowClickedEventArgs (navItem));
		}
	}
}

