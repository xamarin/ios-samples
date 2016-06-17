using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Represents table view sections of the `ActionSetViewController`.
	public enum ActionSetTableViewSection
	{
		Name,
		Actions,
		Accessories
	}

	// A view controller that facilitates creation of Action Sets.
	// It contains a cell for a name, and lists accessories within a home.
	// If there are actions within the action set, it also displays a list of ActionCells displaying those actions.
	// It owns an `ActionSetCreator` and routes events to the creator as appropriate.
	public partial class ActionSetViewController : HMCatalogViewController
	{
		static readonly NSString accessoryCell = (NSString)"AccessoryCell";
		static readonly NSString unreachableAccessoryCell = (NSString)"UnreachableAccessoryCell";
		static readonly NSString actionCell = (NSString)"ActionCell";
		const string showServiceSegue = "Show Services";

		[Outlet ("nameField")]
		public UITextField NameField { get; set; }

		[Outlet ("saveButton")]
		public UIBarButtonItem SaveButton { get; set; }

		ActionSetCreator ActionSetCreator { get; set; }

		List<HMAccessory> displayedAccessories = new List<HMAccessory> ();

		public HMActionSet ActionSet { get; set; }

		#region ctors

		public ActionSetViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ActionSetViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ActionSetCreator = new ActionSetCreator (ActionSet, Home);
			displayedAccessories.Clear ();
			displayedAccessories.AddRange (Home.SortedControlAccessories ());

			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), accessoryCell);
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), unreachableAccessoryCell);
			TableView.RegisterClassForCellReuse (typeof(ActionCell), actionCell);

			TableView.RowHeight = UITableView.AutomaticDimension;

			TableView.EstimatedRowHeight = 44;

			var actionSet = ActionSet;
			if (actionSet != null) {
				NameField.Text = actionSet.Name;
				NameFieldDidChange (NameField);
			}

			NameField.Enabled &= Home.IsAdmin ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TableView.ReloadData ();
			EnableSaveButtonIfNecessary ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			if (ShouldPopViewController ())
				DismissViewController (true, null);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			ResignFirstResponder ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == showServiceSegue) {
				var servicesViewController = (ServicesViewController)segue.IntendedDestinationViewController ();
				servicesViewController.OnlyShowsControlServices = true;
				servicesViewController.CellDelegate = ActionSetCreator;

				var index = TableView.IndexPathForCell ((UITableViewCell)sender).Row;
				servicesViewController.Accessory = displayedAccessories [index];
			}
		}

		#region IBAction Methods

		// Dismisses the view controller.
		[Export ("dismiss")]
		void Dismiss ()
		{
			DismissViewController (true, null);
		}

		// Saves the action set, adds it to the home, and dismisses the view.
		[Export ("saveAndDismiss")]
		void SaveAndDismiss ()
		{
			SaveButton.Enabled = false;

			ActionSetCreator.SaveActionSetWithName (TrimmedName, error => {
				SaveButton.Enabled = true;

				if (error != null)
					DisplayError (error);
				else
					Dismiss ();
			});
		}

		// Prompts an update to the save button enabled state.
		[Export ("nameFieldDidChange:")]
		void NameFieldDidChange (UITextField field)
		{
			EnableSaveButtonIfNecessary ();
		}

		#endregion

		#region Table View Methods

		// We do not allow the creation of action sets in a shared home.
		public override nint NumberOfSections (UITableView tableView)
		{
			return Home.IsAdmin () ? 3 : 2;
		}

		// returns:  In the Actions section: the number of actions this set will contain upon saving.
		// In the Accessories section: The number of accessories in the home.
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((ActionSetTableViewSection)(int)section) {
			case ActionSetTableViewSection.Name:
				return base.RowsInSection (tableView, section);
			case ActionSetTableViewSection.Actions:
				return Math.Max (ActionSetCreator.AllCharacteristics ().Length, 1);
			case ActionSetTableViewSection.Accessories:
				return displayedAccessories.Count;
			default:
				throw new InvalidOperationException ("Unexpected `ActionSetTableViewSection` value.");
			}
		}

		// Required override to allow for a tableView with both static and dynamic content.
		// Basically, since the superclass's indentationLevelForRowAtIndexPath is only
		// expecting 1 row per section, just call the super class's implementation for the first row.
		public override nint IndentationLevel (UITableView tableView, NSIndexPath indexPath)
		{
			return base.IndentationLevel (tableView, NSIndexPath.FromRowSection (0, indexPath.Section));
		}

		// Removes the action associated with the index path.
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				var characteristic = ActionSetCreator.AllCharacteristics () [indexPath.Row];
				ActionSetCreator.RemoveTargetValueForCharacteristic (characteristic, () => {
					if (ActionSetCreator.ContainsActions)
						tableView.DeleteRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
					else
						tableView.ReloadRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
				});
			}
		}

		// returns:  `true` for the Actions section; `false` otherwise.
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return (ActionSetTableViewSection)indexPath.Section == ActionSetTableViewSection.Actions && Home.IsAdmin ();
		}

		// returns:  `UITableViewAutomaticDimension` for dynamic sections, otherwise the superclass's implementation.
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((ActionSetTableViewSection)indexPath.Section) {
			case ActionSetTableViewSection.Name:
				return base.GetHeightForRow (tableView, indexPath);
			case ActionSetTableViewSection.Actions:
			case ActionSetTableViewSection.Accessories:
				return UITableView.AutomaticDimension;
			default:
				throw new InvalidOperationException ("Unexpected `ActionSetTableViewSection` value.");
			}
		}

		// returns:  An action cell for the actions section, an accessory cell for the accessory section, or the superclass's implementation.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((ActionSetTableViewSection)indexPath.Section) {
			case ActionSetTableViewSection.Name:
				return base.GetCell (tableView, indexPath);
			case ActionSetTableViewSection.Actions:
				return ActionSetCreator.ContainsActions
					? GetActionCell (tableView, indexPath)
					: base.GetCell (tableView, indexPath);
			case ActionSetTableViewSection.Accessories:
				return GetAccessoryCell (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `ActionSetTableViewSection` value.");
			}
		}

		#endregion

		#region Helper Methods

		// Enables the save button if there is a valid name and at least one action.
		void EnableSaveButtonIfNecessary ()
		{
			SaveButton.Enabled = Home.IsAdmin () && !string.IsNullOrEmpty (TrimmedName) && ActionSetCreator.ContainsActions;
		}

		// returns:  The contents of the nameField, with whitespace trimmed from the beginning and end.
		string TrimmedName {
			get {
				return NameField.Text.Trim ();
			}
		}

		// returns:  `true` if there are no accessories in the home, we have no set action set, or if our home no longer exists; `false` otherwise
		bool ShouldPopViewController ()
		{
			var h = HomeStore.Home;
			if (h != null && h.Accessories.Length == 0 && ActionSet == null)
				return true;

			return HomeStore.HomeManager.Homes.All (home => HomeStore.Home != home);
		}

		// - returns:  An `ActionCell` instance with the target value for the characteristic at the specified index path.
		UITableViewCell GetActionCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ActionCell)tableView.DequeueReusableCell (actionCell, indexPath);
			var characteristic = ActionSetCreator.AllCharacteristics () [indexPath.Row];

			NSObject target = ActionSetCreator.TargetValueForCharacteristic (characteristic);
			if (target != null)
				cell.SetCharacteristic (characteristic, (NSNumber)target);

			return cell;
		}

		// returns:  An Accessory cell that contains an accessory's name.
		UITableViewCell GetAccessoryCell (UITableView tableView, NSIndexPath indexPath)
		{
			// These cells are static, the identifiers are defined in the Storyboard,
			// but they're not recognized here. In viewDidLoad:, we're registering
			// `UITableViewCell` as the class for "AccessoryCell" and "UnreachableAccessoryCell".
			// We must configure these cells manually, the cells in the Storyboard
			// are just for reference.

			var accessory = displayedAccessories [indexPath.Row];
			var cellIdentifier = accessory.Reachable ? accessoryCell : unreachableAccessoryCell;

			var cell = tableView.DequeueReusableCell (cellIdentifier, indexPath);
			cell.TextLabel.Text = accessory.Name;

			if (accessory.Reachable) {
				cell.TextLabel.TextColor = UIColor.DarkTextColor;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				cell.SelectionStyle = UITableViewCellSelectionStyle.Default;
			} else {
				cell.TextLabel.TextColor = UIColor.LightGray;
				cell.Accessory = UITableViewCellAccessory.None;
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			}

			return cell;
		}

		// Shows the services in the selected accessory.
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt (indexPath);
			if (cell.SelectionStyle == UITableViewCellSelectionStyle.None)
				return;

			if ((ActionSetTableViewSection)indexPath.Section == ActionSetTableViewSection.Accessories)
				PerformSegue (showServiceSegue, cell);
		}

		#endregion

		#region HMHomeDelegate Methods

		// Pops the view controller if our configuration is invalid; reloads the view otherwise.
		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HMHome home, HMAccessory accessory)
		{
			if (ShouldPopViewController ())
				DismissViewController (true, null);
			else
				TableView.ReloadData ();
		}

		// Reloads the table view data.
		[Export ("home:didAddAccessory:")]
		public void DidAddAccessory (HMHome home, HMAccessory accessory)
		{
			TableView.ReloadData ();
		}

		// If our action set was removed, dismiss the view.
		[Export ("home:didRemoveActionSet:")]
		public void DidRemoveActionSet (HMHome home, HMActionSet actionSet)
		{
			if (actionSet == ActionSet)
				DismissViewController (true, null);
		}

		#endregion
	}
}
