namespace XamarinNL;

public partial class LanguageTokenizerTableViewController : UITableViewController
{
    const string TokenCell = "TokenCell";

    public NSValue[]? Tokens { get; set; }
    public string? Text { get; set; }

    public LanguageTokenizerTableViewController (IntPtr handle) : base (handle) { }

    public override nint RowsInSection (UITableView tableView, nint section)
    {
        if (Tokens is null)
            return 0;
        return Tokens.Length;
    }

    public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
    {
        if (Tokens is null)
            throw new InvalidOperationException ("Tokens");

        var cell = TableView.DequeueReusableCell (TokenCell);
        NSRange range = Tokens[indexPath.Row].RangeValue;

        if (cell is null)
            throw new InvalidOperationException ("cell");

        if (Text is not null)
		{
            var content = cell.DefaultContentConfiguration;
            content.Text = Text.Substring ( (int)range.Location, (int)range.Length);
            cell.ContentConfiguration = content;
        }

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
