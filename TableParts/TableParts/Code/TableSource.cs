using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace Example_TableParts
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class TableSource : UITableViewSource
	{
		//---- declare vars
		protected List<TableItemGroup> tableItems;
		protected string cellIdentifier = "TableCell";

		public TableSource (List<TableItemGroup> items)
		{
			tableItems = items;
		}
		
		#region data binding/display methods
		
		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override nint NumberOfSections (UITableView tableView)
		{
			return tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems[(int)section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return tableItems[(int)section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return tableItems[(int)section].Footer;
		}
		
		#endregion	
				
		#region user interaction methods
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView ("Row Selected"
				, tableItems[(int)indexPath.Section].Items[(int)indexPath.Row], null, "OK", null).Show ();
		}
		
		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("Row " + indexPath.Row.ToString () + " deselected");	
		}
		
		#endregion	

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			Console.WriteLine ("Calling Get Cell, isEditing:" + tableView.Editing);
			
			//---- declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			
			//---- if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			
			//---- set the item text
			cell.TextLabel.Text = tableItems[(int)indexPath.Section].Items[(int)indexPath.Row];
			
			return cell;
		}
	}
}

