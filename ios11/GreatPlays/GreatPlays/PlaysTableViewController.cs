using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace GreatPlays {
	public partial class PlaysTableViewController : UITableViewController {

		public List<Play> Plays { get => PlayLibrary.Shared.Plays; }

		protected PlaysTableViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override nint NumberOfSections (UITableView tableView) => 1;
		public override nint RowsInSection (UITableView tableView, nint section) => Plays.Count;

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("PlayCellIdentifier", indexPath);
			cell.TextLabel.Text = Plays [indexPath.Row].Title;
			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var indexPath = TableView.IndexPathForSelectedRow;
			var nav = segue.DestinationViewController as UINavigationController;
			var controller = nav?.TopViewController as ActsTableViewController;
			if (segue.Identifier == "showActs" && indexPath != null && controller != null) {
				controller.Play = Plays [indexPath.Row];
				controller.NavigationItem.LeftBarButtonItem = SplitViewController?.DisplayModeButtonItem;
			}
		}
	}
}

