using System;
using UIKit;

namespace TableViewDragAndDrop {
	/// <summary>
	/// The standard delegate methods for a table view, which are required whether or not you add drag and drop
	/// </summary>
	public partial class TableViewController : IUITableViewDataSource, IUITableViewDelegate {
		// IUITableViewDataSource
		public override nint RowsInSection (UIKit.UITableView tableView, nint section)
		{
			return model.PlaceNames.Count;
		}

		public override UIKit.UITableViewCell GetCell (UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("Cell", indexPath);
			cell.TextLabel.Text = model.PlaceNames [indexPath.Row];
			return cell;
		}

		// IUITableViewDelegate
		public override bool CanMoveRow (UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return true;
		}

		public override void MoveRow (UIKit.UITableView tableView, Foundation.NSIndexPath sourceIndexPath, Foundation.NSIndexPath destinationIndexPath)
		{
			model.MoveItem (sourceIndexPath.Row, destinationIndexPath.Row);
		}
	}
}
