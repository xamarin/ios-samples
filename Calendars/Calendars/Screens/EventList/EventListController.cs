using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;
using MonoTouch.Dialog;
using Foundation;
using EventKit;

namespace Calendars.Screens.EventList
{
	public class EventListController : DialogViewController
	{
		// our roote element for MonoTouch.Dialog
		protected RootElement itemListRoot = new RootElement ( "Calendar/Reminder Items" );

		protected EKCalendarItem[] events;
		protected EKEntityType eventType;

		public EventListController ( EKCalendarItem[] events, EKEntityType eventType )
			: base ( UITableViewStyle.Plain, null, true)
		{
			this.events = events;
			this.eventType = eventType;

			Section section;
			if (events == null) {
				section = new Section () { new StringElement ("No calendar events") };
			} else {
				section = new Section () { 
					from items in this.events
						select ( Element ) new StringElement ( items.Title )
				};
			} 
			itemListRoot.Add (section);
			// set our element root
			this.InvokeOnMainThread ( () => { this.Root = itemListRoot; } ); 
		}
	}
}

