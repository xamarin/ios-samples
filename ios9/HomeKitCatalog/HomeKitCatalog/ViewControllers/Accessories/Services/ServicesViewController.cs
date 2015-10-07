using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	public enum AccessoryTableViewSection
	{
		Services,
		BridgedAccessories
	}

	// A view controller which displays all the services of a provided accessory, and passes its cell delegate onto a `CharacteristicsViewController`.
	public partial class ServicesViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		static readonly NSString AccessoryCell = (NSString)"AccessoryCell";
		static readonly NSString ServiceCell = (NSString)"ServiceCell";
		static readonly string ShowServiceSegue = "Show Service";

		List<HMService> displayedServices = new List<HMService> ();
		List<HMAccessory> bridgedAccessories = new List<HMAccessory> ();

		public bool AllowsAllWrites { get; set; }

		public bool OnlyShowsControlServices { get; set; }

		public bool ShowsFavorites { get; set; }

		public HMAccessory Accessory { get; set; }

		ICharacteristicCellDelegate cellDelegate;

		public ICharacteristicCellDelegate CellDelegate {
			get {
				cellDelegate = cellDelegate ?? new AccessoryUpdateController ();
				return cellDelegate;
			}
			set {
				cellDelegate = value;
			}
		}

		public ServicesViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ServicesViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.EstimatedRowHeight = 44f;
			TableView.RowHeight = UITableView.AutomaticDimension;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			UpdateTitle ();
			ReloadData ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			if (ShouldPopViewController ())
				NavigationController.PopToRootViewController (true);
		}

		// Passes the `CharacteristicsViewController` the service from the cell and configures the view controller.
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier != ShowServiceSegue)
				return;

			var indexPath = TableView.IndexPathForCell ((UITableViewCell)sender);
			if (indexPath == null)
				return;

			var selectedService = displayedServices [indexPath.Row];
			var characteristicsViewController = (CharacteristicsViewController)segue.IntendedDestinationViewController ();
			characteristicsViewController.ShowsFavorites = ShowsFavorites;
			characteristicsViewController.AllowsAllWrites = AllowsAllWrites;
			characteristicsViewController.Service = selectedService;
			characteristicsViewController.CellDelegate = CellDelegate;
		}

		// returns:  `true` if our accessory is no longer in the current home's list of accessories.
		bool ShouldPopViewController ()
		{
			foreach (var accessory in HomeStore.Home.Accessories) {
				if (Accessory == accessory)
					return false;
			}
			return true;
		}

		#region Delegate Registration

		// Registers as the delegate for the current home and for the current accessory.
		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			Accessory.Delegate = this;
		}

		#endregion

		#region Table View Methods

		// Two sections if we're showing bridged accessories.
		public override nint NumberOfSections (UITableView tableView)
		{
			return Accessory.UniqueIdentifiersForBridgedAccessories != null ? 2 : 1;
		}

		// Section 1 contains the services within the accessory.
		// Section 2 contains the bridged accessories.
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((AccessoryTableViewSection)(int)section) {
			case AccessoryTableViewSection.Services:
				return displayedServices.Count;
			case AccessoryTableViewSection.BridgedAccessories:
				return bridgedAccessories.Count;
			default:
				throw new InvalidOperationException ("Unexpected `AccessoryTableViewSection` value.");
			}
		}

		// returns:  A Service or Bridged Accessory Cell based on the section.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((AccessoryTableViewSection)indexPath.Section) {
			case AccessoryTableViewSection.Services:
				return GetServiceCell (tableView, indexPath);
			case AccessoryTableViewSection.BridgedAccessories:
				return GetBridgedAccessoryCell (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `AccessoryTableViewSection` value.");
			}
		}

		// returns:  A cell containing the name of a bridged accessory at a given index path.
		UITableViewCell GetBridgedAccessoryCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (AccessoryCell, indexPath);
			var accessory = bridgedAccessories [indexPath.Row];
			cell.TextLabel.Text = accessory.Name;
			return cell;
		}

		// returns:  A cell containing the name of a service at a given index path, as well as a localized description of its service type.
		UITableViewCell GetServiceCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (ServiceCell, indexPath);
			var service = displayedServices [indexPath.Row];

			// Inherit the name from the accessory if the Service doesn't have one.
			cell.TextLabel.Text = service.Name ?? service.Accessory.Name;

			var detailTextLabel = cell.DetailTextLabel;
			if (detailTextLabel != null)
				detailTextLabel.Text = service.LocalizedDescription;

			return cell;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			switch ((AccessoryTableViewSection)(int)section) {
			case AccessoryTableViewSection.Services:
				return "Services";
			case AccessoryTableViewSection.BridgedAccessories:
				return "Bridged Accessories";
			default:
				throw new InvalidOperationException ("Unexpected `AccessoryTableViewSection` value.");
			}
		}

		// returns:  A description of the accessories bridged status.
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			if (Accessory.Bridged && (AccessoryTableViewSection)(int)section == AccessoryTableViewSection.Services) {
				var bridge = Home.BridgeForAccessory (Accessory);
				return bridge != null ?
					string.Format ("This accessory is being bridged into HomeKit by {0}.", bridge.Name) :
					"This accessory is being bridged into HomeKit.";
			}
			return null;
		}

		#endregion

		#region Helper Methods

		/// Updates the navigation bar's title.
		void UpdateTitle ()
		{
			NavigationItem.Title = Accessory.Name;
		}

		// Updates the title, resets the displayed services based on
		// view controller configurations, reloads the bridge accessory
		// array and reloads the table view.
		void ReloadData ()
		{
			displayedServices.Clear ();
			displayedServices.AddRange (Accessory.Services);
			displayedServices.SortByLocalizedName (s => s.Name);

			if (OnlyShowsControlServices) {
				// We are configured to only show control services, filter the array.
				var filtered = displayedServices.Where (s => s.IsControllType ()).ToArray ();
				displayedServices.Clear ();
				displayedServices.AddRange (filtered);
			}

			NSUuid[] identifiers = Accessory.UniqueIdentifiersForBridgedAccessories;
			if (identifiers != null) {
				bridgedAccessories.Clear ();
				bridgedAccessories.AddRange (Home.AccessoriesWithIdentifiers (new HashSet<NSUuid> (identifiers)));
				bridgedAccessories.SortByLocalizedName (a => a.Name);
			}
			TableView.ReloadData ();
		}

		#endregion

		#region HMAccessoryDelegate Methods

		// Reloads the title based on the accessories new name.
		[Export ("accessoryDidUpdateName:")]
		public void DidUpdateName (HMAccessory accessory)
		{
			UpdateTitle ();
		}

		// Reloads the cell for the specified service.
		[Export ("accessory:didUpdateNameForService:")]
		public void DidUpdateNameForService (HMAccessory accessory, HMService service)
		{
			var index = displayedServices.IndexOf (service);
			if (index >= 0) {
				var path = NSIndexPath.FromRowSection (index, (int)AccessoryTableViewSection.Services);
				TableView.ReloadRows (new []{ path }, UITableViewRowAnimation.Automatic);
			}
		}

		[Export ("accessoryDidUpdateServices:")]
		public void DidUpdateServices (HMAccessory accessory)
		{
			ReloadData ();
		}

		// If our accessory has become unreachable, go back the previous view.
		[Export ("accessoryDidUpdateReachability:")]
		public void DidUpdateReachability (HMAccessory accessory)
		{
			if (Accessory == accessory)
				NavigationController.PopViewController (true);
		}

		#endregion
	}
}