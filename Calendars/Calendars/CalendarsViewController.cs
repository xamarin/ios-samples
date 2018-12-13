using EventKit;
using Foundation;
using System;
using UIKit;

namespace Calendars
{
    public partial class CalendarsViewController : UITableViewController
    {
        private EKCalendar[] calendars;

        public CalendarsViewController (IntPtr handle) : base (handle) { }

        public EKEntityType EntityType { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // get calendars of the particular type (either events or reminders)
            this.calendars = MainViewController.EventStore.GetCalendars(this.EntityType);
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.calendars.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("calendarCellIdentifier") ?? new UITableViewCell();
            cell.TextLabel.Text = this.calendars[indexPath.Row].Title;
            return cell;
        }
    }
}