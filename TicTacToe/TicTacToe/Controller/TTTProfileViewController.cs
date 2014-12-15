using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;
using ObjCRuntime;

namespace TicTacToe
{
	public class TTTProfileViewController : UITableViewController
	{
		public enum Section
		{
			Icon,
			Statistics,
			History,
			Count
		}

		NSString IconIdentifier = new NSString ("Icon");
		NSString StatisticsIdentifier = new NSString ("Statistics");
		NSString HistoryIdentifier = new NSString ("History");
		public TTTProfile Profile;
		public string ProfilePath;
		public TTTProfileIconTableViewCell iconCell;

		public TTTProfileViewController () : base (UITableViewStyle.Grouped)
		{
			Title = "Profile";
			TabBarItem.Image = UIImage.FromBundle ("profileTab");
			TabBarItem.SelectedImage = UIImage.FromBundle ("profileTabSelected");
		}

		public static UIViewController FromProfile (TTTProfile profile, string profilePath)
		{
			TTTProfileViewController controller = new TTTProfileViewController () {
				Profile = profile,
				ProfilePath = profilePath
			};
			return new UINavigationController (controller);
		}

		public override void LoadView ()
		{
			base.LoadView ();
			TableView.RegisterClassForCellReuse (typeof(TTTProfileIconTableViewCell),
			                                     IconIdentifier);
			TableView.RegisterClassForCellReuse (typeof(TTTProfileStatisticsTableViewCell),
			                                     StatisticsIdentifier);
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell),
			                                     HistoryIdentifier);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			int rowCount = (int)TableView.NumberOfRowsInSection ((nint)(int)Section.Statistics);
			List<NSIndexPath> indexPaths = new List<NSIndexPath> ();

			for (int row = 0; row < rowCount; row++)
				indexPaths.Add (NSIndexPath.FromRowSection (row, (int)Section.Statistics));

			TableView.ReloadRows (indexPaths.ToArray (), UITableViewRowAnimation.None);
		}

		public void ChangeIcon (object sender, EventArgs e)
		{
			Profile.Icon = (TTTProfileIcon)(int)((UISegmentedControl)sender).SelectedSegment;
		}

		#region Table View
		public override nint NumberOfSections (UITableView tableView)
		{
			return (int)Section.Count;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			switch (section) {
			case (int)Section.Icon:
			case (int)Section.History:
				return 1;
			case (int)Section.Statistics:
				return 3;
			}

			return 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Section) {
			case (int)Section.Icon:
				return tableView.DequeueReusableCell (IconIdentifier, indexPath);
			case (int)Section.Statistics:
				return tableView.DequeueReusableCell (StatisticsIdentifier, indexPath);
			case (int)Section.History:
				return tableView.DequeueReusableCell (HistoryIdentifier, indexPath);
			}

			return null;
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			int section = indexPath.Section;
			int row = indexPath.Row;

			switch (section) {
			case (int)Section.Icon:
				if (iconCell == null) {
					iconCell = (TTTProfileIconTableViewCell)cell;
					iconCell.SegmentedControl.ValueChanged += ChangeIcon;
				}

				iconCell.SelectionStyle = UITableViewCellSelectionStyle.None;
				iconCell.SegmentedControl.SelectedSegment = (int)Profile.Icon;
				break;
			case (int)Section.Statistics:
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				if (row == 0) {
					cell.TextLabel.Text = "Victories";
					cell.ImageView.Image = UIImage.FromBundle ("victory").
						ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
					((TTTProfileStatisticsTableViewCell)cell).CountView.Count =
						Profile.NumberOfGamesWithResult (TTTGameResult.Victory);
				} else if (row == 1) {
					cell.TextLabel.Text = "Defeats";
					cell.ImageView.Image = UIImage.FromBundle ("defeat").
						ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
					((TTTProfileStatisticsTableViewCell)cell).CountView.Count =
						Profile.NumberOfGamesWithResult (TTTGameResult.Defeat);
				} else if (row == 2) {
					cell.TextLabel.Text = "Draws";
					cell.ImageView.Image = UIImage.FromBundle ("draw").
						ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
					((TTTProfileStatisticsTableViewCell)cell).CountView.Count =
						Profile.NumberOfGamesWithResult (TTTGameResult.Draw);
				}
				break;
			case (int)Section.History:
				cell.TextLabel.Text = "Show History";
				cell.TextLabel.TextAlignment = UITextAlignment.Center;
				break;
			}
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			if (section == (int)Section.Statistics)
				return "Statistics";

			return null;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == (int)Section.Icon)
				return 100f;

			return tableView.RowHeight;
		}

		public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == (int)Section.History)
				return indexPath;

			return null;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			TTTHistoryListViewController controller = new TTTHistoryListViewController ();
			controller.NavigationItem.RightBarButtonItem = new UIBarButtonItem (
				UIBarButtonSystemItem.Done, closeHistory);
			controller.Profile = Profile;
			UINavigationController navController = new UINavigationController (controller);
			navController.NavigationBar.BackIndicatorImage = UIImage.FromBundle ("backIndicator");
			navController.NavigationBar.BackIndicatorTransitionMaskImage = UIImage.FromBundle ("backIndicatorMask");

			PresentViewController (navController, true, null);
		}
		#endregion

		void closeHistory (object sender, EventArgs e)
		{
			DismissViewController (true, null);
		}
	}

	public class TTTProfileIconTableViewCell : UITableViewCell
	{
		public UISegmentedControl SegmentedControl;

		[Export ("initWithStyle:reuseIdentifier:")]
		public TTTProfileIconTableViewCell (UITableViewCellStyle style, string reuseIdentifier) :
			base (style, reuseIdentifier)
		{
			UIImage x = TTTProfile.ImageForIcon (TTTProfileIcon.X).
				ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
			UIImage o = TTTProfile.ImageForIcon (TTTProfileIcon.O).
				ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);

			SegmentedControl = new UISegmentedControl (new object[] { x, o }) {
				Frame = new CGRect (UIScreen.MainScreen.Bounds.Width / 2 - 120, 0, 240, 80),
				AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin |
					UIViewAutoresizing.FlexibleBottomMargin
			};
			UIEdgeInsets capInsets = new UIEdgeInsets (6f, 6f, 6f, 6f);
			SegmentedControl.SetBackgroundImage (
				UIImage.FromBundle ("segmentBackground").CreateResizableImage (capInsets),
				UIControlState.Normal, UIBarMetrics.Default);
			SegmentedControl.SetBackgroundImage (
				UIImage.FromBundle ("segmentBackgroundHighlighted").CreateResizableImage (capInsets),
				UIControlState.Highlighted, UIBarMetrics.Default);
			SegmentedControl.SetBackgroundImage (
				UIImage.FromBundle ("segmentBackgroundSelected").CreateResizableImage (capInsets),
				UIControlState.Selected, UIBarMetrics.Default);
			SegmentedControl.SetDividerImage (
				UIImage.FromBundle ("segmentDivider"), UIControlState.Normal,
				UIControlState.Normal, UIBarMetrics.Default);

			UIView containerView = new UIView (SegmentedControl.Frame) {
				Frame = ContentView.Bounds,
				AutoresizingMask =
				UIViewAutoresizing.FlexibleWidth |
				UIViewAutoresizing.FlexibleHeight
			};
			containerView.AddSubview (SegmentedControl);

			ContentView.AddSubview (containerView);
		}
	}

	public class TTTProfileStatisticsTableViewCell : UITableViewCell
	{
		public TTTCountView CountView;

		[Export ("initWithStyle:reuseIdentifier:")]
		public TTTProfileStatisticsTableViewCell (UITableViewCellStyle style, string reuseIdentifier) :
			base (style, reuseIdentifier)
		{
			CountView = new TTTCountView (new CGRect (0, 0, 160, 20));
			AccessoryView = CountView;
		}
	}
}

