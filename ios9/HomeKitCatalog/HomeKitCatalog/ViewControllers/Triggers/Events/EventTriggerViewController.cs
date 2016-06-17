using System;

using Foundation;
using UIKit;

namespace HomeKitCatalog
{
	// A base class for all event-based view controllers.
	// It handles the process of creating and managing trigger conditions.
	public class EventTriggerViewController : TriggerViewController
	{
		protected static readonly NSString AddCell = (NSString)"AddCell";
		protected static readonly NSString ConditionCell = (NSString)"ConditionCell";
		const string ShowTimeConditionSegue = "Show Time Condition";

		EventTriggerCreator EventTriggerCreator {
			get {
				return (EventTriggerCreator)TriggerCreator;
			}
		}

		#region ctros

		public EventTriggerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public EventTriggerViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), AddCell);
			TableView.RegisterClassForCellReuse (typeof(ConditionCell), ConditionCell);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			var vc = segue.IntendedDestinationViewController ();

			var timeVC = vc as TimeConditionViewController;
			if (timeVC != null) {
				timeVC.TriggerCreator = EventTriggerCreator;
				return;
			}

			var characteristicEventVC = vc as CharacteristicSelectionViewController;
			if (characteristicEventVC != null) {
				characteristicEventVC.TriggerCreator = (EventTriggerCreator)TriggerCreator;
				return;
			}
		}

		#region Table View Methods

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			// Add row.
			case TriggerTableViewSection.Conditions:
				return EventTriggerCreator.Conditions.Count + 1;
			default:
				return base.RowsInSection (tableView, section);
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Conditions:
				if (IndexPathIsAdd (indexPath))
					AddCondition ();
				break;
			default:
				base.RowSelected (tableView, indexPath);
				break;
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (IndexPathIsAdd (indexPath))
				return GetAddCell (tableView, indexPath);

			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Conditions:
				return GetConditionCell (tableView, indexPath);
			default:
				return base.GetCell (tableView, indexPath);
			}
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (IndexPathIsAdd (indexPath))
				return false;

			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Conditions:
				return true;
			default:
				return false;
			}
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				var predicate = EventTriggerCreator.Conditions [indexPath.Row];
				EventTriggerCreator.RemoveCondition (predicate);
				tableView.DeleteRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		protected virtual UITableViewCell GetAddCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (AddCell, indexPath);
			string cellText;
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Conditions:
				cellText = "Add Condition…";
				break;
			default:
				cellText = "Add…";
				break;
			}

			cell.TextLabel.Text = cellText;
			cell.TextLabel.TextColor = Colors.EditableBlueColor ();

			return cell;
		}

		// returns:  A 'condition cell', which displays information about the condition.
		UITableViewCell GetConditionCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ConditionCell)tableView.DequeueReusableCell (ConditionCell);
			NSPredicate condition = EventTriggerCreator.Conditions [indexPath.Row];

			var hkCondition = condition.HomeKitConditionType ();
			switch (hkCondition.Type) {
			case HomeKitConditionType.Characteristic:
				cell.SetCharacteristic (hkCondition.CharacteristicData.Item1, (NSNumber)hkCondition.CharacteristicData.Item2);
				break;
			case HomeKitConditionType.ExactTime:
				cell.SetOrder (hkCondition.ExactTimeData.Item1, hkCondition.ExactTimeData.Item2);
				break;
			case HomeKitConditionType.SunTime:
				cell.SetOrder (hkCondition.SunTimeData.Item1, hkCondition.SunTimeData.Item2);
				break;
			case HomeKitConditionType.Unknown:
				cell.SetUnknown ();
				break;
			}

			return cell;
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Conditions:
				return "When a trigger is activated by an event, it checks these conditions. If all of them are true, it will set its scenes.";
			default:
				return base.TitleForFooter (tableView, section);
			}
		}

		#endregion

		/// Presents an alert controller to choose the type of trigger.
		void AddCondition ()
		{
			var alertController = UIAlertController.Create ("Add Condition", null, UIAlertControllerStyle.ActionSheet);

			// Time Condition.
			var timeAction = UIAlertAction.Create ("Time", UIAlertActionStyle.Default, _ => PerformSegue (ShowTimeConditionSegue, this));
			alertController.AddAction (timeAction);

			// Characteristic trigger.
			var eventAction = UIAlertAction.Create ("Characteristic", UIAlertActionStyle.Default, _ => {
				var triggerCreator = TriggerCreator as CharacteristicTriggerCreator;
				if (triggerCreator != null)
					triggerCreator.Mode = CharacteristicTriggerCreatorMode.Condition;
				PerformSegue ("Select Characteristic", this);
			});

			alertController.AddAction (eventAction);

			// Cancel.
			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null);
			alertController.AddAction (cancelAction);

			// Present alert.
			PresentViewController (alertController, true, null);
		}

		protected virtual bool IndexPathIsAdd (NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Conditions:
				return indexPath.Row == EventTriggerCreator.Conditions.Count;
			default:
				return false;
			}
		}
	}
}