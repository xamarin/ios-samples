using EventKit;
using Foundation;
using System;
using UIKit;

namespace Calendars
{
    public partial class EventsController : UITableViewController
    {
        private EKEvent[] events;

        public EventsController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // create our NSPredicate which we'll use for the query
            var startDate = (NSDate)DateTime.Now.AddDays(-7);
            var endDate = (NSDate)DateTime.Now;
            // the third parameter is calendars we want to look in, to use all calendars, we pass null
            using (var query = MainViewController.EventStore.PredicateForEvents(startDate, endDate, null))
            {
                // execute the query
                this.events = MainViewController.EventStore.EventsMatching(query);
            }

            this.TableView.ReloadData();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.events.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("eventCellIdentifier") ?? new UITableViewCell();
            cell.TextLabel.Text = this.events[indexPath.Row].Title;
            return cell;
        }
    }
}