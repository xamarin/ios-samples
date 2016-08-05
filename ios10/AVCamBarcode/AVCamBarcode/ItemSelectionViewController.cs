using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using static UIKit.UITableViewCellAccessory;

namespace AVCamBarcode
{
	public interface ItemSelectionViewControllerDelegate
	{
		void ItemSelectionViewController (ItemSelectionViewController itemSelectionViewController, List<string> selectedItems);
	}

	[Register ("ItemSelectionViewController")]
	public class ItemSelectionViewController : UITableViewController
	{
		public string[] AllItems { get; set; }
		public List<string> SelectedItems { get; set; }

		public bool AllowsMultipleSelection { get; set; }

		public string Identifier { get; set; } = string.Empty;
		public ItemSelectionViewControllerDelegate Delegate { get; set; }

		[Action ("done")]
		void Done ()
		{
			// Notify the delegate that selecting items is finished.
			Delegate?.ItemSelectionViewController (this, SelectedItems);

			// Dismiss the view controller.
			DismissViewController (true, null);
		}

		public ItemSelectionViewController (IntPtr handle)
			: base (handle)
		{
		}


		//[Export ("initWithCoder:")]
		//public ItemSelectionViewController (NSCoder coder)
		//	: base (coder)
		//{
		//}


		#region UITableViewDataSource

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = AllItems [indexPath.Row];

			var cell = tableView.DequeueReusableCell ("Item", indexPath);
			cell.TintColor = UIColor.Black;
			cell.TextLabel.Text = item;
			cell.Accessory =  SelectedItems.Contains(item) ? Checkmark : None;

			return cell;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return AllItems.Length;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			NSIndexPath [] indexPathsToReload = null;
			if (AllowsMultipleSelection) {
				var item = AllItems [indexPath.Row];

				// Togle selection
				if (!SelectedItems.Remove (item))
					SelectedItems.Add (item);

				indexPathsToReload = new NSIndexPath [] { indexPath };
			} else {
				indexPathsToReload = (SelectedItems.Count == 0)
									? new NSIndexPath [] { indexPath }
									: SelectedItems.Select (f => Array.IndexOf(AllItems, f))
												   .Select (i => NSIndexPath.FromRowSection (i, 0))
												   .Concat (indexPath)
												   .ToArray ();

				SelectedItems.Clear ();
				SelectedItems.Add (AllItems [indexPath.Row]);
			}

			// Deselect the selected row & reload the table view cells for the old and new items to swap checkmarks.
			tableView.DeselectRow (indexPath, true);
			tableView.ReloadRows (indexPathsToReload, UITableViewRowAnimation.Automatic);
		}

		#endregion
	}
}
