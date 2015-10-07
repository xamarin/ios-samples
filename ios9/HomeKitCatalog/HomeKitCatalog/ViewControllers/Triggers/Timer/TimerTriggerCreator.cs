using System;

using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// A `TriggerCreator` subclass which allows for the creation of timer triggers.
	public class TimerTriggerCreator : TriggerCreator
	{
		static readonly NSCalendarUnit[] Components = {
			NSCalendarUnit.Hour,
			NSCalendarUnit.Day,
			NSCalendarUnit.WeekOfYear
		};

		public int SelectedRecurrenceIndex { get ; set; }

		HMTimerTrigger TimerTrigger {
			get {
				return Trigger as HMTimerTrigger;
			}
		}

		public NSDate RawFireDate { get; set; }

		public NSDate FireDate {
			get {
				NSCalendarUnit flags = NSCalendarUnit.Year | NSCalendarUnit.Weekday | NSCalendarUnit.Month | NSCalendarUnit.Day | NSCalendarUnit.Hour | NSCalendarUnit.Minute;
				var dateComponents = NSCalendar.CurrentCalendar.Components (flags, RawFireDate);
				var probableDate = NSCalendar.CurrentCalendar.DateFromComponents (dateComponents);
				return probableDate ?? RawFireDate;
			}
		}

		public TimerTriggerCreator (HMTrigger trigger, HMHome home)
			: base (trigger, home)
		{
			SelectedRecurrenceIndex = -1;
			RawFireDate = new NSDate ();

			var timerTrigger = TimerTrigger;
			if (timerTrigger != null) {
				RawFireDate = timerTrigger.FireDate;
				SelectedRecurrenceIndex = RecurrenceIndexFromDateComponents (timerTrigger.Recurrence);
			}
		}

		protected override HMTrigger NewTrigger ()
		{
			return new HMTimerTrigger (Name, FireDate, NSCalendar.CurrentCalendar.TimeZone, RecurrenceComponents, null);
		}

		protected override void UpdateTrigger ()
		{
			UpdateFireDateIfNecessary ();
			UpdateRecurrenceIfNecessary ();
		}

		#region Helper Methods

		// Creates an NSDateComponent for the selected recurrence type.
		NSDateComponents RecurrenceComponents {
			get {
				if (SelectedRecurrenceIndex == -1)
					return null;

				var recurrenceComponents = new NSDateComponents ();
				NSCalendarUnit unit = Components [SelectedRecurrenceIndex];

				switch (unit) {
				case NSCalendarUnit.WeekOfYear:
					recurrenceComponents.WeekOfYear = 1;
					break;
				case NSCalendarUnit.Hour:
					recurrenceComponents.Hour = 1;
					break;
				case NSCalendarUnit.Day:
					recurrenceComponents.Day = 1;
					break;
				}
				return recurrenceComponents;
			}
		}

		// Maps the possible calendar units associated with recurrence titles, so we can properly
		// set our recurrenceUnit when an index is selected.
		static int RecurrenceIndexFromDateComponents (NSDateComponents components)
		{
			if (components == null)
				return -1;

			var unit = (NSCalendarUnit)0;
			if (components.Day == 1)
				unit = NSCalendarUnit.Day;
			else if (components.WeekOfYear == 1)
				unit = NSCalendarUnit.WeekOfYear;
			else if (components.Hour == 1)
				unit = NSCalendarUnit.Hour;

			return (int)unit == 0 ? -1 : Math.Max (Array.IndexOf (Components, unit), -1);
		}

		// Updates the trigger's fire date, entering and leaving the dispatch group if necessary.
		// If the trigger's fire date is already equal to the passed-in fire date, this method does nothing.
		void UpdateFireDateIfNecessary ()
		{
			var timerTrigger = TimerTrigger;
			if (timerTrigger == null || timerTrigger.FireDate == FireDate)
				return;

			SaveTriggerGroup.Enter ();
			timerTrigger.UpdateFireDate (FireDate, error => {
				if (error != null)
					Errors.Add (error);
				SaveTriggerGroup.Leave ();
			});
		}

		// Updates the trigger's recurrence components, entering and leaving the dispatch group if necessary.
		// If the trigger's components are already equal to the passed-in components, this method does nothing.
		void UpdateRecurrenceIfNecessary ()
		{
			var timerTrigger = TimerTrigger;
			if (timerTrigger == null || timerTrigger.Recurrence == RecurrenceComponents)
				return;

			SaveTriggerGroup.Enter ();
			timerTrigger.UpdateRecurrence (RecurrenceComponents, error => {
				if (error != null)
					Errors.Add (error);
				SaveTriggerGroup.Leave ();
			});
		}

		#endregion
	}
}