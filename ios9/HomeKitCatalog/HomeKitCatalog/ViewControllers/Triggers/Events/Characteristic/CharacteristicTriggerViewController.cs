using System;
using System.Collections.Generic;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller which facilitates the creation of characteristic triggers.
	public partial class CharacteristicTriggerViewController : EventTriggerViewController
	{
		const string SelectCharacteristicSegue = "Select Characteristic";

		CharacteristicTriggerCreator CharacteristicTriggerCreator {
			get {
				return (CharacteristicTriggerCreator)TriggerCreator;
			}
		}

		HMEventTrigger EventTrigger {
			get {
				return Trigger as HMEventTrigger;
			}
		}

		// An internal array of `HMCharacteristicEvent`s to save into the trigger.
		readonly List<HMCharacteristicEvent> events = new List<HMCharacteristicEvent> ();

		#region ctors

		public CharacteristicTriggerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CharacteristicTriggerViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TriggerCreator = new CharacteristicTriggerCreator (EventTrigger, Home);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ReloadData ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == SelectCharacteristicSegue) {

				var destinationVC = segue.IntendedDestinationViewController () as CharacteristicSelectionViewController;
				if (destinationVC != null) {
					destinationVC.EventTrigger = EventTrigger;
					destinationVC.TriggerCreator = CharacteristicTriggerCreator;
				}
			}
		}

		#region Table View Methods

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Characteristics:
				// Plus one for the add row.
				return events.Count + 1;
			default:
				return base.RowsInSection (tableView, section);
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (IndexPathIsAdd (indexPath))
				return GetAddCell (tableView, indexPath);

			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Characteristics:
				return GetConditionCell (tableView, indexPath);
			default:
				return base.GetCell (tableView, indexPath);
			}
		}

		// Returns  A 'condition cell' with the event at the specified index path.
		UITableViewCell GetConditionCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ConditionCell)tableView.DequeueReusableCell (ConditionCell, indexPath);
			var e = events [indexPath.Row];
			cell.SetCharacteristic (e.Characteristic, (NSNumber)e.TriggerValue);
			return cell;
		}

		// returns:  An 'add cell' with text. Defaults to base implementation.
		protected override UITableViewCell GetAddCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Characteristics:
				var cell = tableView.DequeueReusableCell (AddCell, indexPath);
				cell.TextLabel.Text = "Add Characteristic…";
				cell.TextLabel.TextColor = Colors.EditableBlueColor ();
				return cell;
			default:
				return base.GetAddCell (tableView, indexPath);
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Characteristics:
				if (IndexPathIsAdd (indexPath)) {
					AddEvent ();
					return;
				}
				var cell = tableView.CellAt (indexPath);
				PerformSegue (SelectCharacteristicSegue, cell);
				break;
			default:
				base.RowSelected (tableView, indexPath);
				break;
			}
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (IndexPathIsAdd (indexPath))
				return false;

			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Characteristics:
				return true;
			default:
				return base.CanEditRow (tableView, indexPath);
			}
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				switch (SectionForIndex (indexPath.Section)) {
				case TriggerTableViewSection.Characteristics:
					CharacteristicTriggerCreator.RemoveEvent (events [indexPath.Row]);
					events.Clear ();
					events.AddRange (CharacteristicTriggerCreator.Events ());
					tableView.DeleteRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
					break;
				default:
					base.CommitEditingStyle (tableView, editingStyle, indexPath);
					break;
				}
			}
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Characteristics:
				return "This trigger will activate when any of these characteristics change to their value. For example, 'run when the garage door is opened'.";
			default:
				return base.TitleForFooter (tableView, section);
			}
		}

		#endregion

		#region Helper Methods

		// Resets the internal events array from the trigger creator.
		void ReloadData ()
		{
			events.Clear ();
			events.AddRange (CharacteristicTriggerCreator.Events ());
			TableView.ReloadData ();
		}

		// Performs a segue to the `CharacteristicSelectionViewController`.
		void AddEvent ()
		{
			CharacteristicTriggerCreator.Mode = CharacteristicTriggerCreatorMode.Event;
			PerformSegue (SelectCharacteristicSegue, null);
		}

		protected override bool IndexPathIsAdd (NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Characteristics:
				return indexPath.Row == events.Count;
			default:
				return base.IndexPathIsAdd (indexPath);
			}
		}

		#endregion

		protected override TriggerTableViewSection SectionForIndex (int index)
		{
			switch (index) {
			case 0:
				return TriggerTableViewSection.Name;
			case 1:
				return TriggerTableViewSection.Enabled;
			case 2:
				return TriggerTableViewSection.Characteristics;
			case 3:
				return TriggerTableViewSection.Conditions;
			case 4:
				return TriggerTableViewSection.ActionSets;
			default:
				return TriggerTableViewSection.None;
			}
		}
	}
}
