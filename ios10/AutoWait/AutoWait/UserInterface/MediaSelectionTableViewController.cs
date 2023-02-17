using System;
using Foundation;
using UIKit;

namespace AutoWait {
	public struct MediaItem {
		public string Name;
		public NSUrl Url;
	}

	public partial class MediaSelectionTableViewController : UITableViewController {
		readonly MediaItem [] mediaItems = {
			new MediaItem {
				Name = "In the Woods",
				Url = new NSUrl ("http://devimages.apple.com.edgekey.net/samplecode/avfoundationMedia/AVFoundationQueuePlayer_Progressive.mov")
			}
			// Add your own media items here.
		};

		public MediaSelectionTableViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return mediaItems.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("Media", indexPath);
			cell.TextLabel.Text = mediaItems [indexPath.Row].Name;

			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "ShowMedia")
				return;

			var mediaVC = segue.DestinationViewController as MediaViewController;
			if (mediaVC == null)
				return;

			// Set the selected URL on the destionation view controller.
			var itemIndex = TableView.IndexPathForSelectedRow.Row;
			mediaVC.MediaUrl = mediaItems [itemIndex].Url;
		}
	}
}
