using System;
using Foundation;
using UIKit;

namespace GreatPlays {
	public partial class ScenesTableViewController : UITableViewController {

		Act act;
		public Act Act {
			get => act;
			set {
				act = value;
				NavigationItem.Title = act?.Identifier ?? "Act";
			}
		}

		protected ScenesTableViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override nint NumberOfSections (UITableView tableView) => 1;
		public override nint RowsInSection (UITableView tableView, nint section) => Act?.Scenes.Count ?? 0;

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("SceneCellIdentifier", indexPath);
			cell.TextLabel.Text = Act.Scenes [indexPath.Row].Identifier;
			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var indexPath = TableView.IndexPathForSelectedRow;
			var controller = segue.DestinationViewController as SceneViewController;
			if (segue.Identifier == "showScene" && indexPath != null && controller != null)
				controller.Scene = act?.Scenes [indexPath.Row];
		}
	}
}

