using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.IO;
using System.Linq;

namespace BasicTable {
	public class TableSource : UITableViewSource {
		protected string cellIdentifier = "TableCell";
		
		Dictionary<string, List<TableItem>> indexedTableItems;
		string[] keys;
		HomeScreen owner;

		public TableSource (List<TableItem> items, HomeScreen owner)
		{
			this.owner = owner;

			indexedTableItems = new Dictionary<string, List<TableItem>>();
			foreach (var t in items) {
				if (indexedTableItems.ContainsKey (t.SubHeading)) {
					indexedTableItems[t.SubHeading].Add(t);
				} else {
					indexedTableItems.Add (t.SubHeading, new List<TableItem>() {t});
				}
			}
			keys = indexedTableItems.Keys.ToArray ();
		}
		
		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override nint NumberOfSections (UITableView tableView)
		{
			return keys.Length;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return indexedTableItems[keys[section]].Count;
		}
		
		/// <summary>
		/// Sections the index titles.
		/// </summary>
//		public override string[] SectionIndexTitles (UITableView tableView)
//		{
//			return indexedTableItems.Keys.ToArray ();
//		}
		
		/// <summary>
		/// The string to show in the section header
		/// </summary>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return keys[section];
		}
		
		/// <summary>
		/// The string to show in the section footer
		/// </summary>
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return indexedTableItems[keys[section]].Count + " items";
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			UIAlertController okAlertController = UIAlertController.Create ("Row Selected", indexedTableItems[keys[indexPath.Section]][indexPath.Row].Heading, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			owner.PresentViewController (okAlertController, true, null);

			tableView.DeselectRow (indexPath, true);
		}
			
		
		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			//---- declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			TableItem item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
			
			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ cell = new UITableViewCell (item.CellStyle, cellIdentifier); }
			
			//---- set the item text
			cell.TextLabel.Text = item.Heading;
			
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