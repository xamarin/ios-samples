using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller that allows the user to add services to a service group.
	public partial class ServiceGroupViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		static readonly NSString ServiceCell = (NSString)"ServiceCell";
		const string AddServicesSegue = "Add Services Plus";

		[Outlet ("plusButton")]
		public UIBarButtonItem PlusButton { get; set; }

		readonly List<HMAccessory> accessories = new List<HMAccessory> ();
		readonly Dictionary<HMAccessory, List<HMService>> servicesForAccessory = new Dictionary<HMAccessory, List<HMService>> ();

		public HMServiceGroup ServiceGroup { get; set; }

		#region ctors

		public ServiceGroupViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ServiceGroupViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			Title = ServiceGroup.Name;
			ReloadData ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (ShouldPopViewController ())
				NavigationController.PopViewController (true);
		}

		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			var sections = accessories.Count;
			TableView.SetBackgroundMessage (sections == 0 ? "No Services" : null);
			return sections;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var accessory = accessories [(int)section];
			List<HMService> services;
			return servicesForAccessory.TryGetValue (accessory, out services) ? services.Count : 0;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return accessories [(int)section].Name;
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ServiceCell)tableView.DequeueReusableCell (ServiceCell, indexPath);
			var service = ServiceAtIndexPath (indexPath);
			cell.IncludeAccessoryText = false;
			cell.Service = service;
			return cell;
		}

		bool ShouldEnableAdd ()
		{
			var unAddedServices = Home.ServicesNotAlreadyInServiceGroup (ServiceGroup);
			return unAddedServices.Any ();
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete)
				RemoveServiceAtIndexPath (indexPath);
		}

		void RemoveServiceAtIndexPath (NSIndexPath indexPath)
		{
			var service = ServiceAtIndexPath (indexPath);
			ServiceGroup.RemoveService (service, error => {
				if (error != null)
					DisplayError (error);
				ReloadData ();
			});
		}

		HMService ServiceAtIndexPath (NSIndexPath indexPath)
		{
			var accessory = accessories [indexPath.Section];
			var services = servicesForAccessory [accessory];
			return services [indexPath.Row];
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == AddServicesSegue) {
				var addServicesVC = (AddServicesViewController)segue.IntendedDestinationViewController ();
				addServicesVC.ServiceGroup = ServiceGroup;
			}
		}

		#endregion

		#region Helper Methods

		void ReloadData ()
		{
			ResetLists ();
			PlusButton.Enabled = ShouldEnableAdd ();
			TableView.ReloadData ();
		}

		void ResetLists ()
		{
			accessories.Clear ();
			servicesForAccessory.Clear ();

			foreach (var service in ServiceGroup.Services) {
				var accessory = service.Accessory;
				if (accessory != null) {
					List<HMService> list;
					if (!servicesForAccessory.TryGetValue (accessory, out list)) {
						accessories.Add (accessory);
						servicesForAccessory [accessory] = new List<HMService> { service };
					} else {
						list.Add (service);
					}
				}
			}

			// Sort all service lists.
			foreach (var accessory in accessories) {
				List<HMService> list;
				if (servicesForAccessory.TryGetValue (accessory, out list)) {
					list.SortByLocalizedName (a => a.Name);
				}

				// Sort accessory list.
				accessories.SortByLocalizedName (a => a.Name);
			}
		}

		bool ShouldPopViewController ()
		{
			var home = HomeStore.Home;
			return home == null || home.ServiceGroups.All (g => g != ServiceGroup);
		}

		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			foreach (var service in ServiceGroup.Services) {
				var accessory = service.Accessory;
				if (accessory != null)
					accessory.Delegate = this;
			}
		}

		#endregion

		#region HMHomeDelegate Methods

		[Export ("home:didRemoveServiceGroup:")]
		public void DidRemoveServiceGroup (HMHome home, HMServiceGroup group)
		{
			if (group == ServiceGroup)
				NavigationController.PopViewController (true);
		}

		// Home and accessory changes result in a full data reload.

		[Export ("home:didAddService:toServiceGroup:")]
		public void DidAddService (HMHome home, HMService service, HMServiceGroup group)
		{
			if (ServiceGroup == group)
				ReloadData ();
		}

		[Export ("home:didRemoveService:fromServiceGroup:")]
		public void DidRemoveService (HMHome home, HMService service, HMServiceGroup group)
		{
			if (ServiceGroup == group)
				ReloadData ();
		}

		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HMHome home, HMAccessory accessory)
		{
			ReloadData ();
		}

		[Export ("accessoryDidUpdateServices:")]
		public void DidUpdateServices (HMAccessory accessory)
		{
			ReloadData ();
		}

		[Export ("accessory:didUpdateNameForService:")]
		public void DidUpdateNameForService (HMAccessory accessory, HMService service)
		{
			ReloadData ();
		}

		#endregion
	}
}
