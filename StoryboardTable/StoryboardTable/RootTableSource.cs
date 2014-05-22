using System;
using MonoTouch.UIKit;


namespace StoryboardTable
{
	public class RootTableSource : UITableViewSource {

		// there is NO database or storage of Tasks in this example, just an in-memory List<>
		Chore[] tableItems;
		string cellIdentifier = "taskcell"; // set in the Storyboard

		public RootTableSource (Chore[] items)
		{
			tableItems = items; 
		}
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Length;
		}
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			// in a Storyboard, Dequeue will ALWAYS return a cell, 
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// now set the properties as normal
			cell.TextLabel.Text = tableItems[indexPath.Row].Name;
			if (tableItems[indexPath.Row].Done) 
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			else
				cell.Accessory = UITableViewCellAccessory.None;
			return cell;
		}
		public Chore GetItem(int id) {
			return tableItems[id];
		}
	}
}

