using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AdaptivePhotos
{
	public class ConversationViewController : CustomTableViewController
	{
		readonly NSString AAPLListTableViewControllerCellIdentifier = new NSString ("Cell");

		public Conversation Conversation { get; set; }

		public ConversationViewController () : base (UITableViewStyle.Plain)
		{
			ClearsSelectionOnViewWillAppear = false;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), AAPLListTableViewControllerCellIdentifier);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector ("showDetailTargetDidChange:"), 
				UIViewController.ShowDetailTargetDidChangeNotification, null);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);


			if (TableView.IndexPathsForSelectedRows == null)
				return;

			foreach (var indexPath in TableView.IndexPathsForSelectedRows) {
				bool indexPathPushes = this.Aapl_willShowingDetailViewControllerPushWithSender ();
				if (indexPathPushes)
					TableView.DeselectRow (indexPath, true);
			}

			Photo visiblePhoto = this.Aapl_currentVisibleDetailPhotoWithSender ();
			if (visiblePhoto != null) {
				foreach (var indexPath in TableView.IndexPathsForVisibleRows) {
					Photo photo = PhotoForIndexPath (indexPath);
					if (photo == visiblePhoto)
						TableView.SelectRow (indexPath, false, UITableViewScrollPosition.None);
				}
			}
		}

		public override bool Aapl_containsPhoto (Photo photo)
		{
			for (nint i = 0; i < (nint)Conversation.Photos.Count; i++) {
				if (Conversation.Photos.GetItem<Photo> (i) == photo)
					return true;
			}

			return false;
		}

		[Export ("showDetailTargetDidChange:")]
		public void ShowDetailTargetDidChange (NSNotification notification)
		{
			foreach (var cell in TableView.VisibleCells) {
				NSIndexPath indexPath = TableView.IndexPathForCell (cell);
				WillDisplay (TableView, cell, indexPath);
			}
		}

		Photo PhotoForIndexPath (NSIndexPath indexPath)
		{
			return Conversation.Photos.GetItem<Photo> (indexPath.Item);
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return (nint)Conversation.Photos.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return TableView.DequeueReusableCell (AAPLListTableViewControllerCellIdentifier, indexPath);
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			bool pushes = this.Aapl_willShowingDetailViewControllerPushWithSender ();

			if (pushes) {
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			} else {
				cell.Accessory = UITableViewCellAccessory.None;
			}

			Photo photo = PhotoForIndexPath (indexPath);
			cell.TextLabel.Text = photo.Comment;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Photo photo = PhotoForIndexPath (indexPath);
			var controller = new PhotoViewController ();
			controller.Photo = photo;

			int photoNumber = indexPath.Row + 1;
			nuint photoCount = Conversation.Photos.Count;
			controller.Title = string.Format ("{0} of {1}", photoNumber, photoCount);
			ShowDetailViewController (controller, this);
		}
	}
}

