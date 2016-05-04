using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace iTravel {
	public partial class ViewController : UITableViewController {
		class DestinationDetail {
			public string Title { get; set; }

			public string Tag { get; set; }
		}

		readonly List<DestinationDetail> sampleData = new List<DestinationDetail> {
			new DestinationDetail { Tag = "Paris", Title = "Paris" },
			new DestinationDetail { Tag = "Istanbul", Title = "Istanbul" },
			new DestinationDetail { Tag = "Rotterdam", Title = "Rotterdam" }
		};

		protected ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "showDetail")
				return;

			var indexPath = TableView.IndexPathForSelectedRow;
			var previewDetail = sampleData[indexPath.Row];

			var albumViewController = segue.DestinationViewController as AlbumViewController;
			if (albumViewController == null)
				return;

			albumViewController.LoadAlbum (previewDetail.Tag);
			albumViewController.Title = previewDetail.Title;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return sampleData.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell ("Cell");
			var previewDetail = sampleData[indexPath.Row];
			cell.TextLabel.Text = previewDetail.Title;
			return cell;
		}
	}
}

