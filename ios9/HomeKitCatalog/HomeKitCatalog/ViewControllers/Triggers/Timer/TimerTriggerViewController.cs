using System;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller which facilitates the creation of timer triggers.
	public partial class TimerTriggerViewController : TriggerViewController
	{
		static readonly NSString recurrenceCell = (NSString)"RecurrenceCell";

		static readonly string[] RecurrenceTitles = {
			"Every Hour",
			"Every Day",
			"Every Week",
		};

		[Outlet ("datePicker")]
		UIDatePicker datePicker { get; set; }

		// Sets the stored fireDate to the new value.
		// HomeKit only accepts dates aligned with minute boundaries,
		// so we use NSDateComponents to only get the appropriate pieces of information from that date.
		// Eventually we will end up with a date following this format: "MM/dd/yyyy hh:mm"
		HMTimerTrigger TimerTrigger {
			get {
				return Trigger as HMTimerTrigger;
			}
		}

		TimerTriggerCreator TimerTriggerCreator {
			get {
				return TriggerCreator as TimerTriggerCreator;
			}
		}

		#region ctors

		public TimerTriggerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public TimerTriggerViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;
			TriggerCreator = new TimerTriggerCreator (Trigger, Home);
			datePicker.Date = TimerTriggerCreator.FireDate;
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), recurrenceCell);
		}

		// Reset our saved fire date to the date in the picker.
		[Export ("didChangeDate:")]
		void didChangeDate (UIDatePicker picker)
		{
			TimerTriggerCreator.RawFireDate = picker.Date;
		}

		#region Table View Methods

		// returns:  The number of rows in the Recurrence section; defaults to the super implementation for other sections
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Recurrence:
				return RecurrenceTitles.Length;

			case TriggerTableViewSection.Name:
			case TriggerTableViewSection.Enabled:
			case TriggerTableViewSection.ActionSets:
			case TriggerTableViewSection.DateAndTime:
			case TriggerTableViewSection.Conditions:
			case TriggerTableViewSection.Location:
			case TriggerTableViewSection.Region:
			case TriggerTableViewSection.Characteristics:
				return base.RowsInSection (tableView, section);

			default:
				throw new InvalidOperationException ("Unexpected `TriggerTableViewSection` value.");
			}
		}

		// Generates a recurrence cell. Defaults to the super implementation for other sections
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Recurrence:
				return GetRecurrenceCell (tableView, indexPath);
			case TriggerTableViewSection.Name:
			case TriggerTableViewSection.Enabled:
			case TriggerTableViewSection.ActionSets:
			case TriggerTableViewSection.DateAndTime:
			case TriggerTableViewSection.Conditions:
			case TriggerTableViewSection.Location:
			case TriggerTableViewSection.Region:
			case TriggerTableViewSection.Characteristics:
				return base.GetCell (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `TriggerTableViewSection` value.");
			}
		}

		// Creates a cell that represents a recurrence type.
		UITableViewCell GetRecurrenceCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (recurrenceCell, indexPath);
			var title = RecurrenceTitles [indexPath.Row];
			cell.TextLabel.Text = title;

			// The current preferred recurrence style should have a check mark.
			cell.Accessory = indexPath.Row == TimerTriggerCreator.SelectedRecurrenceIndex
				? UITableViewCellAccessory.Checkmark
				: UITableViewCellAccessory.None;
			return cell;
		}

		// Tell the tableView to automatically size the custom rows, while using the superclass's
		// static sizing for the static cells.
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Recurrence:
				return UITableView.AutomaticDimension;
			case TriggerTableViewSection.Name:
			case TriggerTableViewSection.Enabled:
			case TriggerTableViewSection.ActionSets:
			case TriggerTableViewSection.DateAndTime:
			case TriggerTableViewSection.Conditions:
			case TriggerTableViewSection.Location:
			case TriggerTableViewSection.Region:
			case TriggerTableViewSection.Characteristics:
				return base.GetHeightForRow (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `TriggerTableViewSection` value.");
			}
		}

		// Handles recurrence cell selection. Defaults to the super implementation for other sections
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);

			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Recurrence:
				SelectRecurrenceComponent (tableView, indexPath);
				break;
			case TriggerTableViewSection.Name:
			case TriggerTableViewSection.Enabled:
			case TriggerTableViewSection.ActionSets:
			case TriggerTableViewSection.DateAndTime:
			case TriggerTableViewSection.Conditions:
			case TriggerTableViewSection.Location:
			case TriggerTableViewSection.Region:
			case TriggerTableViewSection.Characteristics:
				base.RowSelected (tableView, indexPath);
				break;
			default:
				throw new InvalidOperationException ("Unexpected `TriggerTableViewSection` value.");
			}
		}

		// Handles selection of a recurrence cell.
		// If the newly selected recurrence component is the previously selected
		// recurrence component, reset the current selected component to `-1` and deselect that row.
		void SelectRecurrenceComponent (UITableView tableView, NSIndexPath indexPath)
		{
			bool isPreviouslySelected = indexPath.Row == TimerTriggerCreator.SelectedRecurrenceIndex;
			TimerTriggerCreator.SelectedRecurrenceIndex = isPreviouslySelected ? -1 : indexPath.Row;
			tableView.ReloadSections (NSIndexSet.FromIndex (indexPath.Section), UITableViewRowAnimation.Automatic);
		}

		protected override TriggerTableViewSection SectionForIndex (int index)
		{
			switch (index) {
			case 0:
				return TriggerTableViewSection.Name;
			case 1:
				return TriggerTableViewSection.Enabled;
			case 2:
				return TriggerTableViewSection.DateAndTime;
			case 3:
				return TriggerTableViewSection.Recurrence;
			case 4:
				return TriggerTableViewSection.ActionSets;
			default:
				return TriggerTableViewSection.None;
			}
		}

		#endregion
	}
}
