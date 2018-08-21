using Foundation;
using System;
using UIKit;
using System.Linq;
using NaturalLanguage;

namespace XamarinNL
{
    public partial class LanguageTokenizerTableViewController : UITableViewController
    {
        const string TokenCell = "TokenCell";

        public NSValue[] Tokens { get; set; }
        public string Text { get; set; }

        public LanguageTokenizerTableViewController(IntPtr handle) : base(handle) { }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return Tokens.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(TokenCell);
            NSRange range = Tokens[indexPath.Row].RangeValue;
            cell.TextLabel.Text = Text.Substring((int)range.Location, (int)range.Length);
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