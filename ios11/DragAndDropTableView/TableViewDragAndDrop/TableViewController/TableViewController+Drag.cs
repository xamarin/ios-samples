using System;
using Foundation;
using UIKit;

namespace TableViewDragAndDrop {
	/// <summary>
	/// Implements the delegate methods for providing data for a drag interaction
	/// </summary>
	public partial class TableViewController : IUITableViewDragDelegate {
		/// <summary>
		/// Required for drag operations from a table
		/// </summary>
		public UIDragItem [] GetItemsForBeginningDragSession (UITableView tableView, IUIDragSession session, NSIndexPath indexPath)
		{
			return model.DragItems (indexPath);
		}
	}
}
