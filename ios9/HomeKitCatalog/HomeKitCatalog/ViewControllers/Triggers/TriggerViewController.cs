using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Represents all possible sections in a `TriggerViewController` subclass.
	public enum TriggerTableViewSection
	{
		None,
		// All triggers have these sections.
		Name,
		Enabled,
		ActionSets,
		// Timer triggers only.
		DateAndTime,
		Recurrence,
		// Location and Characteristic triggers only.
		Conditions,
		// Location triggers only.
		Location,
		Region,
		// Characteristic triggers only.
		Characteristics
	}

	// A base class for all trigger view controllers.
	// It manages the name, enabled state, and action set components of the view,
	// as these are shared components.
	[Register ("TriggerViewController")]
	public class TriggerViewController : HMCatalogViewController
	{
		readonly NSString actionSetCell = (NSString)"ActionSetCell";

		[Outlet ("saveButton")]
		public UIBarButtonItem SaveButton { get; set; }

		[Outlet ("nameField")]
		public UITextField NameField { get; set; }

		[Outlet ("enabledSwitch")]
		public UISwitch EnabledSwitch { get; set; }

		protected TriggerCreator TriggerCreator { get; set; }

		// An internal array of all action sets in the home.
		readonly List<HMActionSet> actionSets = new List<HMActionSet> ();

		// An array of all action sets that the user has selected.
		// This will be used to save the trigger when it is finalized.
		readonly List<HMActionSet> selectedActionSets = new List<HMActionSet> ();

		public HMTrigger Trigger { get; set; }

		#region ctors

		public TriggerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public TriggerViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var filteredActionSets = Home.ActionSets.Where (actionSet => actionSet.Actions.Count != 0);

			actionSets.Clear ();
			actionSets.AddRange (filteredActionSets);
			actionSets.SortByTypeAndLocalizedName ();

			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;

			// If we have a trigger, set the saved properties to the current properties
			// of the passed-in trigger.
			var trigger = Trigger;
			if (trigger != null) {
				selectedActionSets.Clear ();
				selectedActionSets.AddRange (trigger.ActionSets);

				NameField.Text = trigger.Name;
				EnabledSwitch.On = trigger.Enabled;
			}

			EnableSaveButtonIfApplicable ();
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), actionSetCell);
		}

		#region IBAction Methods

		// Any time the name field changed, reevaluate whether or not to enable the save button.
		[Export ("nameFieldDidChange:")]
		void NameFieldDidChange (UITextField sender)
		{
			EnableSaveButtonIfApplicable ();
		}

		/// Saves the trigger and dismisses this view controller.
		[Export ("saveAndDismiss")]
		void SaveAndDismiss ()
		{
			SaveButton.Enabled = false;
			var triggerCreator = TriggerCreator;
			if (triggerCreator != null) {
				triggerCreator.SaveTriggerWithName (TrimmedName, selectedActionSets, (trigger, errors) => {
					Trigger = trigger;
					SaveButton.Enabled = true;

					if (errors.Any ()) {
						this.DisplayErrors (errors);
						return;
					}

					EnableTrigger (Trigger, Dismiss);
				});
			}
		}

		[Export ("dismiss")]
		void Dismiss ()
		{
			DismissViewController (true, null);
		}

		#endregion

		// Generates the section for the index.
		// This allows for the subclasses to lay out their content in different sections
		// while still maintaining common code in the `TriggerViewController`.
		protected virtual TriggerTableViewSection SectionForIndex (int index)
		{
			return TriggerTableViewSection.None;
		}

		#region Helper Methods

		// Enable the trigger if necessary.
		void EnableTrigger (HMTrigger trigger, Action completion)
		{
			if (trigger.Enabled == EnabledSwitch.On) {
				completion ();
				return;
			}

			trigger.Enable (EnabledSwitch.On, error => {
				if (error != null)
					DisplayError (error);
				else
					completion ();
			});
		}

		// Enables the save button if:
		// 1. The name field is not empty, and
		// 2. There will be at least one action set in the trigger after saving.
		void EnableSaveButtonIfApplicable ()
		{
			var trigger = Trigger;

			SaveButton.Enabled = !string.IsNullOrWhiteSpace (TrimmedName) &&
				(selectedActionSets.Any () || trigger != null && trigger.ActionSets.Length > 0);
		}

		// returns:  The name from the `nameField`, stripping newline and whitespace characters.
		string TrimmedName {
			get {
				return NameField.Text.Trim ();
			}
		}

		#endregion

		#region Table View Methods

		UITableViewCell GetActionSetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (actionSetCell, indexPath);
			var actionSet = actionSets [indexPath.Row];

			cell.Accessory = selectedActionSets.Contains (actionSet)
				? UITableViewCellAccessory.Checkmark
				: UITableViewCellAccessory.None;

			cell.TextLabel.Text = actionSet.Name;

			return cell;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var triggerSection = SectionForIndex ((int)section);
			return triggerSection == TriggerTableViewSection.ActionSets
				? actionSets.Count
					: base.RowsInSection (tableView, section);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var triggerSection = SectionForIndex (indexPath.Section);
			return triggerSection == TriggerTableViewSection.ActionSets
				? GetActionSetCell (tableView, indexPath)
					: base.GetCell (tableView, indexPath);
		}

		// This is necessary for mixing static and dynamic table view cells.
		// We return a fake index path because otherwise the superclass's implementation (which does not
		// know about the extra cells we're adding) will cause an error.
		public override nint IndentationLevel (UITableView tableView, NSIndexPath indexPath)
		{
			var newIndexPath = NSIndexPath.FromRowSection (0, indexPath.Section);
			return base.IndentationLevel (tableView, newIndexPath);
		}

		// Tell the tableView to automatically size the custom rows, while using the superclass's static sizing for the static cells.
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Name:
			case TriggerTableViewSection.Enabled:
				return base.GetHeightForRow (tableView, indexPath);

			case TriggerTableViewSection.ActionSets:
			case TriggerTableViewSection.DateAndTime:
			case TriggerTableViewSection.Recurrence:
			case TriggerTableViewSection.Conditions:
			case TriggerTableViewSection.Location:
			case TriggerTableViewSection.Region:
			case TriggerTableViewSection.Characteristics:
				return UITableView.AutomaticDimension;

			default:
				throw new InvalidOperationException ("Unexpected `TriggerTableViewSection` value.");
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
			if (SectionForIndex (indexPath.Section) == TriggerTableViewSection.ActionSets)
				SelectActionSet (tableView, indexPath);
		}

		// Manages footer titles for higher-level sections. Superclasses should fall back
		// on this implementation after attempting to handle any special trigger sections.
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Enabled:
				return "This trigger will only activate if it is enabled. You can disable triggers to temporarily stop them from running.";
			case TriggerTableViewSection.ActionSets:
				return "When this trigger is activated, it will set these scenes. You can only select scenes which have at least one action.";
			case TriggerTableViewSection.Name:
			case TriggerTableViewSection.DateAndTime:
			case TriggerTableViewSection.Recurrence:
			case TriggerTableViewSection.Conditions:
			case TriggerTableViewSection.Location:
			case TriggerTableViewSection.Region:
			case TriggerTableViewSection.Characteristics:
				return base.TitleForFooter (tableView, section);
			default:
				throw new InvalidOperationException ("Unexpected `TriggerTableViewSection` value.");
			}
		}

		// Handle selection of an action set cell. If the action set is already part of the selected action sets,
		// then remove it from the selected list. Otherwise, add it to the selected list.
		void SelectActionSet (UITableView tableView, NSIndexPath indexPath)
		{
			var actionSet = actionSets [indexPath.Row];
			var index = selectedActionSets.IndexOf (actionSet);
			if (index >= 0)
				selectedActionSets.RemoveAt (index);
			else
				selectedActionSets.Add (actionSet);

			EnableSaveButtonIfApplicable ();
			tableView.ReloadRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		#endregion

		#region HMHomeDelegate Methods

		// If our trigger has been removed from the home, dismiss the view controller.
		[Export ("home:didRemoveTrigger:")]
		public void DidRemoveTrigger (HMHome home, HMTrigger trigger)
		{
			if (Trigger == trigger)
				DismissViewController (true, null);
		}

		// If our trigger has been updated, reload our data.
		[Export ("home:didUpdateTrigger:")]
		public void DidUpdateTrigger (HMHome home, HMTrigger trigger)
		{
			if (Trigger == trigger)
				TableView.ReloadData ();
		}

		#endregion
	}
}

