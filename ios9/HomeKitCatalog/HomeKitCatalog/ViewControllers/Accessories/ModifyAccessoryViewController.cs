using System;
using System.Linq;

using CoreFoundation;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Represents the sections in the `ModifyAccessoryViewController`.
	public enum AddAccessoryTableViewSection
	{
		Name,
		Rooms,
		Identify
	}

	public interface IModifyAccessoryDelegate
	{
		void AccessoryViewControllerDidSaveAccessory (ModifyAccessoryViewController accessoryViewController, HMAccessory accessory);
	}

	// A view controller that allows for renaming, reassigning, and identifying accessories before and after they've been added to a home.
	public partial class ModifyAccessoryViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		static readonly NSString RoomCell = (NSString)"RoomCell";

		// Update this if the acessory failed in any way.
		bool didEncounterError;
		bool editingExistingAccessory;
		HMRoom selectedRoom;

		UIActivityIndicatorView activityIndicator;

		UIActivityIndicatorView ActivityIndicator {
			get {
				activityIndicator = activityIndicator ?? new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
				return activityIndicator;
			}
		}

		// returns:  The `nameField`'s text, trimmed of newline and whitespace characters.
		string TrimmedName {
			get {
				return NameField.Text.Trim ();
			}
		}

		readonly DispatchGroup saveAccessoryGroup = DispatchGroup.Create ();
		public IModifyAccessoryDelegate Delegate { get; set; }

		public HMAccessory Accessory { get; set; }

		[Outlet ("nameField")]
		UITextField NameField { get; set; }

		[Outlet ("addButton")]
		UIBarButtonItem AddButton { get; set; }

		#region ctors

		public ModifyAccessoryViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ModifyAccessoryViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		// Configures the table view and initializes view elements.
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.EstimatedRowHeight = 44;
			TableView.RowHeight = UITableView.AutomaticDimension;

			selectedRoom = Accessory.Room ?? Home.GetRoomForEntireHome ();

			// If the accessory belongs to the home already, we are in 'edit' mode.
			editingExistingAccessory = AccessoryHasBeenAddedToHome ();

			// Show 'save' instead of 'add.'
			// If we're not editing an existing accessory, then let the back button show in the left.
			if (editingExistingAccessory)
				AddButton.Title = "Save";
			else
				NavigationItem.LeftBarButtonItem = null;

			// Put the accessory's name in the 'name' field.
			ResetNameField ();

			// Register a cell for the rooms.
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), RoomCell);
		}

		// Registers as the delegate for the current home and the accessory.
		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			Accessory.Delegate = this;
		}

		// Replaces the activity indicator with the 'Add' or 'Save' button.
		void HideActivityIndicator ()
		{
			ActivityIndicator.StopAnimating ();
			NavigationItem.RightBarButtonItem = AddButton;
		}

		// Temporarily replaces the 'Add' or 'Save' button with an activity indicator.
		void ShowActivityIndicator ()
		{
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (ActivityIndicator);
			ActivityIndicator.StartAnimating ();
		}

		// Called whenever the user taps the 'add' button.
		//
		// This method:
		// 1. Adds the accessory to the home, if not already added.
		// 2. Updates the accessory's name, if necessary.
		// 3. Assigns the accessory to the selected room, if necessary.
		[Export ("didTapAddButton")]
		void DidTapAddButton ()
		{
			var name = TrimmedName;
			ShowActivityIndicator ();

			if (editingExistingAccessory) {
				AssignAccessory (Home, Accessory, selectedRoom);
				UpdateName (name, Accessory);
			} else {
				saveAccessoryGroup.Enter ();
				Home.AddAccessory (Accessory, error => {
					if (error != null) {
						HideActivityIndicator ();
						DisplayError (error);
						didEncounterError = true;
					} else {
						// Once it's successfully added to the home, add it to the room that's selected.
						AssignAccessory (Home, Accessory, selectedRoom);
						UpdateName (name, Accessory);
					}
					saveAccessoryGroup.Leave ();
				});
			}

			saveAccessoryGroup.Notify (DispatchQueue.MainQueue, () => {
				HideActivityIndicator ();
				if (!didEncounterError)
					Dismiss (null);
			});
		}

		// Informs the delegate that the accessory has been saved, and
		// dismisses the view controller.
		[Export ("dismiss:")]
		void Dismiss (NSObject sender)
		{
			if (Delegate != null)
				Delegate.AccessoryViewControllerDidSaveAccessory (this, Accessory);
			if (editingExistingAccessory)
				PresentingViewController.DismissViewController (true, null);
			else
				NavigationController.PopViewController (true);
		}

		// returns: `true` if the accessory has already been added to the home; `false` otherwise.
		bool AccessoryHasBeenAddedToHome ()
		{
			return Home.Accessories.Contains (Accessory);
		}

		// Updates the accessories name. This function will enter and leave the saved dispatch group.
		// If the accessory's name is already equal to the passed-in name, this method does nothing.
		void UpdateName (string name, HMAccessory accessory)
		{
			if (accessory.Name == name)
				return;

			saveAccessoryGroup.Enter ();
			accessory.UpdateName (name, error => {
				if (error != null) {
					DisplayError (error);
					didEncounterError = true;
				}
				saveAccessoryGroup.Leave ();
			});
		}

		// Assigns the given accessory to the provided room. This method will enter and leave the saved dispatch group.
		void AssignAccessory (HMHome home, HMAccessory accessory, HMRoom room)
		{
			if (accessory.Room == room)
				return;

			saveAccessoryGroup.Enter ();
			home.AssignAccessory (accessory, room, error => {
				if (error != null) {
					DisplayError (error);
					didEncounterError = true;
				}
				saveAccessoryGroup.Leave ();
			});
		}


		// Tells the current accessory to identify itself.
		void IdentifyAccessory ()
		{
			Accessory.Identify (error => {
				if (error != null)
					DisplayError (error);
			});
		}

		// Enables the name field if the accessory's name changes.
		void ResetNameField ()
		{
			string action;
			action = editingExistingAccessory ? "Edit {0}" : "Add {0}";
			NavigationItem.Title = string.Format (action, Accessory.Name);
			NameField.Text = Accessory.Name;
			NameField.Enabled = Home.IsAdmin ();
			EnableAddButtonIfApplicable ();
		}

		// Enables the save button if the name field is not empty.
		void EnableAddButtonIfApplicable ()
		{
			AddButton.Enabled = Home.IsAdmin () && !string.IsNullOrEmpty (TrimmedName);
		}

		// Enables or disables the add button.
		[Export ("nameFieldDidChange:")]
		void NameFieldDidChange (NSObject sender)
		{
			EnableAddButtonIfApplicable ();
		}

		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			return Enum.GetNames (typeof(AddAccessoryTableViewSection)).Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((AddAccessoryTableViewSection)(int)section) {
			case AddAccessoryTableViewSection.Rooms:
				return Home.GetAllRooms ().Length;
			case AddAccessoryTableViewSection.Identify:
			case AddAccessoryTableViewSection.Name:
				return base.RowsInSection (tableView, section);
			default:
				throw new InvalidOperationException ("Unexpected `AddAccessoryTableViewSection` value.");
			}
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((AddAccessoryTableViewSection)indexPath.Section) {
			case AddAccessoryTableViewSection.Rooms:
				return UITableView.AutomaticDimension;
			case AddAccessoryTableViewSection.Identify:
			case AddAccessoryTableViewSection.Name:
				return base.GetHeightForRow (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `AddAccessoryTableViewSection` value.");
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((AddAccessoryTableViewSection)indexPath.Section) {
			case AddAccessoryTableViewSection.Rooms:
				return GetCellForRoom (tableView, indexPath);
			case AddAccessoryTableViewSection.Identify:
			case AddAccessoryTableViewSection.Name:
				return base.GetCell (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `AddAccessoryTableViewSection` value.");
			}
		}

		UITableViewCell GetCellForRoom (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (RoomCell, indexPath);
			var room = Home.GetAllRooms () [indexPath.Row];

			cell.TextLabel.Text = Home.GetNameForRoom (room);

			// Put a checkmark on the selected room.
			cell.Accessory = (room == selectedRoom) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			if (!Home.IsAdmin ())
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);

			switch ((AddAccessoryTableViewSection)indexPath.Section) {
			case AddAccessoryTableViewSection.Rooms:
				if (!Home.IsAdmin ())
					return;
				selectedRoom = Home.GetAllRooms () [indexPath.Row];

				var sections = NSIndexSet.FromIndex ((nint)(int)AddAccessoryTableViewSection.Rooms);
				tableView.ReloadSections (sections, UITableViewRowAnimation.Automatic);
				break;
			case AddAccessoryTableViewSection.Identify:
				IdentifyAccessory ();
				break;
			case AddAccessoryTableViewSection.Name:
				break;
			default:
				throw new InvalidOperationException ("Unexpected `AddAccessoryTableViewSection` value.");
			}
		}

		public override nint IndentationLevel (UITableView tableView, NSIndexPath indexPath)
		{
			return base.IndentationLevel (tableView, NSIndexPath.FromRowSection (0, indexPath.Section));
		}

		#endregion

		#region HMHomeDelegate Methods

		// All home changes reload the view.

		[Export ("home:didUpdateNameForRoom:")]
		public void DidUpdateNameForRoom (HMHome home, HMRoom room)
		{
			TableView.ReloadData ();
		}

		[Export ("home:didAddRoom:")]
		public void DidAddRoom (HMHome home, HMRoom room)
		{
			TableView.ReloadData ();
		}

		[Export ("home:didRemoveRoom:")]
		public void DidRemoveRoom (HMHome home, HMRoom room)
		{
			// Reset the selected room if ours was deleted.
			if (selectedRoom == room)
				selectedRoom = HomeStore.Home.GetRoomForEntireHome ();
			TableView.ReloadData ();
		}

		[Export ("home:didAddAccessory:")]
		public void DidAddAccessory (HMHome home, HMAccessory accessory)
		{
			// Bridged accessories don't call the original completion handler if their
			// bridges are added to the home. We must respond to `HMHomeDelegate`'s
			// `home:didAddAccessory:` and assign bridged accessories properly.
			if (selectedRoom != null)
				AssignAccessory (home, accessory, selectedRoom);
		}

		[Export ("home:didUnblockAccessory:")]
		public void DidUnblockAccessory (HMHome home, HMAccessory accessory)
		{
			TableView.ReloadData ();
		}

		#endregion

		#region HMAccessoryDelegate Methods

		[Export ("accessoryDidUpdateName:")]
		public void DidUpdateName (HMAccessory accessory)
		{
			ResetNameField ();
		}

		#endregion
	}
}
