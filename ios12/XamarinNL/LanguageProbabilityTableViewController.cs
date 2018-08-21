using Foundation;
using System;
using UIKit;
using System.Linq;
using NaturalLanguage;

namespace XamarinNL
{
    public partial class LanguageProbabilityTableViewController : UITableViewController
    {
        const string LanguageProbabilityCell = "LanguageProbabilityCell";

        NSString[] sortedLanguages;

        NSDictionary<NSString, NSNumber> probabilities;
        public NSDictionary<NSString, NSNumber> Probabilities
        {
            set
            {
                probabilities = value;
                sortedLanguages = value.Keys.OrderByDescending(lang => value[lang].DoubleValue).ToArray<NSString>();
            }
        }

        public LanguageProbabilityTableViewController(IntPtr handle) : base(handle) { }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return sortedLanguages.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(LanguageProbabilityCell);
            NSString languageAbbreviation = sortedLanguages[indexPath.Row];
            NLLanguage language = NLLanguageExtensions.GetValue(languageAbbreviation);
            cell.TextLabel.Text = language.ToString();
            cell.DetailTextLabel.Text = probabilities[languageAbbreviation].ToString();
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