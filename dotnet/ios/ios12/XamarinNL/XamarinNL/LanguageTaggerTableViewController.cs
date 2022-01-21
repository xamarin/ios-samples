namespace XamarinNL;

public partial class LanguageTaggerTableViewController : UITableViewController
{
	const string EntityCell = "EntityCell";

	public NSValue[] TokenRanges { get; set; } = Array.Empty<NSValue> ();
	public NSString[] Tags { get; set; } = Array.Empty<NSString> ();
	public string Text { get; set; } = string.Empty;

	protected LanguageTaggerTableViewController (IntPtr handle) : base (handle) { }

	public override nint RowsInSection (UITableView tableView, nint section) => Tags.Length;

	public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
	{
		if (TableView.DequeueReusableCell (EntityCell) is UITableViewCell cell) {
			var range = TokenRanges[indexPath.Row].RangeValue;
			var token = Text.Substring((int)range.Location, (int)range.Length);

			if (indexPath.Row >= 0 && indexPath.Row < Tags.Length) {
				var tag = Tags[indexPath.Row];
				cell.AddTextLabel (token, tag);
			} else {
				cell.AddTextLabel (token);
			}

			return cell;
		}
		throw new InvalidOperationException ("UITableViewCell");
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
