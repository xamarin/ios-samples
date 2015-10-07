using System;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Represents a section in the `TimeConditionViewController`.
	enum TimeConditionTableViewSection
	{
		// This section contains the segmented control to
		// choose a time condition type.
		TimeOrSun,
		// This section contains cells to allow the selection
		// of 'before', 'after', or 'at'. 'At' is only available
		// when the exact time is specified.
		BeforeOrAfter,
		// If the condition type is exact time, this section will
		// only have one cell, the date picker cell.
		// If the condition type is relative to a solar event,
		// this section will have two cells, one for 'sunrise' and one for 'sunset.
		Value,
	}

	// Represents the type of time condition.
	// The condition can be an exact time, or relative to a solar event.
	enum TimeConditionType
	{
		Time,
		Sun
	}

	// Represents the type of solar event.
	// This can be sunrise or sunset.
	public enum TimeConditionSunState
	{
		Sunrise,
		Sunset
	}

	// Represents the condition order.
	// Conditions can be before, after, or exactly at a given time.
	public enum TimeConditionOrder
	{
		Before,
		After,
		At
	}

	public partial class TimeConditionViewController : HMCatalogViewController
	{
		static readonly NSString selectionCell = (NSString)"SelectionCell";
		static readonly NSString timePickerCell = (NSString)"TimePickerCell";
		static readonly NSString segmentedTimeCell = (NSString)"SegmentedTimeCell";

		static readonly string[] BeforeOrAfterTitles = {
			"Before",
			"After",
			"At",
		};

		static readonly string[] SunriseSunsetTitles = {
			"Sunrise",
			"Sunset",
		};

		TimeConditionType timeType = TimeConditionType.Time;
		TimeConditionOrder order = TimeConditionOrder.Before;
		TimeConditionSunState sunState = TimeConditionSunState.Sunrise;

		UIDatePicker datePicker;

		public EventTriggerCreator TriggerCreator { get; set; }

		#region ctors

		public TimeConditionViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public TimeConditionViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return Enum.GetNames (typeof(TimeConditionTableViewSection)).Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((TimeConditionTableViewSection)(int)section) {
			case TimeConditionTableViewSection.TimeOrSun:
				return 1;
			case TimeConditionTableViewSection.BeforeOrAfter:
				// If we're choosing an exact time, we add the 'At' row.
				return (timeType == TimeConditionType.Time) ? 3 : 2;
			case TimeConditionTableViewSection.Value:
				// Date picker cell or sunrise/sunset selection cells
				return (timeType == TimeConditionType.Time) ? 1 : 2;
			default:
				throw new InvalidOperationException ("Unexpected `TimeConditionTableViewSection` value.");
			}
		}

		// Switches based on the section to generate a cell.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((TimeConditionTableViewSection)indexPath.Section) {
			case TimeConditionTableViewSection.TimeOrSun:
				return GetSegmentedCell (tableView, indexPath);
			case TimeConditionTableViewSection.BeforeOrAfter:
				return GetSectionCell (tableView, indexPath);
			case TimeConditionTableViewSection.Value:
				switch (timeType) {
				case TimeConditionType.Time:
					return GetDatePickerCell (tableView, indexPath);
				case TimeConditionType.Sun:
					return GetSectionCell (tableView, indexPath);
				default:
					throw new InvalidOperationException ();
				}
			default:
				throw new InvalidOperationException ("Unexpected `TimeConditionTableViewSection` value.");
			}
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			switch ((TimeConditionTableViewSection)(int)section) {
			case TimeConditionTableViewSection.TimeOrSun:
				return "Condition Type";
			case TimeConditionTableViewSection.BeforeOrAfter:
				return null;
			case TimeConditionTableViewSection.Value:
				return timeType == TimeConditionType.Time ? "Time" : "Event";
			default:
				throw new InvalidOperationException ("Unexpected `TimeConditionTableViewSection` value.");
			}
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			switch ((TimeConditionTableViewSection)(int)section) {
			case TimeConditionTableViewSection.TimeOrSun:
				return "Time conditions can relate to specific times or special events, like sunrise and sunset.";
			case TimeConditionTableViewSection.BeforeOrAfter:
			case TimeConditionTableViewSection.Value:
				return null;
			default:
				throw new InvalidOperationException ("Unexpected `TimeConditionTableViewSection` value.");
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected (tableView, indexPath);

			var cell = tableView.CellAt (indexPath);
			if (cell.SelectionStyle == UITableViewCellSelectionStyle.None)
				return;

			tableView.DeselectRow (indexPath, true);

			switch ((TimeConditionTableViewSection)indexPath.Section) {
			case TimeConditionTableViewSection.TimeOrSun:
				timeType = (TimeConditionType)indexPath.Row;
				ReloadDynamicSections ();
				return;
			case TimeConditionTableViewSection.BeforeOrAfter:
				order = (TimeConditionOrder)indexPath.Row;
				tableView.ReloadSections (NSIndexSet.FromIndex (indexPath.Section), UITableViewRowAnimation.Automatic);
				break;
			case TimeConditionTableViewSection.Value:
				if (timeType == TimeConditionType.Sun)
					sunState = (TimeConditionSunState)indexPath.Row;
				tableView.ReloadSections (NSIndexSet.FromIndex (indexPath.Section), UITableViewRowAnimation.Automatic);
				break;
			default:
				throw new InvalidOperationException ("Unexpected `TimeConditionTableViewSection` value.");
			}
		}

		// Generates a selection cell based on the section.
		// Ordering and sun-state sections have selections.
		UITableViewCell GetSectionCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (selectionCell, indexPath);
			switch ((TimeConditionTableViewSection)indexPath.Section) {
			case TimeConditionTableViewSection.BeforeOrAfter:
				cell.TextLabel.Text = BeforeOrAfterTitles [indexPath.Row];
				cell.Accessory = ((int)order == indexPath.Row) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				break;
			case TimeConditionTableViewSection.Value:
				if (timeType == TimeConditionType.Sun) {
					cell.TextLabel.Text = SunriseSunsetTitles [indexPath.Row];
					cell.Accessory = ((int)sunState == indexPath.Row) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				}
				break;
			case TimeConditionTableViewSection.TimeOrSun:
				break;
			default:
				throw new InvalidOperationException ("Unexpected `TimeConditionTableViewSection` value.");
			}
			return cell;
		}

		// Generates a date picker cell and sets the internal date picker when created.
		UITableViewCell GetDatePickerCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (TimePickerCell)tableView.DequeueReusableCell (timePickerCell, indexPath);
			// Save the date picker so we can get the result later.
			datePicker = cell.DatePicker;
			return cell;
		}

		/// Generates a segmented cell and sets its target when created.
		UITableViewCell GetSegmentedCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (SegmentedTimeCell)tableView.DequeueReusableCell (segmentedTimeCell, indexPath);
			cell.SegmentedControl.SelectedSegment = (int)timeType;
			cell.SegmentedControl.ValueChanged -= OnSegmentedControlDidChange;
			cell.SegmentedControl.ValueChanged += OnSegmentedControlDidChange;
			return cell;
		}

		// Creates date components from the date picker's date.
		NSDateComponents dateComponents {
			get {
				if (datePicker == null)
					return null;
				NSCalendarUnit flags = NSCalendarUnit.Hour | NSCalendarUnit.Minute;
				return NSCalendar.CurrentCalendar.Components (flags, datePicker.Date);
			}
		}

		// Updates the time type and reloads dynamic sections.
		void OnSegmentedControlDidChange (object sender, EventArgs e)
		{
			var segmentedControl = (UISegmentedControl)sender;
			timeType = (TimeConditionType)(int)segmentedControl.SelectedSegment;
			ReloadDynamicSections ();
		}
		

		// Reloads the BeforeOrAfter and Value section.
		void ReloadDynamicSections ()
		{
			if (timeType == TimeConditionType.Sun && order == TimeConditionOrder.At)
				order = TimeConditionOrder.Before;
			var reloadIndexSet = NSIndexSet.FromNSRange (new NSRange ((int)TimeConditionTableViewSection.BeforeOrAfter, 2));
			TableView.ReloadSections (reloadIndexSet, UITableViewRowAnimation.Automatic);
		}

		// Generates a predicate based on the stored values, adds the condition to the trigger, then dismisses the view.
		[Export ("saveAndDismiss:")]
		void SaveAndDismiss (UIBarButtonItem sender)
		{
			NSPredicate predicate = null;

			switch (timeType) {
			case TimeConditionType.Time:
				switch (order) {
				case TimeConditionOrder.Before:
					predicate = HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringBeforeDate (dateComponents);
					break;
				case TimeConditionOrder.After:
					predicate = HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringAfterDate (dateComponents);
					break;
				case TimeConditionOrder.At:
					predicate = HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringOnDate (dateComponents);
					break;
				}
				break;

			case TimeConditionType.Sun:
				var significantEventString = (sunState == TimeConditionSunState.Sunrise) ? HMSignificantEvent.Sunrise : HMSignificantEvent.Sunset;
				switch (order) {
				case TimeConditionOrder.Before:
					predicate = HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (significantEventString, null);
					break;
				case TimeConditionOrder.After:
					predicate = HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (significantEventString, null);
					break;
				// Significant events must be specified 'before' or 'after'.
				case TimeConditionOrder.At:
					break;
				}
				break;
			}
			var triggerCreator = TriggerCreator;
			if (predicate != null && triggerCreator != null)
				triggerCreator.AddCondition (predicate);

			DismissViewController (true, null);
		}

		// Cancels the creation of the conditions and exits.
		[Export ("dismiss:")]
		void Dismiss (UIBarButtonItem sender)
		{
			DismissViewController (true, null);
		}
	}
}