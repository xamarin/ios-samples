global using NaturalLanguage;

namespace XamarinNL;

public partial class LanguageProbabilityTableViewController : UITableViewController
{
    const string LanguageProbabilityCell = "LanguageProbabilityCell";

    NSString[]? sortedLanguages;

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

    public LanguageProbabilityTableViewController (IntPtr handle) : base (handle) { }

    public override nint RowsInSection (UITableView tableView, nint section)
    {
        if (sortedLanguages is null)
            return 0;

        return sortedLanguages.Length;
    }

    public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
    {
        var cell = TableView.DequeueReusableCell (LanguageProbabilityCell);

        if (cell is null)
            throw new InvalidOperationException ("cell");

        if (sortedLanguages is null)
            throw new InvalidOperationException ("sortedLanguages");

        NSString languageAbbreviation = sortedLanguages[indexPath.Row];
        NLLanguage language = NLLanguageExtensions.GetValue (languageAbbreviation);

        var content = cell.DefaultContentConfiguration;
        content.Text = language.ToString ();

        if (probabilities is not null)
            content.SecondaryText = probabilities[languageAbbreviation].ToString ();

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
