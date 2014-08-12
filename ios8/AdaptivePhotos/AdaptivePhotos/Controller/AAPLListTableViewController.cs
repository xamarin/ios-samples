using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace AdaptivePhotos
{
	public class AAPLListTableViewController : CustomTableViewController
	{
		private readonly NSString AAPLListTableViewControllerCellIdentifier = new NSString ("Cell");

		public AAPLUser User { get; private set; }

		public AAPLListTableViewController (AAPLUser user) : base (UITableViewStyle.Plain)
		{
			User = user;
			Title = "Conversations";
			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Profile", UIBarButtonItemStyle.Plain, ShowProfile);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), AAPLListTableViewControllerCellIdentifier);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector ("showDetailTargetDidChange:"), 
				UIViewController.ShowDetailTargetDidChangeNotification, null);
			ClearsSelectionOnViewWillAppear = false;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (TableView.IndexPathsForSelectedRows == null)
				return;

			foreach (var indexPath in TableView.IndexPathsForSelectedRows) {
				bool pushes = false;
				if (ShouldShowConversationViewForIndexPath (indexPath)) {
					pushes = this.Aapl_willShowingViewControllerPushWithSender ();
				} else {
					pushes = this.Aapl_willShowingDetailViewControllerPushWithSender ();
				}

				if (pushes)
					TableView.DeselectRow (indexPath, true);
			}
		}

		public override bool Aapl_containsPhoto (AAPLPhoto photo)
		{
			return true;
		}

		[Export ("showDetailTargetDidChange:")]
		public void ShowDetailTargetDidChange (NSNotification notification)
		{
			foreach (var cell in TableView.VisibleCells) {
				NSIndexPath indexPath = TableView.IndexPathForCell (cell);
				WillDisplay (TableView, cell, indexPath);
			}
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return (int)User.Conversations.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return TableView.DequeueReusableCell (AAPLListTableViewControllerCellIdentifier, indexPath);
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			bool pushes = false;
			if (ShouldShowConversationViewForIndexPath (indexPath)) {
				pushes = this.Aapl_willShowingViewControllerPushWithSender ();
			} else {
				pushes = this.Aapl_willShowingDetailViewControllerPushWithSender ();
			}

			cell.Accessory = pushes ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
			AAPLConversation conversation = ConversationForIndexPath (indexPath);
			cell.TextLabel.Text = conversation.Name;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			AAPLConversation conversation = ConversationForIndexPath (indexPath);
			if (ShouldShowConversationViewForIndexPath (indexPath)) {
				var controller = new AAPLConversationViewController {
					Conversation = conversation
				};

				controller.Title = conversation.Name;
				ShowViewController (controller, this);
			} else {
				var photo = conversation.Photos.GetItem <AAPLPhoto> ((int)conversation.Photos.Count - 1);
				var controller = new AAPLPhotoViewController {
					Photo = photo,
					Title = conversation.Name
				};

				ShowDetailViewController (controller, this);
			}
		}

		private void ShowProfile (object sender, EventArgs e)
		{
			var controller = new AAPLProfileViewController {
				User = User
			};

			controller.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, CloseProfile);
			var navController = new UINavigationController (controller);
			navController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			navController.PopoverPresentationController.BarButtonItem = (UIBarButtonItem)sender;
			navController.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Any;
			PresentViewController (navController, true, null);
		}

		private void CloseProfile (object sender, EventArgs e)
		{
			DismissViewControllerAsync (true);
		}

		private bool ShouldShowConversationViewForIndexPath (NSIndexPath indexPath)
		{
			AAPLConversation conversation = ConversationForIndexPath (indexPath);
			return conversation.Photos.Count != 1;
		}

		private AAPLConversation ConversationForIndexPath (NSIndexPath indexPath)
		{
			return User.Conversations.GetItem<AAPLConversation> (indexPath.Item);
		}
	}
}

