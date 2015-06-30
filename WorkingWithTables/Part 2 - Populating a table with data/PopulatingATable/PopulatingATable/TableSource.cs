using System;
using UIKit;
using Foundation;

namespace PopulatingATable
{
	public class TableSource : UITableViewSource
	{
		string[] TableItems;
		const string CellIdentifier = "TableCell";

		
		public TableSource (string[] items)
		{
			TableItems = items;
		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView("Row Selected", tableItems[indexPath.Row], null, "OK", null).Show();
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}

//		public string GetTableItem(NSIndexPath indexPath)
//		{
//			return TableItems[indexPath.Row];
//		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Length;
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			string item = TableItems[indexPath.Row];

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier); }

			cell.TextLabel.Text = item;
			//cell.SeparatorInset = new UIEdgeInsets (50, 50, 50, 50);

			return cell;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return " ";
		}
	}
}

