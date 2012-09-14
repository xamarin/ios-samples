using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.EventKit;

namespace Calendars.Screens.Home
{
	public class HomeController : DialogViewController
	{
		// our roote element for MonoTouch.Dialog
		protected RootElement calendarListRoot = new RootElement ( "Calendar Store" );
		protected Screens.CalendarList.CalendarListController calendarListScreen;

		protected CreateEventEditViewDelegate eventControllerDelegate;

		public HomeController () : base ( UITableViewStyle.Grouped, null, false)
		{
			// build out our table using MT.D
			Root = calendarListRoot;
			// add our calendar lists items
			Root.Add ( new Section ( "Calendar Lists" ) {
				new StyledStringElement ("Calendar",
					() => { LaunchCalendarListScreen ( EKEntityType.Event ); } ) 
					{ Value = "Events from the Calendar app.", Accessory = UITableViewCellAccessory.DisclosureIndicator },
				new StyledStringElement ("Reminders",
					() => { LaunchCalendarListScreen ( EKEntityType.Reminder ); } ) 
					{ Value = "Events from the Reminder app.", Accessory = UITableViewCellAccessory.DisclosureIndicator }
				} );
			// tasks
			Root.Add ( new Section ( "Tasks") {
				new StyledStringElement ("Add New Event",
					() => { LaunchCreateNewEvent (); } )
					{ Value = "Using the built-in controllers." },
				new StyledStringElement ("Add New Reminder", "Using the built-in controllers.")
			});
		}

		protected void LaunchCalendarListScreen ( EKEntityType calendarStore )
		{
			calendarListScreen = new Calendars.Screens.CalendarList.CalendarListController ( calendarStore );
			NavigationController.PushViewController ( calendarListScreen, true );
		}

		protected void LaunchCreateNewEvent ()
		{
			MonoTouch.EventKitUI.EKEventEditViewController eventController = 
				new MonoTouch.EventKitUI.EKEventEditViewController ();

			eventController.EventStore = App.Current.EventStore;

			eventControllerDelegate = new CreateEventEditViewDelegate ( eventController );

			eventController.EditViewDelegate = eventControllerDelegate;

			PresentViewController ( eventController, true, null );
		}

		protected class CreateEventEditViewDelegate : MonoTouch.EventKitUI.EKEventEditViewDelegate
		{
			protected MonoTouch.EventKitUI.EKEventEditViewController eventController;

			public CreateEventEditViewDelegate (MonoTouch.EventKitUI.EKEventEditViewController eventController)
			{
				this.eventController = eventController;
			}

			public override void Completed (MonoTouch.EventKitUI.EKEventEditViewController controller, EKEventEditViewAction action)
			{
				eventController.DismissViewController (true, null);
			}
		}
	}
}

