using System;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller that displays a list of characteristics within an `HMService`.
	public partial class CharacteristicsViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		CharacteristicsTableViewDataSource tableViewDataSource;

		public HMService Service { get; set; }

		public bool ShowsFavorites { get; set; }

		public bool AllowsAllWrites { get; set; }

		public ICharacteristicCellDelegate CellDelegate { get; set; }

		#region ctors

		public CharacteristicsViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CharacteristicsViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			tableViewDataSource = new CharacteristicsTableViewDataSource (Service, TableView, CellDelegate, ShowsFavorites, AllowsAllWrites);
		}

		// Reloads the view and enabled notifications for all relevant characteristics.
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Title = Service.Name;
			SetNotificationsEnabled (true);
			ReloadTableView ();
		}

		// Disables notifications for characteristics.
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			SetNotificationsEnabled (false);
		}

		// Registers as the delegate for the current home and the service's accessory.
		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			var accessory = Service.Accessory;
			if (accessory != null)
				accessory.Delegate = this;
		}

		// Enables or disables notifications on all characteristics within this service.
		void SetNotificationsEnabled (bool notificationsEnabled)
		{
			foreach (var characteristic in Service.Characteristics) {
				if (characteristic.SupportsEventNotification) {
					characteristic.EnableNotification (notificationsEnabled, error => {
						if (error != null)
							Console.WriteLine ("HomeKit: Error enabling notification on charcteristic {0}: {1}", characteristic, error.LocalizedDescription);
					});
				}
			}
		}

		// Reloads the table view and stops the refresh control.
		[Export("reloadTableView")]
		void ReloadTableView ()
		{
			SetNotificationsEnabled (true);
			tableViewDataSource.Service = Service;
			var refresh = RefreshControl;
			if (refresh != null)
				refresh.EndRefreshing ();
			TableView.ReloadData ();
		}

		#region Table View Methods

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((CharacteristicTableViewSection)indexPath.Section) {
			case CharacteristicTableViewSection.Characteristics:
				var characteristic = Service.Characteristics [indexPath.Row];
				DidSelectCharacteristic (characteristic, indexPath);
				break;
			case CharacteristicTableViewSection.AssociatedServiceType:
				DidSelectAssociatedServiceTypeAtIndexPath (indexPath);
				break;
			default:
				throw new InvalidOperationException ("Unexpected `CharacteristicTableViewSection` value.");
			}
		}

		// If a characteristic is selected, and it is the 'Identify' characteristic, perform an identify on that accessory.
		void DidSelectCharacteristic (HMCharacteristic characteristic, NSIndexPath indexPath)
		{
			if (characteristic.IsIdentify ()) {
				var accessory = Service.Accessory;
				if (accessory != null)
					accessory.Identify (error => {
						if (error != null)
							DisplayError (error);
					});
			}
		}

		// Handles selection of one of the associated service types in the list.
		void DidSelectAssociatedServiceTypeAtIndexPath (NSIndexPath indexPath)
		{
			HMServiceType[] serviceTypes = CharacteristicsTableViewDataSource.ValidAssociatedServiceTypes;
			HMServiceType newServiceType = HMServiceType.None;
			if (indexPath.Row < serviceTypes.Length)
				newServiceType = serviceTypes [indexPath.Row];

			NSString typeName = HMServiceKeys.Convert (newServiceType);
			Service.UpdateAssociatedServiceType (typeName, error => {
				if (error != null) {
					DisplayError (error);
					return;
				}

				DidUpdateAssociatedServiceType ();
			});
		}

		// Reloads the associated service section in the table view.
		void DidUpdateAssociatedServiceType ()
		{
			var associatedServiceTypeIndexSet = new NSIndexSet ((nuint)(int)CharacteristicTableViewSection.AssociatedServiceType);
			TableView.ReloadSections (associatedServiceTypeIndexSet, UITableViewRowAnimation.Automatic);
		}

		#endregion

		#region HMHomeDelegate Methods

		// If our accessory was removed, pop to root view controller.
		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HMHome home, HMAccessory accessory)
		{
			if (accessory == Service.Accessory)
				NavigationController.PopToRootViewController (true);
		}

		#endregion

		#region HMAccessoryDelegate Methods

		// If our accessory becomes unreachable, pop to root view controller.
		[Export ("accessoryDidUpdateReachability:")]
		public void DidUpdateReachability (HMAccessory accessory)
		{
			if (accessory == Service.Accessory && !accessory.Reachable)
				NavigationController.PopToRootViewController (true);
		}

		// Search for the cell corresponding to that characteristic and update its value.
		[Export ("accessory:service:didUpdateValueForCharacteristic:")]
		public void DidUpdateValueForCharacteristic (HMAccessory accessory, HMService service, HMCharacteristic characteristic)
		{
			var index = Array.IndexOf (service.Characteristics, characteristic);
			if (index >= 0) {
				var indexPath = NSIndexPath.FromRowSection (index, 0);
				var cell = (CharacteristicCell)TableView.CellAt (indexPath);
				cell.SetValue (characteristic.Value, false);
			}
		}

		#endregion
	}
}
