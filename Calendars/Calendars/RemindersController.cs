using EventKit;
using Foundation;
using System;
using UIKit;

namespace Calendars
{
    public partial class RemindersController : UITableViewController
    {
        private EKReminder[] reminders;

        public RemindersController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // create our NSPredicate which we'll use for the query
            using (var query = MainViewController.EventStore.PredicateForReminders(null))
            {
                // execute the query
                MainViewController.EventStore.FetchReminders(query, (collection) =>
                {
                    this.reminders = collection;
                });
            }

            this.TableView.ReloadData();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.reminders?.Length ?? 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("eventCellIdentifier") ?? new UITableViewCell();
            cell.TextLabel.Text = this.reminders[indexPath.Row].Title;
            return cell;
        }
    }
}