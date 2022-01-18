namespace XamarinNL;

public partial class LanguageProbabilityTableViewController : UITableViewController
{
	const string LanguageProbabilityCell = "LanguageProbabilityCell";

	NSString[] sortedLanguages = Array.Empty<NSString> ();

	NSDictionary<NSString, NSNumber>? probabilities;
	public NSDictionary<NSString, NSNumber>? Probabilities
	{
		set
		{
			probabilities = value;
			if (value is not null)
				sortedLanguages = value.Keys.OrderByDescending (lang => value[lang].DoubleValue).ToArray<NSString> ();
		}
	}

	protected LanguageProbabilityTableViewController (IntPtr handle) : base (handle) { }

	public override nint RowsInSection (UITableView tableView, nint section) => sortedLanguages.Length;

	public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
	{
		if (TableView.DequeueReusableCell (LanguageProbabilityCell) is UITableViewCell cell)
		{
			var languageAbbreviation = sortedLanguages[indexPath.Row];
			var language = NLLanguageExtensions.GetValue (languageAbbreviation);

			var content = cell.DefaultContentConfiguration;
			content.Text = language.ToString ();

			if (probabilities is not null)
				content.SecondaryText = probabilities[languageAbbreviation].ToString ();

			cell.ContentConfiguration = content;

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
