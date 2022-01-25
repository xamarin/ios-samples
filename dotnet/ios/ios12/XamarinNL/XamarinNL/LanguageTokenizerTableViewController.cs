namespace XamarinNL;

public partial class LanguageTokenizerTableViewController : UITableViewController
{
	const string TokenCell = "TokenCell";

	public NSValue[] Tokens { get; set; } = Array.Empty<NSValue> ();
	public string Text { get; set; } = string.Empty;

	protected LanguageTokenizerTableViewController (IntPtr handle) : base (handle) { }

	public override nint RowsInSection (UITableView tableView, nint section) => Tokens.Length;

	public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
	{
		if (TableView.DequeueReusableCell (TokenCell) is UITableViewCell cell) {
			NSRange range = Tokens[indexPath.Row].RangeValue;

			cell.AddTextLabel (Text.Substring ( (int)range.Location, (int)range.Length));

			return cell;
		}
		throw new InvalidOperationException (nameof (UITableViewCell));
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
