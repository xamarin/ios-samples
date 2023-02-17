using System;
using System.Collections.Generic;
using UIKit;


namespace StoryboardTable {
	public class SecretTableSource : UITableViewSource {

		// there is NO database or storage of Tasks in this example, just an in-memory List<>
		SecretItem [] tableItems;
		string cellIdentifier = "taskcell"; // set in the Storyboard

		public SecretTableSource ()
		{
			tableItems = new List<SecretItem> {
				new SecretItem() {Name="Gift list", Notes="iPhone X, Apple Watch, HomePod", Done=false},
				new SecretItem() {Name="Reminders", Notes="buy flowers", Done=false}
			}.ToArray ();
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// in a Storyboard, Dequeue will ALWAYS return a cell, 
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// now set the properties as normal
			cell.TextLabel.Text = tableItems [indexPath.Row].Name;
			cell.DetailTextLabel.Text = tableItems [indexPath.Row].Notes;
			return cell;
		}
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
		}
	}
}

