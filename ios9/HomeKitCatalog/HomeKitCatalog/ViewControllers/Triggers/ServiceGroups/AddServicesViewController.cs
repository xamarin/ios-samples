using System;
using System.Collections.Generic;
using System.Linq;

using CoreFoundation;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller that provides a list of services and lets the user select services to be added to the provided Service Group.
	// The services are not added to the service group until the 'Done' button is pressed.
	public partial class AddServicesViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		static readonly NSString ServiceCell = (NSString)"ServiceCell";

		readonly List<HMAccessory> displayedAccessories = new List<HMAccessory> ();
		readonly Dictionary<HMAccessory, List<HMService>> displayedServicesForAccessory = new Dictionary<HMAccessory, List<HMService>> ();
		readonly List<HMService> selectedServices = new List<HMService> ();

		public HMServiceGroup ServiceGroup { get; set; }

		#region ctors

		public AddServicesViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AddServicesViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			selectedServices.Clear ();
			ReloadTable ();
		}

		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			foreach (var accessory in HomeStore.Home.Accessories)
				accessory.Delegate = this;
		}

		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			return displayedAccessories.Count;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var accessory = displayedAccessories [(int)section];
			return displayedServicesForAccessory [accessory].Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ServiceCell)tableView.DequeueReusableCell (ServiceCell, indexPath);

			var service = ServiceAtIndexPath (indexPath);

			cell.IncludeAccessoryText = false;
			cell.Service = service;
			cell.Accessory = selectedServices.Contains (service) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Get the service associated with this index.
			var service = ServiceAtIndexPath (indexPath);

			// Call the appropriate add/remove operation with the closure from above.
			var index = selectedServices.IndexOf (service);
			if (index >= 0)
				selectedServices.RemoveAt (index);
			else
				selectedServices.Add (service);

			tableView.ReloadRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return displayedAccessories [(int)section].Name;
		}

		#endregion

		#region Helper Methods

		void AddSelectedServices (Action completion)
		{
			// Create a dispatch group for each of the service additions.
			var addServicesGroup = DispatchGroup.Create ();
			foreach (var service in selectedServices) {
				addServicesGroup.Enter ();
				ServiceGroup.AddService (service, error => {
					if (error != null)
						DisplayError (error);
					addServicesGroup.Leave ();
				});
			}
			addServicesGroup.Notify (DispatchQueue.MainQueue, completion);
		}

		HMService ServiceAtIndexPath (NSIndexPath indexPath)
		{
			var accessory = displayedAccessories [indexPath.Section];
			var services = displayedServicesForAccessory [accessory];
			return services [indexPath.Row];
		}

		[Export ("dismiss")]
		void Dismiss ()
		{
			AddSelectedServices (() => DismissViewController (true, null));
		}

		// Resets internal data and view.
		void ReloadTable ()
		{
			ResetDisplayedServices ();
			TableView.ReloadData ();
		}

		// Updates internal array of accessories and the mapping of accessories to selected services.
		void ResetDisplayedServices ()
		{
			displayedAccessories.Clear ();
			var allAccessories = new List<HMAccessory> (Home.Accessories);
			allAccessories.SortByLocalizedName (a => a.Name);
			displayedServicesForAccessory.Clear ();
			foreach (var accessory in allAccessories) {
				var displayedServices = new List<HMService> ();
				foreach (var service in accessory.Services) {
					if (!ServiceGroup.Services.Contains (service) && service.ServiceType != HMServiceType.AccessoryInformation)
						displayedServices.Add (service);
				}

				// Only add the accessory if it has displayed services.
				if (displayedServices.Count > 0) {
					displayedServices.SortByLocalizedName (a => a.Name);
					displayedServicesForAccessory [accessory] = displayedServices;
					displayedAccessories.Add (accessory);
				}
			}
		}

		#endregion

		#region HMHomeDelegate Methods

		[Export ("home:didRemoveServiceGroup:")]
		public void DidRemoveServiceGroup (HMHome home, HMServiceGroup group)
		{
			if (ServiceGroup == group)
				DismissViewController (true, null);
		}

		[Export ("home:didAddAccessory:")]
		public void DidAddAccessory (HMHome home, HMAccessory accessory)
		{
			ReloadTable ();
			accessory.Delegate = this;
		}

		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HMHome home, HMAccessory accessory)
		{
			if (home.Accessories.Length == 0)
				NavigationController.DismissViewController (true, null);

			ReloadTable ();
		}

		#endregion

		#region HMAccessoryDelegate Methods

		[Export ("accessory:didUpdateNameForService:")]
		public void DidUpdateNameForService (HMAccessory accessory, HMService service)
		{
			ReloadTable ();
		}

		[Export ("accessoryDidUpdateServices:")]
		public void DidUpdateServices (HMAccessory accessory)
		{
			ReloadTable ();
		}

		#endregion
	}
}
