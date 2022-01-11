namespace XamarinNL
{
    public partial class LanguageTaggerTableViewController : UITableViewController
    {
        const string EntityCell = "EntityCell";

        public NSValue[]? TokenRanges { get; set; }
        public NSString[]? Tags { get; set; }
        public string? Text { get; set; }

        public LanguageTaggerTableViewController (IntPtr handle) : base (handle) { }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            if (Tags is null)
                return 0;

            return Tags.Length;
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell (EntityCell);

            if (cell is null)
                throw new InvalidOperationException ("cell");

            var content = cell.DefaultContentConfiguration;

            if (TokenRanges is not null && Text is not null)
            {
                var range = TokenRanges[indexPath.Row].RangeValue;
                var token = Text.Substring ( (int)range.Location, (int)range.Length);
                content.Text = token;
            }

            if (Tags is not null)
            {
                var tag = Tags[indexPath.Row];
                content.SecondaryText = tag;
            }

            cell.ContentConfiguration = content;
            return cell;
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            TableView.ReloadData ();
        }
    }
}