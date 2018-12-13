using Calendars.Delegates;
using EventKit;
using Foundation;
using System;
using UIKit;

namespace Calendars
{
    public partial class MainViewController : UITableViewController
    {
        public static EKEventStore EventStore { get; } = new EKEventStore();

        public MainViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.RequestAccess();
        }

        private void RequestAccess()
        {
            EventStore.RequestAccess(EKEntityType.Event, AccessCompletionHandler);
            EventStore.RequestAccess(EKEntityType.Reminder, AccessCompletionHandler);

            void AccessCompletionHandler(bool granted, NSError error)
            {
                if (!granted)
                {
                    base.InvokeOnMainThread(() =>
                    {
                        var alert = UIAlertController.Create("Access Denied", "User Denied Access to Calendars/Reminders", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        base.PresentViewController(alert, true, null);
                    });
                }
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if(segue.Identifier == "showCalendarsSegue" && 
               segue.DestinationViewController is CalendarsViewController calendarsViewController && 
               sender is NSNumber number)
            {
                calendarsViewController.EntityType = (EKEntityType)number.UInt64Value;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selectedCell = tableView.CellAt(indexPath);
            if (selectedCell == this.newEventCell)
            {
                this.LaunchCreateNewEvent();
            }
            else if (selectedCell == this.modifyEventCell)
            {
                this.LaunchModifyEvent();
            }
            else if (selectedCell == this.saveEventsCell)
            {
                this.SaveAndRetrieveEvent();
            }
            else if (selectedCell == this.createReminderCell)
            {
                this.CreateReminder();
            }
            else if (selectedCell == this.calendarsCell)
            {
                base.PerformSegue("showCalendarsSegue", NSNumber.FromUInt64((ulong)EKEntityType.Event));
            }
            else if (selectedCell == this.remindersCell)
            {
                base.PerformSegue("showCalendarsSegue", NSNumber.FromUInt64((ulong)EKEntityType.Reminder));
            }

            tableView.DeselectRow(indexPath, false);
        }

        #region Events

        /// <summary>
        /// Launchs the create new event controller.
        /// </summary>
        private void LaunchCreateNewEvent()
        {
            // create a new EKEventEditViewController. This controller is built in an allows
            // the user to create a new, or edit an existing event.
            var eventController = new EventKitUI.EKEventEditViewController
            {
                // set the controller's event store - it needs to know where/how to save the event
                EventStore = EventStore
            };

            // wire up a delegate to handle events from the controller
            eventController.EditViewDelegate = new CreateEventEditViewDelegate(eventController);

            // show the event controller
            base.PresentViewController(eventController, true, null);
        }

        /// <summary>
        /// Launchs the create new event controller.
        /// </summary>
        private void LaunchModifyEvent()
        {
            // first we need to create an event it so we have one that we know exists
            // in a real world scenario, we'd likely either a) be modifying an event that
            // we found via a query, or 2) we'd do like this, in which we'd automatically
            // populate the event data, like for say a dr. appt. reminder, or something
            var newEvent = EKEvent.FromStore(EventStore);
            // set the alarm for 10 minutes from now
            newEvent.AddAlarm(EKAlarm.FromDate((NSDate)DateTime.Now.AddMinutes(10)));
            // make the event start 20 minutes from now and last 30 minutes
            newEvent.StartDate = (NSDate)DateTime.Now.AddMinutes(20);
            newEvent.EndDate = (NSDate)DateTime.Now.AddMinutes(50);
            newEvent.Title = "Get outside and do some exercise!";
            newEvent.Notes = "This is your motivational event to go and do 30 minutes of exercise. Super important. Do this.";

            // create a new EKEventEditViewController. This controller is built in an allows
            // the user to create a new, or edit an existing event.
            var eventController = new EventKitUI.EKEventEditViewController { EventStore = EventStore };
            eventController.Event = newEvent;

            // wire up a delegate to handle events from the controller
            eventController.EditViewDelegate = new CreateEventEditViewDelegate(eventController);

            // show the event controller
            base.PresentViewController(eventController, true, null);
        }

        /// <summary>
        /// This method illustrates how to save and retrieve events programmatically.
        /// </summary>
        private void SaveAndRetrieveEvent()
        {
            var newEvent = EKEvent.FromStore(EventStore);
            // set the alarm for 5 minutes from now
            newEvent.AddAlarm(EKAlarm.FromDate((NSDate)DateTime.Now.AddMinutes(5)));
            // make the event start 10 minutes from now and last 30 minutes
            newEvent.StartDate = (NSDate)DateTime.Now.AddMinutes(10);
            newEvent.EndDate = (NSDate)DateTime.Now.AddMinutes(40);
            newEvent.Title = "Appt. to do something Awesome!";
            newEvent.Notes = "Find a boulder, climb it. Find a river, swim it. Find an ocean, dive it.";
            newEvent.Calendar = EventStore.DefaultCalendarForNewEvents;

            // save the event
            EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out NSError error);
            if (error != null)
            {
                var alert = UIAlertController.Create("Error", error.ToString(), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                base.PresentViewController(alert, true, null);
            }
            else
            {
                var alert = UIAlertController.Create("Event Saved", $"Event ID: {newEvent.EventIdentifier}", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                base.PresentViewController(alert, true, null);

                // to retrieve the event you can call
                var savedEvent = EventStore.EventFromIdentifier(newEvent.EventIdentifier);
                Console.WriteLine($"Retrieved Saved Event: {savedEvent.Title}");

                // to delete, note that once you remove the event, the reference will be null, so
                // if you try to access it you'll get a null reference error.
                EventStore.RemoveEvent(savedEvent, EKSpan.ThisEvent, true, out error);
                Console.WriteLine("Event Deleted.");
            }
        }

        #endregion

        #region Reminders

        /// <summary>
        /// Creates and saves a reminder to the default reminder calendar
        /// </summary>
        private void CreateReminder()
        {
            // create a reminder using the EKReminder.Create method
            var reminder = EKReminder.Create(EventStore);
            reminder.Title = "Do something awesome!";
            reminder.Calendar = EventStore.DefaultCalendarForNewReminders;

            // save the reminder
            EventStore.SaveReminder(reminder, true, out NSError error);
            // if there was an error, show it
            if (error != null)
            {
                var alert = UIAlertController.Create("Error", error.ToString(), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                base.PresentViewController(alert, true, null);
            }
            else
            {
                var alert = UIAlertController.Create("Reminder saved", $"ID: {reminder.CalendarItemIdentifier}", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                base.PresentViewController(alert, true, null);

                // to retrieve the reminder you can call GetCalendarItem
                var myReminder = EventStore.GetCalendarItem(reminder.CalendarItemIdentifier);
                Console.WriteLine($"Retrieved Saved Reminder: {myReminder.Title}");

                // TODO: you can uncomment it to see how does removal work
                // to delete, note that once you remove the event, the reference will be null, so
                // if you try to access it you'll get a null reference error.
                //EventStore.RemoveReminder(myReminder as EKReminder, true, out error);
                //Console.WriteLine("Reminder Deleted.");
            }
        }

        #endregion
    }
}