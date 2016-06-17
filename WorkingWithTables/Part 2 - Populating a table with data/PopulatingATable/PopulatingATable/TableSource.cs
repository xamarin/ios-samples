using System;

using UIKit;
using Foundation;

namespace PopulatingATable
{
	public class RowArgs : EventArgs
	{
		public string Content { get; set; }
	}

	public class TableSource : UITableViewSource
	{
		const string CellIdentifier = "TableCell";

		public event EventHandler<RowArgs> Selected;
		readonly string[] tableItems;

		public TableSource (string[] items)
		{
			tableItems = items;
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
			var handler = Selected;
			if (handler != null)
				handler (this, new RowArgs { Content = tableItems [indexPath.Row] });

			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);

			// if there are no cells to reuse, create a new one
			cell = cell ?? new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);

			cell.TextLabel.Text = tableItems[indexPath.Row];

			return cell;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return " ";
		}
	}
}