using System;
using Foundation;
using UIKit;

namespace GreatPlays {
	public partial class ActsTableViewController : UITableViewController {

		Play play;
		public Play Play {
			get => play;
			set {
				play = value;
				NavigationItem.Title = play?.Title ?? "Acts";
			}
		}

		protected ActsTableViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override nint NumberOfSections (UITableView tableView) => 1;
		public override nint RowsInSection (UITableView tableView, nint section) => Play?.Acts.Count ?? 0;

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("ActCellIdentifier", indexPath);
			cell.TextLabel.Text = Play?.Acts [indexPath.Row].Identifier;
			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var indexPath = TableView.IndexPathForSelectedRow;
			var controller = segue.DestinationViewController as ScenesTableViewController;
			if (segue.Identifier == "showScenes" && indexPath != null && controller != null)
				controller.Act = Play?.Acts [indexPath.Row];
		}
	}
}

