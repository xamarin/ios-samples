using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

using static UIKit.UITableViewCellAccessory;

namespace AVCamBarcode
{
	interface ItemSelectionViewControllerDelegate
	{
		void ItemSelectionViewController (ItemSelectionViewController itemSelectionViewController, List<string> selectedItems);
	}

	public class ItemSelectionViewController : UITableViewController
	{
		ItemSelectionViewControllerDelegate SelectionDelegate;

		string identifier = string.Empty;
		readonly List<string> allItems = new List<string> ();
		readonly List<string> selectedItems = new List<string> ();
		bool allowsMultipleSelection;

		public ItemSelectionViewController ()
		{
		}

		[Action ("done")]
		void Done ()
		{
			// Notify the delegate that selecting items is finished.
			SelectionDelegate?.ItemSelectionViewController (this, selectedItems);

			// Dismiss the view controller.
			DismissViewController (true, null);
		}

		#region UITableViewDataSource

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = allItems [indexPath.Row];

			var cell = tableView.DequeueReusableCell ("Item", indexPath);
			cell.TintColor = UIColor.Black;
			cell.TextLabel.Text = item;
			cell.Accessory = selectedItems.Contains (item) ? Checkmark : None;

			return cell;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return allItems.Count;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (allowsMultipleSelection) {
				var item = allItems [indexPath.Row];

				if (!selectedItems.Remove (item))
					selectedItems.Add (item);

				tableView.DeselectRow (indexPath, true);
				tableView.ReloadRows (new NSIndexPath [] { indexPath }, UITableViewRowAnimation.Automatic);
			} else {
				NSIndexPath [] indexPathsToReload;
				if (selectedItems.Count > 0) {
					indexPathsToReload = new NSIndexPath [] {
						indexPath,
						NSIndexPath.FromRowSection(allItems.FindIndex (v => v == selectedItems [0]), 0)
					};
				} else {
					indexPathsToReload = new NSIndexPath [] { indexPath };
				}

				selectedItems.Clear ();
				selectedItems.Add (allItems [indexPath.Row]);

				// Deselect the selected row & reload the table view cells for the old and new items to swap checkmarks.
				tableView.DeselectRow (indexPath, true);
				tableView.ReloadRows (indexPathsToReload, UITableViewRowAnimation.Automatic);
			}
		}

		#endregion
	}
}
