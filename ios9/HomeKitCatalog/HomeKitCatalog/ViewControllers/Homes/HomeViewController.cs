using System;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Distinguishes between the three types of cells in the `HomeViewController`.
	public enum HomeCellType
	{
		// Represents an actual object in HomeKit.
		Object,
		// Represents an "Add" row for users to select to create an object in HomeKit.
		Add,
		// The cell is displaying text to show the user that no objects exist in this section.
		None
	}

	/// <summary>
	/// A view controller that displays all elements within a home.
	/// It contains separate sections for Accessories, Rooms, Zones, Action Sets,
	/// Triggers, and Service Groups.
	/// </summary>
	public partial class HomeViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		static readonly NSString AddCell = (NSString)"AddCell";
		static readonly NSString DisabledAddCell = (NSString)"DisabledAddCell";
		static readonly NSString AccessoryCell = (NSString)"AccessoryCell";
		static readonly NSString UnreachableAccessoryCell = (NSString)"UnreachableAccessoryCell";
		static readonly NSString RoomCell = (NSString)"RoomCell";
		static readonly NSString ZoneCell = (NSString)"ZoneCell";
		static readonly NSString UserCell = (NSString)"UserCell";
		static readonly NSString ActionSetCell = (NSString)"ActionSetCell";
		static readonly NSString TriggerCell = (NSString)"TriggerCell";
		static readonly NSString ServiceGroupCell = (NSString)"ServiceGroupCell";

		const string AddTimerTriggerSegue = "Add Timer Trigger";
		const string AddCharacteristicTriggerSegue = "Add Characteristic Trigger";
		const string AddLocationTriggerSegue = "Add Location Trigger";
		const string AddActionSetSegue = "Add Action Set";
		const string AddAccessoriesSegue = "Add Accessories";
		const string ShowRoomSegue = "Show Room";
		const string ShowZoneSegue = "Show Zone";
		const string ShowActionSetSegue = "Show Action Set";
		const string ShowServiceGroupSegue = "Show Service Group";
		const string ShowAccessorySegue = "Show Accessory";
		const string ModifyAccessorySegue = "Modify Accessory";
		const string ShowTimerTriggerSegue = "Show Timer Trigger";
		const string ShowLocationTriggerSegue = "Show Location Trigger";
		const string ShowCharacteristicTriggerSegue = "Show Characteristic Trigger";

		readonly HomeKitObjectCollection ObjectCollection = new HomeKitObjectCollection ();

		[Export ("initWithCoder:")]
		public HomeViewController (NSCoder coder)
			: base (coder)
		{
		}

		public HomeViewController (IntPtr handle)
			: base (handle)
		{
		}

		// Determines the destination of the segue and passes the correct
		// HomeKit object onto the next view controller.
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var cell = sender as UITableViewCell;
			if (cell == null)
				return;

			var indexPath = TableView.IndexPathForCell (cell);
			if (indexPath == null)
				return;

			var destination = segue.IntendedDestinationViewController ();
			var row = indexPath.Row;

			switch (segue.Identifier) {
			case ShowRoomSegue:
				var roomVC = (RoomViewController)destination;
				roomVC.Room = ObjectCollection.Rooms [row];
				break;

			case ShowZoneSegue:
				var zoneViewController = (ZoneViewController)destination;
				zoneViewController.HomeZone = ObjectCollection.Zones [row];
				break;

			case ShowActionSetSegue:
				var actionSetVC = (ActionSetViewController)destination;
				actionSetVC.ActionSet = ObjectCollection.ActionSets [row];
				break;

			case ShowServiceGroupSegue:
				var serviceGroupVC = (ServiceGroupViewController)destination;
				serviceGroupVC.ServiceGroup = ObjectCollection.ServiceGroups [row];
				break;

			case ShowAccessorySegue:
				var detailVC = (ServicesViewController)destination;
				// The services view controller is generic, we need to provide 
				// `showsFavorites` to display the stars next to characteristics.
				detailVC.Accessory = ObjectCollection.Accessories [row];
				detailVC.ShowsFavorites = true;
				detailVC.CellDelegate = new AccessoryUpdateController ();

				break;

			case ModifyAccessorySegue:
				var addAccessoryVC = (ModifyAccessoryViewController)destination;
				addAccessoryVC.Accessory = ObjectCollection.Accessories [row];
				break;

			case ShowTimerTriggerSegue:
				var triggerVC = (TimerTriggerViewController)destination;
				triggerVC.Trigger = ObjectCollection.Triggers [row];
				break;

			case ShowLocationTriggerSegue:
				var locationTriggerVC = (LocationTriggerViewController)destination;
				locationTriggerVC.Trigger = ObjectCollection.Triggers [row];
				break;

			case ShowCharacteristicTriggerSegue:
				var characteristicTriggerVC = (CharacteristicTriggerViewController)destination;
				characteristicTriggerVC.Trigger = ObjectCollection.Triggers [row];
				break;

			default:
				throw new InvalidOperationException (string.Format ("Received unknown segue identifier: {0}", segue.Identifier));
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationItem.Title = Home.Name;
			ReloadTable ();
		}

		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			foreach (var accessory in Home.Accessories)
				accessory.Delegate = this;
		}

		#region Helper Methods

		void ReloadTable ()
		{
			ObjectCollection.ResetWithHome (Home);
			TableView.ReloadData ();
		}

		HomeCellType CellTypeForIndexPath(NSIndexPath indexPath)
		{
			var section = (HomeKitObjectSection)indexPath.Section;
			bool contains = Array.IndexOf (Enum.GetValues (typeof(HomeKitObjectSection)), section) >= 0;
			if (!contains)
				return HomeCellType.None;

			var objectCount = ObjectCollection.ObjectsCountForSection (section);

			// No objects -- this is either an 'Add Row' or a 'None Row'.
			if (objectCount == 0)
				return Home.IsAdmin () ? HomeCellType.Add : HomeCellType.None;

			return indexPath.Row == objectCount ? HomeCellType.Add : HomeCellType.Object;
		}

		void UpdateTriggerAddRow()
		{
			var triggerSection = NSIndexSet.FromIndex ((int)HomeKitObjectSection.Trigger);
			TableView.ReloadSections (triggerSection, UITableViewRowAnimation.Automatic);
		}

		// Reloads the action set section.
		void UpdateActionSetSection ()
		{
			var actionSetSection = NSIndexSet.FromIndex ((int)HomeKitObjectSection.ActionSet);

			TableView.ReloadSections (actionSetSection, UITableViewRowAnimation.Automatic);
			UpdateTriggerAddRow ();
		}

		// returns:  `true` if there are accessories within the home; `false` otherwise.
		bool CanAddActionSet {
			get {
				return ObjectCollection.Accessories.Count > 0;
			}
		}

		// returns:  `true` if there are action sets (with actions) within the home; `false` otherwise.
		bool CanAddTrigger {
			get {
				return ObjectCollection.ActionSets.Any (a => a.Actions.Any ());
			}
		}

		#endregion

		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			return Enum.GetNames (typeof(HomeKitObjectSection)).Length;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			switch ((HomeKitObjectSection)(int)section) {
			case HomeKitObjectSection.Accessory:
				return "Accessories";
			case HomeKitObjectSection.Room:
				return "Rooms";
			case HomeKitObjectSection.Zone:
				return "Zones";
			case HomeKitObjectSection.User:
				return "Users";
			case HomeKitObjectSection.ActionSet:
				return "Scenes";
			case HomeKitObjectSection.Trigger:
				return "Triggers";
			case HomeKitObjectSection.ServiceGroup:
				return "Service Groups";
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}
		}

		static string TitleForAddRowInSection(HomeKitObjectSection section)
		{
			switch (section) {
			case HomeKitObjectSection.Accessory:
				return "Add Accessory…";
			case HomeKitObjectSection.Room:
				return "Add Room…";
			case HomeKitObjectSection.Zone:
				return "Add Zone…";
			case HomeKitObjectSection.User:
				return "Manage Users…";
			case HomeKitObjectSection.ActionSet:
				return "Add Scene…";
			case HomeKitObjectSection.Trigger:
				return "Add Trigger…";
			case HomeKitObjectSection.ServiceGroup:
				return "Add Service Group…";
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}
		}

		static string TitleForNoneRowInSection(HomeKitObjectSection section)
		{
			switch (section) {
			case HomeKitObjectSection.Accessory:
				return "No Accessories…";
			case HomeKitObjectSection.Room:
				return "No Rooms…";
			case HomeKitObjectSection.Zone:
				return "Add Zone…";
			case HomeKitObjectSection.User:
				return "Manage Users…";
			case HomeKitObjectSection.ActionSet:
				return "No Scenes…";
			case HomeKitObjectSection.Trigger:
				return "No Triggers…";
			case HomeKitObjectSection.ServiceGroup:
				return "No Service Groups…";
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			switch ((HomeKitObjectSection)(int)section) {
			case HomeKitObjectSection.Accessory:
			case HomeKitObjectSection.Room:
				return null;
			case HomeKitObjectSection.Zone:
				return "Zones are optional collections of rooms.";
			case HomeKitObjectSection.User:
				return "Users can control the accessories in your home. You can share your home with anybody with an iCloud account.";
			case HomeKitObjectSection.ActionSet:
				return "Scenes (action sets) represent a state of your home. You must have at least one paired accessory to create a scene.";
			case HomeKitObjectSection.Trigger:
				return "Triggers set scenes at specific times, when you get to locations, or when a characteristic is in a specific state. You must have created at least one scene with an action to create a trigger.";
			case HomeKitObjectSection.ServiceGroup:
				return "Service groups organize services in a custom way. For example, add a subset of lights in your living room to control them without controlling all the lights in the living room.";
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var sectionEnum = (HomeKitObjectSection)(int)section;

			// Only "Manage Users" button is in the Users section
			if (sectionEnum == HomeKitObjectSection.User)
				return 1;

			var objectCount = ObjectCollection.ObjectsCountForSection (sectionEnum);
			if (Home.IsAdmin ())
				return objectCount + 1; // For add row.
			else
				return Math.Max (objectCount, 1); // Always show at least one row in the section.
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch (CellTypeForIndexPath (indexPath)) {
			case HomeCellType.Add:
				return GetAddCellForRowAtIndexPath (tableView, indexPath);
			case HomeCellType.Object:
				return HomeKitObjectCellForRowAtIndexPath (tableView, indexPath);
			case HomeCellType.None:
				return GetNoneCellForRowAtIndexPath (tableView, indexPath);
			default:
				throw new InvalidProgramException ();
			}
		}

		// Generates a 'none cell' with a localized title.
		UITableViewCell GetNoneCellForRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (DisabledAddCell, indexPath);
			var section = (HomeKitObjectSection)indexPath.Section;
			cell.TextLabel.Text = TitleForNoneRowInSection (section);
			return cell;
		}

		// Generates an 'add cell' with a localized title.
		//
		// In some cases, the 'add cell' will be 'disabled' because the user is not
		// allowed to perform the action.
		UITableViewCell GetAddCellForRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
		{
			var reuseIdentifier = AddCell;

			var section = (HomeKitObjectSection)indexPath.Section;

			if ((!CanAddActionSet && section == HomeKitObjectSection.ActionSet) ||
				(!CanAddTrigger && section == HomeKitObjectSection.Trigger) || !Home.IsAdmin ())
				reuseIdentifier = DisabledAddCell;

			var cell = tableView.DequeueReusableCell (reuseIdentifier, indexPath);
			cell.TextLabel.Text = TitleForAddRowInSection (section);

			return cell;
		}

		UITableViewCell HomeKitObjectCellForRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
		{
			string name;
			var row = indexPath.Row;

			switch ((HomeKitObjectSection)indexPath.Section) {
			case HomeKitObjectSection.Accessory:
				var accessory = ObjectCollection.Accessories [row];
				name = accessory.Name;
				break;
			case HomeKitObjectSection.Room:
				var room = ObjectCollection.Rooms [row];
				name = Home.GetNameForRoom (room);
				break;
			case HomeKitObjectSection.Zone:
				var zone = ObjectCollection.Zones [row];
				name = zone.Name;
				break;
			case HomeKitObjectSection.User:
				name = string.Empty;
				break;
			case HomeKitObjectSection.ActionSet:
				var actionSet = ObjectCollection.ActionSets [row];
				name = actionSet.Name;
				break;
			case HomeKitObjectSection.Trigger:
				var trigger = ObjectCollection.Triggers [row];
				name = trigger.Name;
				break;
			case HomeKitObjectSection.ServiceGroup:
				var serviceGroup = ObjectCollection.ServiceGroups [row];
				name = serviceGroup.Name;
				break;
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}

			// Grab the appropriate reuse identifier for this index path.
			NSString reuseIdentifier = ReuseIdentifierForIndexPath (indexPath);
			var cell = TableView.DequeueReusableCell (reuseIdentifier, indexPath);
			cell.TextLabel.Text = name;

			return cell;
		}

		NSString ReuseIdentifierForIndexPath(NSIndexPath indexPath)
		{
			switch( (HomeKitObjectSection)indexPath.Section) {
			case HomeKitObjectSection.Accessory:
				var accessory = ObjectCollection.Accessories [indexPath.Row];
				return accessory.Reachable ? AccessoryCell : UnreachableAccessoryCell;
			case HomeKitObjectSection.Room:
				return RoomCell;
			case HomeKitObjectSection.Zone:
				return ZoneCell;
			case HomeKitObjectSection.User:
				return UserCell;
			case HomeKitObjectSection.ActionSet:
				return ActionSetCell;
			case HomeKitObjectSection.Trigger:
				return TriggerCell;
			case HomeKitObjectSection.ServiceGroup:
				return ServiceGroupCell;
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}
		}

		// Allows users to remove HomeKit object rows if they are the admin of the home.
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			if(!Home.IsAdmin())
				return false;

			// Any row that is not an 'add' row can be removed.
			if (CellTypeForIndexPath (indexPath) == HomeCellType.Add)
				return false;

			var section = (HomeKitObjectSection)indexPath.Section;

			// We cannot remove built-in action sets.
			if (section == HomeKitObjectSection.ActionSet) {
				var actionSet = ObjectCollection.ActionSets [indexPath.Row];
				if (actionSet.IsBuiltIn ())
					return false;
			}

			// We cannot remove roomForEntireHome
			if (section == HomeKitObjectSection.Room) {
				if (ObjectCollection.Rooms [indexPath.Row] == Home.GetRoomForEntireHome ())
					return false;
			}

			return true;
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if( editingStyle == UITableViewCellEditingStyle.Delete)
				RemoveHomeKitObjectAtIndexPath (indexPath);
		}

		// Remove the object from the data structure. If it fails put it back.
		void RemoveHomeKitObjectAtIndexPath (NSIndexPath indexPath)
		{
			int row = indexPath.Row;
			var section = (HomeKitObjectSection)indexPath.Section;

			switch (section) {
			case HomeKitObjectSection.Accessory:
				var accessory = ObjectCollection.Accessories.RemoveAtIndex (row);
				TryRemove (accessory);
				break;
			case HomeKitObjectSection.Room:
				var room = ObjectCollection.Rooms.RemoveAtIndex (row);
				TryRemove (room);
				break;
			case HomeKitObjectSection.Zone:
				var zone = ObjectCollection.Zones.RemoveAtIndex (row);
				TryRemove (zone);
				break;
			case HomeKitObjectSection.ActionSet:
				var actionSet = ObjectCollection.ActionSets.RemoveAtIndex (row);
				TryRemove (actionSet);
				break;
			case HomeKitObjectSection.Trigger:
				var trigger = ObjectCollection.Triggers.RemoveAtIndex (row);
				TryRemove (trigger);
				break;
			case HomeKitObjectSection.ServiceGroup:
				var serviceGroup = ObjectCollection.ServiceGroups.RemoveAtIndex (row);
				TryRemove (serviceGroup);
				break;
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}

			TableView.DeleteRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		async void TryRemove (HMAccessory accessory)
		{
			try {
				await Home.RemoveAccessoryAsync (accessory);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
				Add (accessory);
			}
		}

		void Add (HMAccessory accessory)
		{
			InsertRowAt (ObjectCollection.Add (accessory));
		}

		async void TryRemove (HMRoom room)
		{
			try {
				await Home.RemoveRoomAsync (room);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
				Add (room);
			}
		}

		void Add (HMRoom room)
		{
			InsertRowAt (ObjectCollection.Add (room));
		}

		async void TryRemove (HMZone zone)
		{
			try {
				await Home.RemoveZoneAsync (zone);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
				Add (zone);
			}
		}

		void Add (HMZone zone)
		{
			InsertRowAt (ObjectCollection.Add (zone));
		}

		async void TryRemove (HMActionSet actionSet)
		{
			try {
				await Home.RemoveActionSetAsync (actionSet);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
				Add (actionSet);
			}
		}

		void Add (HMActionSet actionSet)
		{
			InsertRowAt (ObjectCollection.Add (actionSet));
		}

		async void TryRemove (HMTrigger trigger)
		{
			try {
				await Home.RemoveTriggerAsync (trigger);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
				Add (trigger);
			}
		}

		void Add (HMTrigger trigger)
		{
			InsertRowAt (ObjectCollection.Add (trigger));
		}

		async void TryRemove (HMServiceGroup serviceGroup)
		{
			try {
				await Home.RemoveServiceGroupAsync (serviceGroup);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
				Add (serviceGroup);
			}
		}

		void Add (HMServiceGroup serviceGroup)
		{
			InsertRowAt (ObjectCollection.Add (serviceGroup));
		}

		void InsertRowAt (NSIndexPath indexPath)
		{
			TableView.InsertRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		void DeleteRowAt (NSIndexPath indexPath)
		{
			TableView.DeleteRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		void ReloadRowAt (NSIndexPath indexPath)
		{
			TableView.ReloadRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		// Handles cell selection based on the cell type.
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
			var cell = tableView.CellAt (indexPath);

			if (cell.SelectionStyle == UITableViewCellSelectionStyle.None)
				return;

			var section = (HomeKitObjectSection)indexPath.Section;

			if (CellTypeForIndexPath (indexPath) == HomeCellType.Add) {
				switch (section) {
				case HomeKitObjectSection.Accessory:
					BrowseForAccessories ();
					break;
				case HomeKitObjectSection.Room:
					AddNewRoom ();
					break;
				case HomeKitObjectSection.Zone:
					AddNewZone ();
					break;
				case HomeKitObjectSection.User:
					ManageUsers ();
					break;
				case HomeKitObjectSection.ActionSet:
					AddNewActionSet ();
					break;
				case HomeKitObjectSection.Trigger:
					AddNewTrigger ();
					break;
				case HomeKitObjectSection.ServiceGroup:
					AddNewServiceGroup ();
					break;
				default:
					throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
				}
			} else if (section == HomeKitObjectSection.ActionSet) {
				var selectedActionSet = ObjectCollection.ActionSets [indexPath.Row];
				ExecuteActionSet (selectedActionSet);
			}
		}

		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt (indexPath);

			if ((HomeKitObjectSection)indexPath.Section == HomeKitObjectSection.Trigger) {
				var trigger = ObjectCollection.Triggers [indexPath.Row];

				var timerTrigger = trigger as HMTimerTrigger;
				if (timerTrigger != null) {
					PerformSegue (ShowTimerTriggerSegue, cell);
					return;
				}

				var eventTrigger = trigger as HMEventTrigger;
				if (eventTrigger != null) {
					if (eventTrigger.IsLocationEvent ())
						PerformSegue (ShowLocationTriggerSegue, cell);
					else
						PerformSegue (ShowCharacteristicTriggerSegue, cell);
				}
			}
		}

		#region Action Methods

		// Presents an alert controller to allow the user to choose a trigger type.
		void AddNewTrigger ()
		{
			var alertController = UIAlertController.Create ("Add Trigger", null, UIAlertControllerStyle.ActionSheet);

			// Timer trigger
			var timeAction = UIAlertAction.Create ("Time", UIAlertActionStyle.Default, _ => PerformSegue (AddTimerTriggerSegue, this));
			alertController.AddAction (timeAction);

			// Characteristic trigger
			var eventAction = UIAlertAction.Create ("Characteristic", UIAlertActionStyle.Default, _ => PerformSegue (AddCharacteristicTriggerSegue, this));
			alertController.AddAction (eventAction);

			// Location trigger
			var locationAction = UIAlertAction.Create ("Location", UIAlertActionStyle.Default, _ => PerformSegue (AddLocationTriggerSegue, this));
			alertController.AddAction (locationAction);

			// Cancel
			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null);
			alertController.AddAction (cancelAction);

			// Present alert
			PresentViewController (alertController, true, null);
		}

		#endregion

		void AddNewActionSet ()
		{
			PerformSegue (AddActionSetSegue, this);
		}

		void BrowseForAccessories ()
		{
			PerformSegue (AddAccessoriesSegue, this);
		}

		void AddNewRoom ()
		{
			this.PresentAddAlertWithAttributeType ("Room", "Living Room", null, AddRoomWithName);
		}

		void AddNewZone ()
		{
			this.PresentAddAlertWithAttributeType ("Zone", "Upstairs", null, AddZoneWithName);
		}

		void AddNewServiceGroup ()
		{
			this.PresentAddAlertWithAttributeType ("Service Group", "Group", null, AddServiceGroupWithName);
		}

		#region HomeKit Object Creation and Deletion

		async void AddRoomWithName (string roomName)
		{
			try {
				HMRoom room = await Home.AddRoomAsync (roomName);
				Add(room);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
			}
		}

		async void AddServiceGroupWithName (string groupName)
		{
			try {
				var serviceGroup = await Home.AddServiceGroupAsync (groupName);
				Add (serviceGroup);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
			}
		}

		async void AddZoneWithName (string zoneName)
		{
			try {
				var zone = await Home.AddZoneAsync (zoneName);
				Add (zone);
			} catch (NSErrorException ex) {
				DisplayError (ex.Error);
			}
		}

		void ManageUsers ()
		{
			Home.ManageUsers (error => {
				if (error != null)
					DisplayError (error);
			});
		}

		void ExecuteActionSet (HMActionSet actionSet)
		{
			if (actionSet.Actions.Count == 0) {
				DisplayMessage ("Empty Scene",
					"This scene is empty. To set this scene, first add some actions to it.");
				return;
			}

			Home.ExecuteActionSet (actionSet, error => {
				if (error != null)
					DisplayError (error);
			});
		}

		#endregion

		#region HMHomeDelegate Methods

		[Export ("homeDidUpdateName:")]
		public void DidUpdateNameForHome (HMHome home)
		{
			NavigationItem.Title = home.Name;
			ReloadTable ();
		}

		[Export ("home:didAddAccessory:")]
		public void DidAddAccessory (HomeKit.HMHome home, HomeKit.HMAccessory accessory)
		{
			Add (accessory);
			accessory.Delegate = this;
		}

		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HomeKit.HMHome home, HomeKit.HMAccessory accessory)
		{
			DeleteRowAt (ObjectCollection.Remove (accessory));
		}

		#endregion

		#region Triggers

		[Export ("home:didAddTrigger:")]
		public void DidAddTrigger (HomeKit.HMHome home, HomeKit.HMTrigger trigger)
		{
			Add(trigger);
		}

		[Export ("home:didRemoveTrigger:")]
		public void DidRemoveTrigger (HomeKit.HMHome home, HomeKit.HMTrigger trigger)
		{
			DeleteRowAt(ObjectCollection.Remove(trigger));
		}

		[Export ("home:didUpdateNameForTrigger:")]
		public void DidUpdateNameForTrigger (HMHome home, HMTrigger trigger)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (trigger));
		}

		#endregion

		#region Service Groups

		[Export ("home:didAddServiceGroup:")]
		public void DidAddServiceGroup (HMHome home, HMServiceGroup group)
		{
			Add(group);
		}

		[Export ("home:didRemoveServiceGroup:")]
		public void DidRemoveServiceGroup (HMHome home, HMServiceGroup group)
		{
			DeleteRowAt (ObjectCollection.Remove (group));
		}

		[Export ("home:didUpdateNameForServiceGroup:")]
		public void DidUpdateNameForServiceGroup (HMHome home, HMServiceGroup group)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (group));
		}

		#endregion

		#region Action Sets

		[Export ("home:didAddActionSet:")]
		public void DidAddActionSet (HMHome home, HMActionSet actionSet)
		{
			Add (actionSet);
		}

		[Export ("home:didRemoveActionSet:")]
		public void DidRemoveActionSet (HMHome home, HMActionSet actionSet)
		{
			DeleteRowAt (ObjectCollection.Remove (actionSet));
		}

		[Export ("home:didUpdateNameForActionSet:")]
		public void DidUpdateNameForActionSet (HMHome home, HMActionSet actionSet)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (actionSet));
		}

		#endregion

		#region Zones

		[Export ("home:didAddZone:")]
		public void DidAddZone (HMHome home, HMZone zone)
		{
			Add (zone);
		}

		[Export ("home:didRemoveZone:")]
		public void DidRemoveZone (HMHome home, HMZone zone)
		{
			DeleteRowAt (ObjectCollection.Remove (zone));
		}

		[Export ("home:didUpdateNameForZone:")]
		public void DidUpdateNameForZone (HMHome home, HMZone zone)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (zone));
		}

		#endregion

		#region Rooms

		[Export ("home:didAddRoom:")]
		public void DidAddRoom (HMHome home, HMRoom room)
		{
			Add (room);
		}

		[Export ("home:didRemoveRoom:")]
		public void DidRemoveRoom (HMHome home, HMRoom room)
		{
			DeleteRowAt (ObjectCollection.Remove (room));
		}

		[Export ("home:didUpdateNameForRoom:")]
		public void DidUpdateNameForRoom (HMHome home, HMRoom room)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (room));
		}

		#endregion

		#region Accessories

		[Export ("accessoryDidUpdateReachability:")]
		public void DidUpdateReachability (HMAccessory accessory)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (accessory));
		}

		[Export ("accessoryDidUpdateName:")]
		public void DidUpdateName (HMAccessory accessory)
		{
			ReloadRowAt (ObjectCollection.IndexPathOfObject (accessory));
		}
		#endregion

		#endregion
	}
}
