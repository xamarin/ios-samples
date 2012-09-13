using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.EventKit;

namespace Calendars.Screens.CalendarList
{
	public class CalendarListController : DialogViewController
	{
		protected RootElement calendarListRoot = new RootElement ( "Calendars" );
		protected EKEventStore eventStore;
		protected EKCalendar[] calendars;

		public CalendarListController ( ) : base ( UITableViewStyle.Plain, null, false)
		{
			eventStore = new EKEventStore ();
			eventStore.RequestAccess ( EKEntityType.Event, (bool granted, NSError e) => { PopulateCalendarList ( granted, e ); } );
		}

		protected void PopulateCalendarList (bool grantedAccess, NSError e)
		{
			if (grantedAccess) {
				calendars = eventStore.GetCalendars (EKEntityType.Event);

				calendarListRoot.Add (
					new Section ( ) { 
						from elements in calendars
						select ( Element ) new StringElement ( elements.Title )
					}
				);

				this.InvokeOnMainThread ( () => { this.Root = calendarListRoot; } ); 
			}
		}

	}
}

