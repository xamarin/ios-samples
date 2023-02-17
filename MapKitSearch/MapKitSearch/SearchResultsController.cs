using Foundation;
using MapKit;
using System;
using UIKit;

namespace MapKitSearch {
	public partial class SearchResultsController : UITableViewController {
		private const string CellIdentifier = "cellIdentifier";

		public SearchResultsController (IntPtr handle) : base (handle) { }

		public MKMapItem [] Items { get; set; }

		public string Query { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationItem.Title = Query;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Items.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (CellIdentifier, indexPath);

			var item = Items [indexPath.Row];
			cell.TextLabel.Text = item.Name;
			cell.DetailTextLabel.Text = item.PhoneNumber;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Items [indexPath.Row].OpenInMaps ();
			tableView.DeselectRow (indexPath, false);
		}
	}
}
