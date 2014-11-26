using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace TicTacToe
{
	public class TTTGameHistoryViewController : UIViewController
	{
		const float Margin = 20f;

		TTTGameView gameView;
		TTTRatingControl ratingControl;

		public TTTProfile Profile { get; set; }
		public TTTGame Game { get; set; }

		public TTTGameHistoryViewController ()
		{
			Title = "Game";

			ratingControl = new TTTRatingControl (new CGRect (0, 0, 150, 30));
			ratingControl.ValueChanged += ChangeRating;
			NavigationItem.TitleView = ratingControl;

			NSNotificationCenter.DefaultCenter.AddObserver ((NSString)TTTProfile.IconDidChangeNotification,
			                                                IconDidChange);
		}

		public override void LoadView ()
		{
			UIView view = new UIView ();
			view.BackgroundColor = UIColor.White;

			gameView = new TTTGameView () {
				ImageForPlayer = ImageForPlayer,
				ColorForPlayer = ColorForPlayer,
				UserInteractionEnabled = false,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			view.AddSubview (gameView);
			gameView.Game = Game;

			nfloat topHeight = UIApplication.SharedApplication.StatusBarFrame.Size.Height +
				NavigationController.NavigationBar.Frame.Size.Height;

			view.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|-margin-[gameView]-margin-|",
				(NSLayoutFormatOptions)0,
				"margin", Margin,
				"gameView", gameView));

			view.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-topHeight-[gameView]-bottomHeight-|",
				(NSLayoutFormatOptions)0,
				"topHeight", topHeight + Margin,
				"gameView", gameView,
				"bottomHeight", Margin));

			View = view;
		}

		void ChangeRating (object sender, EventArgs e)
		{
			Game.Rating = ((TTTRatingControl)sender).Rating;
		}

		#region Game View

		public UIImage ImageForPlayer (TTTGameView gameView, TTTMovePlayer player)
		{
			return Profile.ImageForPlayer (player);
		}

		public UIColor ColorForPlayer (TTTGameView gameView, TTTMovePlayer player)
		{
			return Profile.ColorForPlayer (player);
		}

		void IconDidChange (NSNotification notification)
		{
			gameView.UpdateGameState ();
		}

		#endregion
	}
}

