using System;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace AdaptivePhotos
{
	public class AAPLConversationViewController : CustomTableViewController
	{
		private readonly NSString AAPLListTableViewControllerCellIdentifier = new NSString ("Cell");

		public AAPLConversation Conversation { get; set; }

		public AAPLConversationViewController () : base (UITableViewStyle.Plain)
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

			AAPLPhoto visiblePhoto = this.Aapl_currentVisibleDetailPhotoWithSender ();
			if (visiblePhoto != null) {
				foreach (var indexPath in TableView.IndexPathsForVisibleRows) {
					AAPLPhoto photo = PhotoForIndexPath (indexPath);
					if (photo == visiblePhoto)
						TableView.SelectRow (indexPath, false, UITableViewScrollPosition.None);
				}
			}
		}

		public override bool Aapl_containsPhoto (AAPLPhoto photo)
		{
			for (int i = 0; i < Conversation.Photos.Count; i++) {
				if (Conversation.Photos.GetItem<AAPLPhoto> (i) == photo)
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

		private AAPLPhoto PhotoForIndexPath (NSIndexPath indexPath)
		{
			return Conversation.Photos.GetItem<AAPLPhoto> (indexPath.Item);
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return (int)Conversation.Photos.Count;
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

			AAPLPhoto photo = PhotoForIndexPath (indexPath);
			cell.TextLabel.Text = photo.Comment;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			AAPLPhoto photo = PhotoForIndexPath (indexPath);
			var controller = new AAPLPhotoViewController ();
			controller.Photo = photo;

			int photoNumber = indexPath.Row + 1;
			int photoCount = (int)Conversation.Photos.Count;
			controller.Title = string.Format ("{0} of {1}", photoNumber, photoCount);
			ShowDetailViewController (controller, this);
		}
	}
}

