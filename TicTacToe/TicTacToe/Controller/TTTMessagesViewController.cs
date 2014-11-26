using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
	public class TTTMessagesViewController : UITableViewController
	{
		public TTTProfile Profile;
		TTTMessage selectedMessage;
		NSString cellIdentifier = new NSString ("Cell");

		public TTTMessagesViewController () : base (UITableViewStyle.Plain)
		{
			Title = "Messages";
			TabBarItem.Image = UIImage.FromBundle ("messagesTab");
			TabBarItem.SelectedImage = UIImage.FromBundle ("messagesTabSelected");

			NavigationItem.RightBarButtonItem = new UIBarButtonItem (
				UIBarButtonSystemItem.Compose, NewMessage);
			NSNotificationCenter.DefaultCenter.AddObserver ((NSString)TTTMessageServer.DidAddMessagesNotification,
			                                                DidAddMessages);

			NavigationItem.LeftBarButtonItem = 
				new UIBarButtonItem (new UIImage (), UIBarButtonItemStyle.Plain, favorite);
			updateFavoriteButton ();
		}

		public static UIViewController FromProfile (TTTProfile profile, string profilePath)
		{
			TTTMessagesViewController controller = new TTTMessagesViewController ();
			controller.Profile = profile;
			UINavigationController navController = new UINavigationController (controller);
			return navController;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterClassForCellReuse (typeof(TTTMessageTableViewCell),
			                                     cellIdentifier);
		}

		void NewMessage (object sender, EventArgs e)
		{
			TTTNewMessageViewController controller = new TTTNewMessageViewController ();
			controller.Profile = Profile;
			controller.PresentFromViewController (this);
		}

		void DidAddMessages (NSNotification notification)
		{
			var addedIndexes = (NSArray)notification.UserInfo [TTTMessageServer.AddedMessageIndexesUserInfoKey];
			var paths = NSArray.FromArray<NSNumber> (addedIndexes)
				.Select (num => NSIndexPath.FromRowSection (num.NIntValue, 0))
				.ToArray ();
			TableView.InsertRows (paths, UITableViewRowAnimation.Automatic);
		}

		void favorite (object sender, EventArgs e)
		{
			bool fav = !TTTMessageServer.SharedMessageServer.IsFavoriteMessage (selectedMessage);
			TTTMessageServer.SharedMessageServer.SetFavorite (fav, selectedMessage);
			updateFavoriteButton ();
		}

		void updateFavoriteButton ()
		{
			bool fav = false;
			if (selectedMessage != null)
				fav = TTTMessageServer.SharedMessageServer.IsFavoriteMessage (selectedMessage);

			UIImage image;
			if (fav)
				image = UIImage.FromBundle ("favorite").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
			else
				image = UIImage.FromBundle ("favoriteUnselected");

			NavigationItem.LeftBarButtonItem.Image = image;
			NavigationItem.LeftBarButtonItem.Enabled = (selectedMessage != null);
		}

		#region Table View
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TTTMessageServer.SharedMessageServer.NumberOfMessages;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return tableView.DequeueReusableCell (cellIdentifier, indexPath);
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			TTTMessageTableViewCell messageCell = (TTTMessageTableViewCell)cell;
			TTTMessage message = TTTMessageServer.SharedMessageServer.MessageAtIndex (indexPath.Row);
			cell.TextLabel.Text = message.Text;
			cell.ImageView.Image = TTTProfile.SmallImageForIcon (message.Icon);

			if (messageCell.ReplyButton == null) {
				UIButton replyButton = UIButton.FromType (UIButtonType.System);
				replyButton.TouchUpInside += NewMessage;
				replyButton.SetImage (UIImage.FromBundle ("reply"), UIControlState.Normal);
				replyButton.SizeToFit ();
				messageCell.ReplyButton = replyButton;
			}

			bool isSelected = tableView.IndexPathForSelectedRow != null &&
				tableView.IndexPathForSelectedRow.Equals (indexPath);
			messageCell.SetShowReplyButton (isSelected);
		}

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			TTTMessageTableViewCell cell = (TTTMessageTableViewCell)tableView.CellAt (indexPath);
			cell.SetShowReplyButton (false);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			TTTMessage message = TTTMessageServer.SharedMessageServer.MessageAtIndex (indexPath.Row);
			TTTMessageTableViewCell cell = (TTTMessageTableViewCell)tableView.CellAt (indexPath);

			if (selectedMessage == message) {
				tableView.DeselectRow (indexPath, true);
				selectedMessage = null;
				cell.SetShowReplyButton (false);
			} else {
				selectedMessage = message;
				cell.SetShowReplyButton (true);
			}
			updateFavoriteButton ();
		}
		#endregion
	}

	public class TTTMessageTableViewCell : UITableViewCell
	{
		public UIButton ReplyButton;
		public bool ShowReplyButton;

		public TTTMessageTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public void SetShowReplyButton (bool value)
		{
			if (ShowReplyButton != value) {
				ShowReplyButton = value;
				if (ShowReplyButton)
					AccessoryView = ReplyButton;
				else
					AccessoryView = null;
			}
		}
	}
}

