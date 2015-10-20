using System;
using System.Linq;

using UIKit;
using Foundation;

namespace UICatalog
{
	public class SearchControllerBaseViewController : UITableViewController
	{
		static readonly NSString kTableViewCellIdentifier = new NSString ("searchResultsCell");

		string[] allResults;
		string[] visibleResults;

		string filterString;

		public SearchControllerBaseViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			allResults = new [] { "Here's", "to", "the", "crazy", "ones.", "The", "misfits.", "The", "rebels.",
				"The", "troublemakers.", "The", "round", "pegs", "in", "the", "square", "holes.", "The", "ones", "who",
				"see", "things", "differently.", "They're", "not", "fond", "of", @"rules.", "And", "they", "have", "no",
				"respect", "for", "the", "status", "quo.", "You", "can", "quote", "them,", "disagree", "with", "them,",
				"glorify", "or", "vilify", "them.", "About", "the", "only", "thing", "you", "can't", "do", "is", "ignore",
				"them.", "Because", "they", "change", "things.", "They", "push", "the", "human", "race", "forward.",
				"And", "while", "some", "may", "see", "them", "as", "the", "crazy", "ones,", "we", "see", "genius.",
				"Because", "the", "people", "who", "are", "crazy", "enough", "to", "think", "they", "can", "change",
				"the", "world,", "are", "the", "ones", "who", "do."
			};

			visibleResults = allResults;
		}

		protected void ApplyFilter (string filter)
		{
			filterString = filter;

			if (string.IsNullOrWhiteSpace (filterString))
				visibleResults = allResults;
			else
				visibleResults = allResults.Where (s => s.Contains (filterString)).ToArray ();

			TableView.ReloadData ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return visibleResults.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (kTableViewCellIdentifier, indexPath);
			cell.TextLabel.Text = visibleResults [indexPath.Row];

			return cell;
		}
	}
}