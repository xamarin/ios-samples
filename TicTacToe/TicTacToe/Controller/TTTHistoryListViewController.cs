using System;
using UIKit;
using Foundation;

namespace TicTacToe
{
	public class TTTHistoryListViewController : UITableViewController
	{
		public TTTProfile Profile;
		NSString CellIdentifier = new NSString ("Cell");

		public TTTHistoryListViewController () : base (UITableViewStyle.Plain)
		{
			Title = "History";
		}

		public override void LoadView ()
		{
			base.LoadView ();
			TableView.RegisterClassForCellReuse (typeof(TTTHistoryListTableViewCell), CellIdentifier);
		}

		#region Table View
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return Profile.Games.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier, indexPath);

			if (cell != null)
				return cell;

			return new TTTHistoryListTableViewCell (UITableViewCellStyle.Subtitle, CellIdentifier);
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			TTTGame game = Profile.Games [indexPath.Row];
			cell.TextLabel.Text = game.Date.ToString ();
			TTTGameResult result = game.Result;
			if (result == TTTGameResult.Victory)
				cell.DetailTextLabel.Text = "Victory";
			else if (result == TTTGameResult.Defeat)
				cell.DetailTextLabel.Text = "Defeat";
			else if (result == TTTGameResult.Draw)
				cell.DetailTextLabel.Text = "Draw";
			else
				cell.DetailTextLabel.Text = "In Progress";
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			TTTGameHistoryViewController controller = new TTTGameHistoryViewController ();
			controller.Profile = Profile;
			controller.Game = Profile.Games [indexPath.Row];
			NavigationController.PushViewController (controller, true);
		}
		#endregion
	}

	public class TTTHistoryListTableViewCell : UITableViewCell
	{
		[Export ("initWithStyle:reuseIdentifier:")]
		public TTTHistoryListTableViewCell (UITableViewCellStyle style, string reuseIdentifier) :
			base (UITableViewCellStyle.Subtitle, reuseIdentifier)
		{
			Accessory = UITableViewCellAccessory.DisclosureIndicator;
		}

		public override void TintColorDidChange ()
		{
			base.TintColorDidChange ();
			DetailTextLabel.TextColor = TintColor;
		}
	}
}

