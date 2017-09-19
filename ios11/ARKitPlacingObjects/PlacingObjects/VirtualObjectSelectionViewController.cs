using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	[Register("VirtualObjectSelectionViewController")]
	public class VirtualObjectSelectionViewController : UITableViewController
	{
		public IVirtualObjectSelectionViewControllerDelegate Delegate { get; set; }

		private NSIndexSet selectedVirtualObjectRows = new NSIndexSet();

		public VirtualObjectSelectionViewController(NSCoder coder) : base(coder)
		{

		}

		public VirtualObjectSelectionViewController(IntPtr ptr) : base(ptr)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			TableView.SeparatorEffect = UIVibrancyEffect.FromBlurEffect(UIBlurEffect.FromStyle(UIBlurEffectStyle.Light));
		}

		public override void ViewWillLayoutSubviews()
		{
			PreferredContentSize = new CGSize(250, TableView.ContentSize.Height);
		}

		override public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(ObjectCell.Identifier, indexPath) as ObjectCell;
			cell.VirtualObject = VirtualObjectManager.AvailableObjects[indexPath.Row];

			if (selectedVirtualObjectRows.Contains((nuint)indexPath.Row))
			{
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			else
			{
				cell.Accessory = UITableViewCellAccessory.None;
			}
			return cell;
		}

		override public nint RowsInSection(UITableView tableView, nint section)
		{
			return VirtualObjectManager.AvailableObjects.Count;
		}

		override public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			// Check if the current row is already selected, then deselect it.
			if (selectedVirtualObjectRows.Contains((nuint)indexPath.Row))
			{
				Delegate?.DidDeselectObjectAt(indexPath.Row);
			}
			else
			{
				Delegate?.DidSelectObjectAt(indexPath.Row);
			}

			DismissViewController(true, null);
		}

		override public void RowHighlighted(UITableView tableView, NSIndexPath rowIndexPath)
		{
			var cell = TableView.CellAt(rowIndexPath);
			if (cell != null)
			{
				cell.BackgroundColor = UIColor.LightGray.ColorWithAlpha(0.5f);
			}
		}

		override public void RowUnhighlighted(UITableView tableView, NSIndexPath rowIndexPath)
		{
			var cell = TableView.CellAt(rowIndexPath);
			if (cell != null)
			{
				cell.BackgroundColor = UIColor.Clear;
			}
		}
	}
}
