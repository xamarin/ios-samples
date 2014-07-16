using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using EventKit;
using Foundation;
using UIKit;
using EventKitUI;
using MonoTouch.Dialog;

namespace PokerNightVoting
{
	public partial class PokerNightVotingViewController : DialogViewController 
	{
		public PNVModel model { get; set; }

		public PokerNightVotingViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			model = new PNVModel ();

			if(model.SelectedCalendar != null)
				Title = model.SelectedCalendar.Title;

			// Start listening for changes
			model.StartBoradcastingModelChangedNotificaitons ();
			NSNotificationCenter.DefaultCenter.AddObserver (model.PNVModelChangedNotification, delegate {
				RefreshView (); 
			});

			// Fetch the poker events
			// When it is done it will send a PNVModelChangedNotification, which updates our view.
			model.FetchPokerEvents ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Stop listening for changes
			model.StopBroadcastingModelChangedNotifications ();

			NSNotificationCenter.DefaultCenter.RemoveObserver (this);

			// Relinquish ownership of anything that can be recreated in ViewDidLoad or on demand.
			model = null;
		}

		public EKEvent EventAtIndexPath (NSIndexPath indexPath)
		{
			// Given an index path from our table view, figure out which event is at that path
			var date = model.EventDates.ElementAt ((int)indexPath.Section);
			var eventsWithStartDate = model.eventDateToEventsDictionary [date];

			return eventsWithStartDate.ElementAt ((int)indexPath.Row);
		}

		string GetCaption (int section)
		{
			var now = DateTime.Now;
			var evnt = model.EventDates.ElementAt (section);
			
			if (now.Day == evnt.Day && now.Month == evnt.Month && now.Year == evnt.Year) 
				return "Today";

			return evnt.Date.ToShortDateString ();
		}

		public void RefreshView ()
		{
			Root = new RootElement (model.SelectedCalendar.Title);
			int sections = model.EventDates.Count;

			for (int i = 0; i < sections; i++) {
				var section = new Section (GetCaption (i));
				DateTime date = model.EventDates.ElementAt (i);
				int sectionElements = model.eventDateToEventsDictionary [date].Count;

				for (int row = 0; row < sectionElements; row++) {
					var eventsWithStartDate = model.eventDateToEventsDictionary [date];
					var evt = eventsWithStartDate.ElementAt (row);

					// Configure the cell.
					var dt = (DateTime) evt.StartDate;
					var local = TimeZone.CurrentTimeZone.ToLocalTime (dt);

					string votes = "";
					if (model.AreNotesInteger (evt))
						votes = "Votes: " + evt.Notes;

					var entry = new StyledStringElement (local.ToShortTimeString (), votes, UITableViewCellStyle.Value1);

					entry.Tapped += delegate {
						var controller = new EKEventViewController ();
						controller.Event = EventAtIndexPath (entry.IndexPath);
						controller.AllowsEditing = true;
						controller.Completed += (object sender, EKEventViewEventArgs e) => 
						{
							model.FetchPokerEvents ();
						};
						
						NavigationController.PushViewController (controller, true);
					};
				
					entry.Accessory = UITableViewCellAccessory.DetailDisclosureButton;

					entry.AccessoryTapped += delegate {
						var ekevent = EventAtIndexPath (entry.IndexPath);
						model.IncreaseVoteOnEvent (ekevent);
					};

					section.Add (entry);
				}

				Root.Add (section);
			}
		}

		partial void addTime (UIKit.UIBarButtonItem sender)
		{
			// Show the EKEventEditViewController
			var controller = new EKEventEditViewController ();
			controller.EventStore = model.EventStore;
			controller.Completed += (object obj, EKEventEditEventArgs e) => 
			{
				DismissViewController (true, null);

				if (e.Action != EKEventEditViewAction.Canceled) {
					// Update our events, since we added a new event.
					model.FetchPokerEvents ();
				}
			};
			controller.GetDefaultCalendarForNewEvents += delegate (EKEventEditViewController obj)
			{
				return model.SelectedCalendar;
			};

			var ekevent = EKEvent.FromStore (model.EventStore);
			ekevent.Title = PNVModel.DefaultEventTitle;
			ekevent.TimeZone = NSTimeZone.LocalTimeZone;
			ekevent.StartDate = NSDate.Now;
			ekevent.EndDate = NSDate.Now.AddSeconds (60 * 60);
			controller.Event = ekevent;

			PresentViewController (controller, true, null);
		}

		partial void showCalendarChooser (UIKit.UIBarButtonItem sender)
		{
			// Show the EKCalendarChooser
			var calendarChooser = new EKCalendarChooser (EKCalendarChooserSelectionStyle.Single, 
			                                                           EKCalendarChooserDisplayStyle.WritableCalendarsOnly,
			                                                           model.EventStore);
			calendarChooser.ShowsDoneButton = true;
			calendarChooser.ShowsCancelButton = false;
			calendarChooser.SelectionChanged += (object obj, EventArgs e) => 
			{
				// Called whenever the selection is changed by the user
				model.SelectedCalendar = (EKCalendar) calendarChooser.SelectedCalendars.AnyObject;
				Title = model.SelectedCalendar.Title;
			};
			calendarChooser.Finished += (object obj, EventArgs e) => 
			{
				// These are called when the corresponding button is pressed to dismiss the
				// controller. It is up to the recipient to dismiss the chooser.
				model.FetchPokerEvents ();
				DismissViewController (true, null);
			};
			calendarChooser.SelectionChanged += (object obj, EventArgs e) => 
			{
				// Update our events, since the selected calendar may have changed.
				model.SelectedCalendar = (EKCalendar) calendarChooser.SelectedCalendars.AnyObject;
				Title = model.SelectedCalendar.Title;
			};

			if (model.SelectedCalendar != null) {
				EKCalendar[] temp = new EKCalendar [1];
				temp [0] = model.SelectedCalendar;
				var selectedCalendars = new NSSet (temp);
				calendarChooser.SelectedCalendars = selectedCalendars;
			}

			UINavigationController navigationController = new UINavigationController (calendarChooser);
			PresentViewController (navigationController, true, null);
		}
	}
}