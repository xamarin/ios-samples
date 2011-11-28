using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.IO;

namespace Example_TableAndCellStyles.Code
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class TableSource : UITableViewSource
	{
		// declare vars
		protected List<TableItemGroup> tableItems;
		protected string cellIdentifier = "TableCell";

		protected TableSource() {}
		
		public TableSource (List<TableItemGroup> items)
		{
			tableItems = items;
		}
		
			#region -= data binding/display methods =-
		
		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems[section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return tableItems[section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, int section)
		{
			return tableItems[section].Footer;
		}
		
		#endregion
				
			#region -= user interaction methods =-
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView("Row Selected"
				, tableItems[indexPath.Section].Items[indexPath.Row].Heading, null, "OK", null).Show();
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

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			// declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			TableItem item = tableItems[indexPath.Section].Items[indexPath.Row];
			
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (item.CellStyle, cellIdentifier);
			
			// set the item text
			cell.TextLabel.Text = tableItems[indexPath.Section].Items[indexPath.Row].Heading;
			
			// if it's a cell style that supports a subheading, set it
			if(item.CellStyle == UITableViewCellStyle.Subtitle 
			   || item.CellStyle == UITableViewCellStyle.Value1
			   || item.CellStyle == UITableViewCellStyle.Value2)
			{ cell.DetailTextLabel.Text = item.SubHeading; }
			
			// if the item has a valid image, and it's not the contact style (doesn't support images)
			if(!string.IsNullOrEmpty(item.ImageName) && item.CellStyle != UITableViewCellStyle.Value2)
			{
				if(File.Exists(item.ImageName))
					cell.ImageView.Image = UIImage.FromBundle(item.ImageName);
			}
			
			// set the accessory
			cell.Accessory = item.CellAccessory;
			
			return cell;
		}
		
	}
}

