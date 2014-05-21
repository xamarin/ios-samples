using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;
using MonoTouch.Dialog;
using Foundation;
using EventKit;

namespace Calendars.Screens.CalendarList
{
	public class CalendarListController : DialogViewController
	{
		// our roote element for MonoTouch.Dialog
		protected RootElement calendarListRoot = new RootElement ( "Calendars" );

		// list of calendars
		protected EKCalendar[] calendars;
		// the type of calendar: Event or Reminder
		protected EKEntityType entityType;

		public CalendarListController ( EKEntityType storeType ) : base ( UITableViewStyle.Plain, null, true)
		{
			entityType = storeType;
			// request access will popup a dialog to the user asking them if they 
			// want to grant calendar access to the application. as such, this method 
			// is asynchronous and you need to pass a completion handler that will get 
			// called once the user has made a decision
			App.Current.EventStore.RequestAccess ( entityType, (bool granted, NSError e) => { PopulateCalendarList ( granted, e ); } ); 
		}

		/// <summary>
		/// called as the completion handler to requesting access to the calendar
		/// </summary>
		protected void PopulateCalendarList (bool grantedAccess, NSError e)
		{
			// if it err'd show it to the user
			if ( e != null ) {
				Console.WriteLine ( "Err: " + e.ToString () );
				new UIAlertView ( "Error", e.ToString(), null, "ok", null ).Show();
				return;
			}

			// if the user granted access to the calendar data
			if (grantedAccess) {
				// get calendars of the particular type (either events or reminders)
				calendars = App.Current.EventStore.GetCalendars ( entityType );

				// build out an MT.D list of all the calendars, we show the calendar title
				// as well as the source (where the calendar is pulled from, like iCloud, local
				// exchange, etc.)
				calendarListRoot.Add (
					new Section ( ) { 
						from elements in calendars
						select ( Element ) new StringElement ( elements.Title, elements.Source.Title )
					}
				);

				this.InvokeOnMainThread ( () => { this.Root = calendarListRoot; } ); 
			}
			// if the user didn't grant access, show an alert
			else {
				Console.WriteLine ( "Access denied by user. " );
				InvokeOnMainThread ( () => { 
					new UIAlertView ( "No Access", "Access to calendar not granted", null, "ok", null).Show ();
				});
			}
		}

	}
}

