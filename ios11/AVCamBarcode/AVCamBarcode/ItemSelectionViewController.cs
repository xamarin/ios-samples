
namespace AVCamBarcode {
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;

	public interface IItemSelectionViewControllerDelegate {
		void ItemSelectionViewController<T> (ItemSelectionViewController<T> itemSelectionViewController, IList<T> selectedItems);
	}

	public class ItemSelectionViewController<T> : UITableViewController {
		private const string itemCellIdentifier = "Item";

		private readonly IItemSelectionViewControllerDelegate @delegate;

		private bool isMultipleSelectionAllowed;

		private readonly IList<T> selectedItems;

		private readonly IList<T> items;

		public ItemSelectionViewController (IItemSelectionViewControllerDelegate @delegate,
										   string identifier,
										   IList<T> items,
										   IList<T> selectedItems,
										   bool isMultipleSelectionAllowed) : base (UITableViewStyle.Grouped)
		{
			this.@delegate = @delegate;
			this.Identifier = identifier;
			this.items = items;
			this.selectedItems = selectedItems;
			this.isMultipleSelectionAllowed = isMultipleSelectionAllowed;

			base.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, (sender, args) => this.Done ());
			this.TableView.RegisterClassForCellReuse (typeof (UITableViewCell), itemCellIdentifier);
		}

		public string Identifier { get; private set; }

		private void Done ()
		{
			// Notify the delegate that selecting items is finished.
			this.@delegate?.ItemSelectionViewController (this, this.selectedItems);

			// Dismiss the view controller.
			this.DismissViewController (true, null);
		}

		#region UITableView

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = this.items [indexPath.Row];

			var cell = tableView.DequeueReusableCell (itemCellIdentifier, indexPath);
			cell.TintColor = UIColor.Black;
			cell.TextLabel.Text = item.ToString ();
			cell.Accessory = this.selectedItems.Contains (item) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return this.items.Count;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			NSIndexPath [] indexPathsToReload = null;
			if (this.isMultipleSelectionAllowed) {
				var item = this.items [indexPath.Row];
				if (!this.selectedItems.Remove (item)) {
					this.selectedItems.Add (item);
				}

				indexPathsToReload = new NSIndexPath [] { indexPath };
			} else {
				indexPathsToReload = !this.selectedItems.Any ()
									 ? new NSIndexPath [] { indexPath }
									 : this.selectedItems.Select (item => this.items.IndexOf (item))
														 .Select (index => NSIndexPath.FromRowSection (index, 0))
														 .Concat (new NSIndexPath [] { indexPath })
														 .ToArray ();

				this.selectedItems.Clear ();
				this.selectedItems.Add (this.items [indexPath.Row]);
			}

			// Deselect the selected row & reload the table view cells for the old and new items to swap checkmarks.
			tableView.DeselectRow (indexPath, true);
			tableView.ReloadRows (indexPathsToReload, UITableViewRowAnimation.Automatic);
		}

		#endregion
	}
}
