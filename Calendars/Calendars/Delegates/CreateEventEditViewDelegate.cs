using UIKit;

namespace Calendars.Delegates {
	public class CreateEventEditViewDelegate : EventKitUI.EKEventEditViewDelegate {
		// we need to keep a reference to the controller so we can dismiss it
		private readonly UIViewController eventController;

		public CreateEventEditViewDelegate (EventKitUI.EKEventEditViewController eventController)
		{
			// save our controller reference
			this.eventController = eventController;
		}

		// completed is called when a user eith
		public override void Completed (EventKitUI.EKEventEditViewController controller, EventKitUI.EKEventEditViewAction action)
		{
			this.eventController.DismissViewController (true, null);

			// action tells you what the user did in the dialog, so you can optionally
			// do things based on what their action was. additionally, you can get the
			// Event from the controller.Event property, so for instance, you could
			// modify the event and then resave if you'd like.
			switch (action) {
			case EventKitUI.EKEventEditViewAction.Canceled:
				break;
			case EventKitUI.EKEventEditViewAction.Deleted:
				break;
			case EventKitUI.EKEventEditViewAction.Saved:
				// if you wanted to modify the event you could do so here, and then
				// save:
				//App.Current.EventStore.SaveEvent ( controller.Event, )
				break;
			}
		}
	}
}
