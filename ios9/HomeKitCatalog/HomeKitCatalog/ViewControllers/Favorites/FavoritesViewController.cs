using System;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Lists favorite characteristics (grouped by accessory) and allows users to manipulate their values.
	partial class FavoritesViewController : UITableViewController, IUITabBarControllerDelegate, IHMAccessoryDelegate, IHMHomeManagerDelegate
	{
		static readonly NSString characteristicCell = (NSString)"CharacteristicCell";
		static readonly NSString segmentedControlCharacteristicCell = (NSString)"SegmentedControlCharacteristicCell";
		static readonly NSString switchCharacteristicCell = (NSString)"SwitchCharacteristicCell";
		static readonly NSString sliderCharacteristicCell = (NSString)"SliderCharacteristicCell";
		static readonly NSString textCharacteristicCell = (NSString)"TextCharacteristicCell";
		static readonly NSString serviceTypeCell = (NSString)"ServiceTypeCell";

		HMAccessory[] favoriteAccessories = FavoritesManager.SharedManager.FavoriteAccessories ();
		AccessoryUpdateController cellDelegate = new AccessoryUpdateController ();

		// If `true`, the characteristic cells should show stars.
		bool showsFavorites;
		bool ShowsFavorites {
			get {
				return showsFavorites;
			}
			set {
				showsFavorites = value;
				EditButton.Title = showsFavorites ? "Done" : "Edit";
				ReloadData ();
			}
		}

		#region ctors

		public FavoritesViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public FavoritesViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			TableView.EstimatedRowHeight = 44f;
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.AllowsSelectionDuringEditing = true;

			RegisterReuseIdentifiers ();

			var tabBarController = TabBarController;
			if (tabBarController != null)
				tabBarController.Delegate = this;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			RegisterAsDelegate ();
			SetNotificationsEnabled (true);
			ReloadData ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			SetNotificationsEnabled (false);

			// We don't want any more callbacks once the view has disappeared.
			HomeStore.SharedStore.HomeManager.Delegate = null;
		}

		void RegisterReuseIdentifiers ()
		{
			var characteristicNib = UINib.FromName (characteristicCell, null);
			TableView.RegisterNibForCellReuse (characteristicNib, characteristicCell);

			var sliderNib = UINib.FromName (sliderCharacteristicCell, null);
			TableView.RegisterNibForCellReuse (sliderNib, sliderCharacteristicCell);

			var switchNib = UINib.FromName (switchCharacteristicCell, null);
			TableView.RegisterNibForCellReuse (switchNib, switchCharacteristicCell);

			var segmentedNib = UINib.FromName (segmentedControlCharacteristicCell, null);
			TableView.RegisterNibForCellReuse (segmentedNib, segmentedControlCharacteristicCell);

			var textNib = UINib.FromName (textCharacteristicCell, null);
			TableView.RegisterNibForCellReuse (textNib, textCharacteristicCell);

			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), serviceTypeCell);
		}


		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			var sectionCount = favoriteAccessories.Length;
			TableView.SetBackgroundMessage (sectionCount == 0 ? "No Favorite Characteristics" : null);
			return sectionCount;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var accessory = favoriteAccessories [section];
			var characteristics = FavoritesManager.SharedManager.FavoriteCharacteristicsForAccessory (accessory);
			return characteristics.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var characteristics = FavoritesManager.SharedManager.FavoriteCharacteristicsForAccessory (favoriteAccessories [indexPath.Section]);
			var characteristic = characteristics [indexPath.Row];
			var reuseIdentifier = characteristicCell;

			if (characteristic.IsReadOnly () || characteristic.IsWriteOnly ())
				reuseIdentifier = characteristicCell;
			else if (characteristic.IsBoolean ())
				reuseIdentifier = switchCharacteristicCell;
			else if (characteristic.HasPredeterminedValueDescriptions ())
				reuseIdentifier = segmentedControlCharacteristicCell;
			else if (characteristic.IsNumeric ())
				reuseIdentifier = sliderCharacteristicCell;
			else if (characteristic.IsTextWritable ())
				reuseIdentifier = textCharacteristicCell;

			var cell = (CharacteristicCell)tableView.DequeueReusableCell (reuseIdentifier, indexPath);
			cell.ShowsFavorites = ShowsFavorites;
			cell.Delegate = cellDelegate;
			cell.Characteristic = characteristic;

			return cell;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return favoriteAccessories [(int)section].Name;
		}

		#endregion

		[Export ("didTapEdit:")]
		void DidTapEdit (UIBarButtonItem sender)
		{
			ShowsFavorites = !ShowsFavorites;
		}

		#region Helper Methods

		// Resets the `favoriteAccessories` array from the `FavoritesManager`, resets the state of the edit button, and reloads the data.
		void ReloadData ()
		{
			favoriteAccessories = FavoritesManager.SharedManager.FavoriteAccessories ();
			EditButton.Enabled = favoriteAccessories.Length > 0;
			TableView.ReloadData ();
		}

		// Enables or disables notifications for all favorite characteristics which support event notifications.
		static void SetNotificationsEnabled (bool notificationsEnabled)
		{
			foreach (var characteristic in FavoritesManager.SharedManager.FavoriteCharacteristics()) {
				if (characteristic.SupportsEventNotification) {
					characteristic.EnableNotification (notificationsEnabled, error => {
						if (error != null)
							Console.WriteLine ("HomeKit: Error enabling notification on characteristic {0}: {1}.", characteristic, error.LocalizedDescription);
					});
				}
			}
		}

		// Registers as the delegate for the home manager and all favorite accessories.
		void RegisterAsDelegate ()
		{
			HomeStore.SharedStore.HomeManager.Delegate = this;
			foreach (var accessory in favoriteAccessories)
				accessory.Delegate = this;
		}

		#endregion

		#region HMAccessoryDelegate Methods

		// Update the view to disable cells with unavailable accessories.
		[Export ("accessoryDidUpdateReachability:")]
		public void DidUpdateReachability (HMAccessory accessory)
		{
			ReloadData ();
		}

		// Search for the cell corresponding to that characteristic and update its value.
		[Export ("accessory:service:didUpdateValueForCharacteristic:")]
		public void DidUpdateValueForCharacteristic (HMAccessory accessory, HMService service, HMCharacteristic characteristic)
		{
			var s = characteristic.Service;
			if (s == null)
				return;

			var a = service.Accessory;
			if (a == null)
				return;

			var indexOfAccessory = Array.IndexOf (favoriteAccessories, accessory);
			if (indexOfAccessory < 0)
				return;

			var favoriteCharacteristics = FavoritesManager.SharedManager.FavoriteCharacteristicsForAccessory (accessory);
			var indexOfCharacteristic = Array.IndexOf (favoriteCharacteristics, characteristic);
			if (indexOfCharacteristic < 0)
				return;

			var indexPath = NSIndexPath.FromRowSection (indexOfCharacteristic, indexOfAccessory);
			var cell = (CharacteristicCell)TableView.CellAt (indexPath);
			cell.SetValue (characteristic.Value, false);
		}

		#endregion

		#region HMHomeManagerDelegate Methods

		[Export ("homeManagerDidUpdateHomes:")]
		public void DidUpdateHomes (HMHomeManager manager)
		{
			RegisterAsDelegate ();
			SetNotificationsEnabled (true);
			ReloadData ();
		}

		#endregion
	}
}