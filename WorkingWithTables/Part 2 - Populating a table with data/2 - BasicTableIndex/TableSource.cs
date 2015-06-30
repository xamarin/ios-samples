using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Foundation;
using UIKit;

namespace BasicTable {
	public class TableSource : UITableViewSource {
		string cellIdentifier = "TableCell";

		Dictionary<string, List<string>> indexedTableItems;
		string[] keys;
		HomeScreen owner;

		public TableSource (string[] items, HomeScreen owner)
		{
			this.owner = owner;

			indexedTableItems = new Dictionary<string, List<string>>();
			foreach (var t in items) {
				if (indexedTableItems.ContainsKey (t[0].ToString ())) {
					indexedTableItems[t[0].ToString ()].Add(t);
				} else {
					indexedTableItems.Add (t[0].ToString (), new List<string>() {t});
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
		public override String[] SectionIndexTitles (UITableView tableView)
		{
			return indexedTableItems.Keys.ToArray ();
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			UIAlertController okAlertController = UIAlertController.Create ("Row Selected", indexedTableItems[keys[indexPath.Section]][indexPath.Row], UIAlertControllerStyle.Alert);
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
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			
			cell.TextLabel.Text = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
			
			return cell;
		}
	}
}