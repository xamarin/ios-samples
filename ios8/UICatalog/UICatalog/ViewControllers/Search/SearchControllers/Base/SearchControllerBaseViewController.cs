using System;
using System.Linq;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace UICatalog
{
	public class SearchControllerBaseViewController : UITableViewController
	{
		private static readonly NSString kTableViewCellIdentifier = new NSString("searchResultsCell");

		private string[] _allResults;
		private string[] _visibleResults;

		private string _filterString;

		public SearchControllerBaseViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_allResults = new string[] { "Here's", "to", "the", "crazy", "ones.", "The", "misfits.", "The", "rebels.",
				"The", "troublemakers.", "The", "round", "pegs", "in", "the", "square", "holes.", "The", "ones", "who",
				"see", "things", "differently.", "They're", "not", "fond", "of", @"rules.", "And", "they", "have", "no",
				"respect", "for", "the", "status", "quo.", "You", "can", "quote", "them,", "disagree", "with", "them,",
				"glorify", "or", "vilify", "them.", "About", "the", "only", "thing", "you", "can't", "do", "is", "ignore",
				"them.", "Because", "they", "change", "things.", "They", "push", "the", "human", "race", "forward.",
				"And", "while", "some", "may", "see", "them", "as", "the", "crazy", "ones,", "we", "see", "genius.",
				"Because", "the", "people", "who", "are", "crazy", "enough", "to", "think", "they", "can", "change",
				"the", "world,", "are", "the", "ones", "who", "do."
			};

			_visibleResults = _allResults;
		}

		protected void ApplyFilter(string filter)
		{
			_filterString = filter;

			if (string.IsNullOrWhiteSpace(_filterString))
				_visibleResults = _allResults;
			else
				_visibleResults = _allResults.Where (s => s.Contains (_filterString)).ToArray ();

			TableView.ReloadData ();
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _visibleResults.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(kTableViewCellIdentifier, indexPath);
			cell.TextLabel.Text = _visibleResults[indexPath.Row];

			return cell;
		}
	}
}

