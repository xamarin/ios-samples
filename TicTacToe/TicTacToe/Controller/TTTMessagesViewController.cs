using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
	public class TTTMessagesViewController : UITableViewController
	{
		readonly NSString cellIdentifier = new NSString ("Cell");

		TTTProfile profile;
		TTTMessage selectedMessage;

		UIImage FavoriteImage {
			get {
				return UIImage.FromBundle ("favorite").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
			}
		}

		UIImage UnselectedFavoriteImage {
			get {
				return UIImage.FromBundle ("favoriteUnselected");
			}
		}

		public TTTMessagesViewController ()
			: base (UITableViewStyle.Plain)
		{
			Title = "Messages";
			TabBarItem.Image = UIImage.FromBundle ("messagesTab");
			TabBarItem.SelectedImage = UIImage.FromBundle ("messagesTabSelected");

			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Compose, NewMessage);
			TTTMessageServer.SharedMessageServer.MessagesAdded += OnMessagesAdded;

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (new UIImage (), UIBarButtonItemStyle.Plain, Favorite);
			UpdateFavoriteButton ();
		}

		public static UIViewController FromProfile (TTTProfile profile, string profilePath)
		{
			TTTMessagesViewController controller = new TTTMessagesViewController ();
			controller.profile = profile;
			UINavigationController navController = new UINavigationController (controller);
			return navController;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterClassForCellReuse (typeof(TTTMessageTableViewCell), cellIdentifier);
		}

		void NewMessage (object sender, EventArgs e)
		{
			var controller = new TTTNewMessageViewController ();
			controller.Profile = profile;
			controller.PresentFromViewController (this);
		}

		void OnMessagesAdded (object sender, MessageEvenArg e)
		{
			NSIndexPath[] paths = e.Indexes.Select (index => NSIndexPath.FromRowSection (index, 0)).ToArray ();
			TableView.InsertRows (paths, UITableViewRowAnimation.Automatic);
		}

		void Favorite (object sender, EventArgs e)
		{
			bool fav = !TTTMessageServer.SharedMessageServer.IsFavoriteMessage (selectedMessage);
			TTTMessageServer.SharedMessageServer.SetFavorite (fav, selectedMessage);
			UpdateFavoriteButton ();
		}

		void UpdateFavoriteButton ()
		{
			bool isFavorite = selectedMessage != null
			                  && TTTMessageServer.SharedMessageServer.IsFavoriteMessage (selectedMessage);

			UIImage image = isFavorite ? FavoriteImage : UnselectedFavoriteImage;

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
			UpdateFavoriteButton ();
		}
		#endregion
	}

	public class TTTMessageTableViewCell : UITableViewCell
	{
		bool showReplyButton;

		public UIButton ReplyButton { get; set; }

		public TTTMessageTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		public void SetShowReplyButton (bool value)
		{
			if (showReplyButton == value)
				return;

			showReplyButton = value;
			AccessoryView = showReplyButton ? ReplyButton : null;
		}
	}
}

