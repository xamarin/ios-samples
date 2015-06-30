using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;

namespace BasicTable {
	public class TableSource : UITableViewSource {
		List<TableItem> tableItems;
		 string cellIdentifier = "TableCell";
		HomeScreen owner;
	
		public TableSource (List<TableItem> items, HomeScreen owner)
		{
			tableItems = items;
			this.owner = owner;
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
			UIAlertController okAlertController = UIAlertController.Create ("Row Selected", tableItems[indexPath.Row].Heading, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			owner.PresentViewController (okAlertController, true, null);

			tableView.DeselectRow (indexPath, true);
		}
		
		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);

			// UNCOMMENT one of these to use that style
//			var cellStyle = UITableViewCellStyle.Default;
			var cellStyle = UITableViewCellStyle.Subtitle;
//			var cellStyle = UITableViewCellStyle.Value1;
//			var cellStyle = UITableViewCellStyle.Value2;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);
			}

			cell.TextLabel.Text = tableItems[indexPath.Row].Heading;
			
			// Default style doesn't support Subtitle
			if (cellStyle == UITableViewCellStyle.Subtitle 
			   || cellStyle == UITableViewCellStyle.Value1
			   || cellStyle == UITableViewCellStyle.Value2) {
				cell.DetailTextLabel.Text = tableItems[indexPath.Row].SubHeading;
			}
			
			// Value2 style doesn't support an image
			if (cellStyle != UITableViewCellStyle.Value2)
				cell.ImageView.Image = UIImage.FromFile ("Images/" +tableItems[indexPath.Row].ImageName);
			
			return cell;
		}

		#region -= editing methods =-

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			switch (editingStyle) {
				case UITableViewCellEditingStyle.Delete:
					// remove the item from the underlying data source
					tableItems.RemoveAt(indexPath.Row);
					// delete the row from the table
					tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					break;
				
				case UITableViewCellEditingStyle.Insert:
					//---- create a new item and add it to our underlying data
					tableItems.Insert (indexPath.Row, new TableItem ("(inserted)"));
					//---- insert a new row in the table
					tableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					break;

				case UITableViewCellEditingStyle.None:
					Console.WriteLine ("CommitEditingStyle:None called");
					break;
			}
		}
		
		/// <summary>
		/// Called by the table view to determine whether or not the row is editable
		/// </summary>
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you wish to disable editing for a specific indexPath or for all rows
		}
		
		/// <summary>
		/// Called by the table view to determine whether or not the row is moveable
		/// </summary>
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		/// <summary>
		/// Custom text for delete button
		/// </summary>
		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{
			return "Trash (" + tableItems[indexPath.Row].SubHeading + ")";
		}
		
		/// <summary>
		/// Called by the table view to determine whether the editing control should be an insert
		/// or a delete.
		/// </summary>
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete;
		}
		
		/// <summary>
		/// called by the table view when a row is moved.
		/// </summary>
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			//---- get a reference to the item
			var item = tableItems[sourceIndexPath.Row];
			var deleteAt = sourceIndexPath.Row;
			var insertAt = destinationIndexPath.Row;
			
			//---- if we're inserting it before, the one to delete will have a higher index 
			if (destinationIndexPath.Row < sourceIndexPath.Row) {
				//---- add one to where we delete, because we're increasing the index by inserting
				deleteAt += 1;
			} else {
				//---- add one to where we insert, because we haven't deleted the original yet
				insertAt += 1;
			}
			
			//---- copy the item to the new location
			tableItems.Insert (insertAt, item);
			
			//---- remove from the old
			tableItems.RemoveAt (deleteAt);
		}

		#endregion
	}
}