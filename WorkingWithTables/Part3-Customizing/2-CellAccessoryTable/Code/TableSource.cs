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
		/// Called when the DetailDisclosureButton is touched.
		/// Does nothing if DetailDisclosureButton isn't in the cell
		/// </summary>
		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			UIAlertController okAlertController = UIAlertController.Create ("DetailDisclosureButton Touched", tableItems[indexPath.Row].Heading, UIAlertControllerStyle.Alert);
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
			var cellStyle = UITableViewCellStyle.Default;
//			var cellStyle = UITableViewCellStyle.Subtitle;
//			var cellStyle = UITableViewCellStyle.Value1;
//			var cellStyle = UITableViewCellStyle.Value2;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);
			}


			
			// UNCOMMENT one of these to see that accessory
//			cell.Accessory = UITableViewCellAccessory.Checkmark;
//			cell.Accessory = UITableViewCellAccessory.DetailButton;
//			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//			cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;  // implement AccessoryButtonTapped
			cell.Accessory = UITableViewCellAccessory.None; // to clear the accessory
			
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
	}
}