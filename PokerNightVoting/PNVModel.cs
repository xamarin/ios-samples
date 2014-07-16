using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Foundation;
using EventKit;
using CoreFoundation;

namespace PokerNightVoting
{
	public class PNVModel : NSObject
	{
		public EKEventStore EventStore { get; set; }

		public EKCalendar SelectedCalendar { get; set; }

		// updated by fetchPokerEvents: to store the matching events.
		List<EKEvent> events = new List<EKEvent> ();
		public List<DateTime> EventDates = new List<DateTime> ();
		public Dictionary<DateTime,List<EKEvent>> eventDateToEventsDictionary = new Dictionary<DateTime,List<EKEvent>> ();
		public NSString PNVModelChangedNotification = new NSString ("PNVModelChangedNotification");

		public PNVModel ()
		{
			EventStore = new EKEventStore ();

			EventStore.RequestAccess(EKEntityType.Event, delegate (bool arg1, NSError arg2) {
				if (arg2 != null) {
					Console.WriteLine (arg2.ToString ());
				}
			});

			SelectedCalendar = EventStore.DefaultCalendarForNewEvents;
		}

		// if you call AtartBroadcastingModelChangedNotifications you should call
		//  stopBroadcastingModelChangedNotifications before releaseing this object.
		public void StartBoradcastingModelChangedNotificaitons ()
		{
			// We want to listen to the EKEventStoreChangedNotification on the EKEventStore,
			// so that we update our list of events if anything changes in the EKEventStore.

			NSNotificationCenter.DefaultCenter.AddObserver (EKEventStore.ChangedNotification, delegate {
				FetchPokerEvents ();
			}, EventStore);
		}
		
		public void StopBroadcastingModelChangedNotifications ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}

		public List<EKCalendar> Calendars ()
		{
			var allEventCalendars = EventStore.GetCalendars (EKEntityType.Event);
			var filteredCalendars = new List<EKCalendar> ();

			foreach (var calendar in allEventCalendars) {
				if (calendar.AllowsContentModifications)
					filteredCalendars.Add (calendar);
			}

			return filteredCalendars;
		}

		public List<string> CalendarTitles ()
		{
			List<string> calendarTitles = new List<string> ();

			foreach (var calendar in Calendars ())
				calendarTitles.Add (calendar.Title);

			return calendarTitles;
		}

		public EKCalendar CalendarWithTitle (string title)
		{
			foreach (var calendar in Calendars ()) {
				if (calendar.Title == title)
					return calendar;
			}

			return null;
		}

		// updates data structures to only unclude events with the default title in our calendar.
		public void FetchPokerEvents ()
		{
			ThreadPool.QueueUserWorkItem ((v) =>
			{
				// create NSDates to represent arbitrary fetch date range.
				//approximately 2 days ago:
				var yesterday = DateTime.Now - TimeSpan.FromDays (1);
				var twoMonthsInFuture = DateTime.Now + TimeSpan.FromDays (60);

				var predicate = EventStore.PredicateForEvents (yesterday, twoMonthsInFuture, null);
				var allEvents = EventStore.EventsMatching (predicate);

				if (allEvents == null)
					return;

				var result = allEvents.Where (evt => evt.Title == DefaultEventTitle);
			
				// update our internal data structures
				UpdateDataStructuresWithMatchingEvents (result);

				// notify the UI (on the main thread) that our model has changed
				BeginInvokeOnMainThread (delegate {
					NSNotificationCenter.DefaultCenter.PostNotificationName (PNVModelChangedNotification, this); 
				});
			});
		}

		public void UpdateDataStructuresWithMatchingEvents (IEnumerable<EKEvent> evts)
		{
			events = evts.ToList ();

			// Create a list of event start dates
			// Create a dictionary mapping from event start date to events with that start date
			var eventDates = new List<DateTime> ();
			var eventDictionary = new Dictionary<DateTime,List<EKEvent>> ();

			foreach (var e in events) {
				var dt = (DateTime) e.StartDate;
				var local = TimeZone.CurrentTimeZone.ToLocalTime (dt);
				var ymd = new DateTime (local.Year, local.Month, local.Day);
				List<EKEvent> list;

				// Get the array of events on a given start date from the dictionary

				// If the dictionary doesn't already have an array for this start date
				// then create one and also add the date to our array of dates
				if (!eventDictionary.TryGetValue (ymd, out list)) {
					list = new List<EKEvent> ();
					eventDates.Add (ymd);
					eventDictionary [ymd] = list;
				}

				// if the date exists in the dictionary, continue to add events to the list corresponding to this date
				list.Add (e);
			}

			this.EventDates = eventDates;
			this.eventDateToEventsDictionary = eventDictionary;
		}

		public void AddEventWithStartTime (NSDate startDate)
		{
			// Create a new EKEvent and then set the properties on it.
			var e = EKEvent.FromStore (EventStore);
			e.Title = DefaultEventTitle;
			e.TimeZone = NSTimeZone.LocalTimeZone;
			e.StartDate = startDate;
			e.EndDate = startDate.AddSeconds (60 * 60); // 1 hour
			e.Calendar = SelectedCalendar;

			// Save our new EKEvent
			NSError err;
			bool success = EventStore.SaveEvent (e, EKSpan.FutureEvents, true, out err);

			if (!success) 
				Console.WriteLine ("There was an error saving a new event: " + err);
		}

		public int EventNumberOfVotes (EKEvent evt)
		{
			if (evt.Notes == null || evt.Notes == "") 
				return 0;
			return int.Parse (evt.Notes);
		}
		
		public void EventSetNumberOfVotes (EKEvent evt, int newVote)
		{
			// Since vote count is stored in notes, only allow voting if the notes field is empty or only contains the vote count,
			// so that we don't accidentally delete important data in the notes field.
			if (evt.Notes == null || evt.Notes == "" || newVote == EventNumberOfVotes (evt) + 1) 
				evt.Notes = newVote.ToString ();
		}
		
		// Increases the vote count
		public void IncreaseVoteOnEvent (EKEvent e)
		{
			// We will not overwrite the notes field if it cannot be parsed as an int.
			if (AreNotesInteger (e)) { 
				int numVotes = EventNumberOfVotes (e);
				EventSetNumberOfVotes (e, numVotes + 1);
			}
			
			// Save the event since we modified the notes field.
			NSError err;
			bool success = EventStore.SaveEvent (e, EKSpan.ThisEvent, true, out err);
			
			if (!success) 
				Console.WriteLine ("There was an error updating the vote count on an event: " + err);
		}

		public bool AreNotesInteger (EKEvent e)
		{
			try {
				if (e.Notes != null) 
					int.Parse (e.Notes);
				return true;

			} catch (System.FormatException) {
				return false;
			}
		}

		public const string DefaultEventTitle = "Poker";
	}
}

