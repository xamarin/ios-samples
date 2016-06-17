using System;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller which displays a list of `HMServices`, separated by Service Type.
	public partial class ControlsViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		const string ShowServiceSegue = "Show Service";

		readonly AccessoryUpdateController cellController = new AccessoryUpdateController ();

		ControlsTableViewDataSource tableViewDataSource;

		[Outlet ("addButton")]
		UIBarButtonItem addButton { get; set; }

		#region ctors

		public ControlsViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ControlsViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == ShowServiceSegue) {
				var indexPath = TableView.IndexPathForCell ((UITableViewCell)sender);
				if (indexPath != null) {
					var characteristicsViewController = (CharacteristicsViewController)segue.IntendedDestinationViewController ();
					var selectedService = tableViewDataSource.ServiceForIndexPath (indexPath);
					if (selectedService != null)
						characteristicsViewController.Service = selectedService;

					characteristicsViewController.CellDelegate = cellController;
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			tableViewDataSource = new ControlsTableViewDataSource (TableView);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationItem.Title = Home.Name;
			ReloadData ();
		}

		#region Helper Methods

		void ReloadData ()
		{
			tableViewDataSource.ReloadTable ();
			var sections = tableViewDataSource.NumberOfSections (TableView);
			string message = sections == 0 ? tableViewDataSource.EmptyMessage () : null;
			TableView.SetBackgroundMessage (message);
		}

		#endregion

		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			foreach (var accessory in Home.Accessories)
				accessory.Delegate = this;
		}

		#region HMHomeDelegate Methods

		// Any delegate methods which could change data will reload the table view data source.

		[Export ("home:didAddAccessory:")]
		public void DidAddAccessory (HMHome home, HMAccessory accessory)
		{
			accessory.Delegate = this;
			ReloadData ();
		}

		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HMHome home, HMAccessory accessory)
		{
			ReloadData ();
		}

		#endregion

		#region HMAccessoryDelegate Methods

		[Export ("accessoryDidUpdateReachability:")]
		public void DidUpdateReachability (HMAccessory accessory)
		{
			ReloadData ();
		}

		[Export ("accessory:didUpdateNameForService:")]
		public void DidUpdateNameForService (HMAccessory accessory, HMService service)
		{
			ReloadData ();
		}

		[Export ("accessory:didUpdateAssociatedServiceTypeForService:")]
		public void DidUpdateAssociatedServiceType (HMAccessory accessory, HMService service)
		{
			ReloadData ();
		}

		[Export ("accessoryDidUpdateServices:")]
		public void DidUpdateServices (HMAccessory accessory)
		{
			ReloadData ();
		}

		[Export ("accessoryDidUpdateName:")]
		public void DidUpdateName (HMAccessory accessory)
		{
			ReloadData ();
		}

		#endregion
	}
}
