using System;
using System.Collections.Generic;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Allows for the selection of characteristics. This is mainly used for creating characteristic events and conditions
	public partial class CharacteristicSelectionViewController : HMCatalogViewController
	{
		static readonly NSString AccessoryCell = (NSString)"AccessoryCell";
		static readonly NSString UnreachableAccessoryCell = (NSString)"UnreachableAccessoryCell";
		const string ShowServicesSegue = "Show Services";

		readonly List<HMAccessory> accessories = new List<HMAccessory> ();

		[Outlet ("saveButton")]
		public UIBarButtonItem  SaveButton { get; set; }

		public HMEventTrigger EventTrigger { get; set; }

		public EventTriggerCreator TriggerCreator { get; set; }

		#region ctors

		public CharacteristicSelectionViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CharacteristicSelectionViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Only take accessories which have one control service.
			accessories.Clear ();
			accessories.AddRange (Home.SortedControlAccessories ());
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == ShowServicesSegue) {
				var senderCell = (UITableViewCell)sender;
				var servicesVC = (ServicesViewController)segue.IntendedDestinationViewController ();
				var cellIndex = TableView.IndexPathForCell (senderCell).Row;
				servicesVC.AllowsAllWrites = true;
				servicesVC.OnlyShowsControlServices = true;
				servicesVC.Accessory = accessories [cellIndex];
				servicesVC.CellDelegate = TriggerCreator;
			}
		}

		// Updates the predicates in the trigger creator and then
		// dismisses the view controller.
		[Export ("didTapSave:")]
		void didTapSave (UIBarButtonItem sender)
		{
			// We should not save the trigger completely, the user still has a chance to bail out.
			// Instead, we generate all of the predicates that were in the map.
			TriggerCreator.UpdatePredicates ();
			DismissViewController (true, null);
		}

		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Math.Max (accessories.Count, 1);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			accessories.SortByLocalizedName (a => a.Name);
			var accessory = accessories [indexPath.Row];
			var cellIdentifier = accessory.Reachable ? AccessoryCell : UnreachableAccessoryCell;

			var cell = tableView.DequeueReusableCell (cellIdentifier, indexPath);
			cell.TextLabel.Text = accessory.Name;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected (tableView, indexPath);

			tableView.DeselectRow (indexPath, true);
			var cell = tableView.CellAt (indexPath);
			if (cell.SelectionStyle == UITableViewCellSelectionStyle.None)
				return;

			PerformSegue (ShowServicesSegue, cell);
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Accessories";
		}

		#endregion
	}
}