using Foundation;
using NaturalLanguage;
using System;
using UIKit;

namespace XamarinNL
{
    public partial class LanguageTaggerTableViewController : UITableViewController
    {
        const string EntityCell = "EntityCell";

        public NSValue[] TokenRanges { get; set; }
        public NSString[] Tags { get; set; }
        public string Text { get; set; }

        public LanguageTaggerTableViewController(IntPtr handle) : base(handle) { }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return Tags.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(EntityCell);

            var range = TokenRanges[indexPath.Row].RangeValue;
            var token = Text.Substring((int)range.Location, (int)range.Length);
            var tag = Tags[indexPath.Row];

            cell.TextLabel.Text = token;
            cell.DetailTextLabel.Text = tag;

            return cell;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.ReloadData();
        }
    }
}