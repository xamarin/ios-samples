using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;

namespace BasicTable {
	public class TableSource : UITableViewSource {
		List<TableItem> tableItems;
		 NSString cellIdentifier = new NSString("TableCell");
	
		public TableSource (List<TableItem> items)
		{
			tableItems = items;
		}
	
		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Count;
		}
		
		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView("Row Selected"
				, tableItems[indexPath.Row].Heading, null, "OK", null).Show();
			tableView.DeselectRow (indexPath, true);
		}
		
		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			CustomVegeCell cell = tableView.DequeueReusableCell (cellIdentifier) as CustomVegeCell;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new CustomVegeCell (cellIdentifier);
			}

			cell.UpdateCell (tableItems[indexPath.Row].Heading
							, tableItems[indexPath.Row].SubHeading
							, UIImage.FromFile ("Images/" +tableItems[indexPath.Row].ImageName) );
			
			return cell;
		}
	}
}