using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;

namespace TableEditing.Code {
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class TableSource : UITableViewSource {
		//---- declare vars
		List<TableItemGroup> tableItems;
		string cellIdentifier = "TableCell";

		public TableSource (List<TableItemGroup> items)
		{
			this.tableItems = items;
		}
		
		#region -= data binding/display methods =-
		
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
	
		#region -= user interaction methods =-
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView("Row Selected"
				, tableItems[(int)indexPath.Section].Items[(int)indexPath.Row].Heading, null, "OK", null).Show();
		}
		
		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine("Row " + indexPath.Row.ToString() + " deselected");	
		}
		
		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine("Accessory for Section, " + indexPath.Section.ToString() + " and Row, " + indexPath.Row.ToString() + " tapped");
		}
			
		#endregion
		
		#region -= editing methods =-
		
		/// <summary>
		/// Called by the table view to determine whether or not the row is editable
		/// </summary>
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		/// <summary>
		/// Called by the table view to determine whether or not the row is moveable
		/// </summary>
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}
		
		/// <summary>
		/// Called by the table view to determine whether the editing control should be an insert
		/// or a delete.
		/// </summary>
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// WARNING: SPECIAL HANDLING HERE FOR THE SECOND ROW
			// ALSO MEANS SWIPE-TO-DELETE DOESN'T WORK ON THAT ROW
			if (indexPath.Section == 0 && indexPath.Row == 1)
			{
				return UITableViewCellEditingStyle.Insert;
			} else
			{
				return UITableViewCellEditingStyle.Delete;
			}
		}
		
		/// <summary>
		/// Custom text for delete button
		/// </summary>
		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{
			return "Trash";  // instead of Delete
		}

		/// <summary>
		/// Should be called CommitEditingAction or something, is called when a user initiates a specific editing event
		/// </summary>
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle)
			{
				case UITableViewCellEditingStyle.Delete:
					//---- remove the item from the underlying data source
					tableItems[(int)indexPath.Section].Items.RemoveAt ((int)indexPath.Row);
					//---- delete the row from the table
					tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					break;
				
				case UITableViewCellEditingStyle.Insert:
					//---- create a new item and add it to our underlying data
					tableItems[(int)indexPath.Section].Items.Insert ((int)indexPath.Row, new TableItem ("(inserted)"));
					//---- insert a new row in the table
					tableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					break;
				
				case UITableViewCellEditingStyle.None:
					Console.WriteLine ("CommitEditingStyle:None called");
					break;
			}
		}
		
		/// <summary>
		/// called by the table view when a row is moved.
		/// </summary>
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			//---- get a reference to the item
			var item = tableItems[(int)sourceIndexPath.Section].Items[(int)sourceIndexPath.Row];
			nint deleteAt = sourceIndexPath.Row;
			
			//---- if we're moving within the same section, and we're inserting it before
			if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row))
			{
				//---- add one to where we delete, because we're increasing the index by inserting
				deleteAt = sourceIndexPath.Row + 1;
			}
			
			//---- copy the item to the new location
			tableItems[(int)destinationIndexPath.Section].Items.Insert ((int)destinationIndexPath.Row, item);
			
			//---- remove from the old
			tableItems[(int)sourceIndexPath.Section].Items.RemoveAt ((int)deleteAt);
			
		}
		
		/// <summary>
		/// Called manually when the table goes into edit mode
		/// </summary>
		public void WillBeginTableEditing (UITableView tableView)
		{
			//---- start animations
			tableView.BeginUpdates ();
			
			//---- insert a new row in the table
			tableView.InsertRows (new NSIndexPath[] { NSIndexPath.FromRowSection (1, 1) }, UITableViewRowAnimation.Fade);
			//---- create a new item and add it to our underlying data
			tableItems[1].Items.Insert (1, new TableItem ());
			
			//---- end animations
			tableView.EndUpdates ();
		}
		
		/// <summary>
		/// Called manually when the table leaves edit mode
		/// </summary>
		public void DidFinishTableEditing (UITableView tableView)
		{
			//---- start animations
			tableView.BeginUpdates ();
			//---- remove our row from the underlying data
			tableItems[1].Items.RemoveAt (1);
			//---- remove the row from the table
			tableView.DeleteRows (new NSIndexPath[] { NSIndexPath.FromRowSection (1, 1) }, UITableViewRowAnimation.Fade);
			//---- finish animations
			tableView.EndUpdates ();
		}

		
		
		#endregion
		
		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//---- declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			TableItem item = tableItems[(int)indexPath.Section].Items[(int)indexPath.Row];
			
			//---- if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (item.CellStyle, cellIdentifier); 
			
			//---- set the item text
			cell.TextLabel.Text = tableItems[(int)indexPath.Section].Items[(int)indexPath.Row].Heading;
			
			//---- if it's a cell style that supports a subheading, set it
			if(item.CellStyle == UITableViewCellStyle.Subtitle 
			   || item.CellStyle == UITableViewCellStyle.Value1
			   || item.CellStyle == UITableViewCellStyle.Value2)
			{ cell.DetailTextLabel.Text = item.SubHeading; }
			
			//---- if the item has a valid image, and it's not the contact style (doesn't support images)
			if(!string.IsNullOrEmpty(item.ImageName) && item.CellStyle != UITableViewCellStyle.Value2)
			{
				if(File.Exists(item.ImageName))
				{ cell.ImageView.Image = UIImage.FromBundle(item.ImageName); }
			}
			
			//---- set the accessory
			cell.Accessory = item.CellAccessory;
			
			return cell;
		}	
	}
}