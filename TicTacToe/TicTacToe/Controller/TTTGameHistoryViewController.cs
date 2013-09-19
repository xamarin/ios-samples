using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace TicTacToe
{
	public class TTTGameHistoryViewController : UIViewController
	{
		const float Margin = 20f;
		public TTTProfile Profile;
		public TTTGame Game;
		TTTGameView gameView;
		TTTRatingControl ratingControl;

		public TTTGameHistoryViewController ()
		{
			Title = "Game";

			ratingControl = new TTTRatingControl (new RectangleF (0, 0, 150, 30));
			ratingControl.ValueChanged += changeRating;
			NavigationItem.TitleView = ratingControl;

			NSNotificationCenter.DefaultCenter.AddObserver (TTTProfile.IconDidChangeNotification,
			                                                iconDidChange);
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

			float topHeight = UIApplication.SharedApplication.StatusBarFrame.Size.Height +
				NavigationController.NavigationBar.Frame.Size.Height;
			NSDictionary bindings = NSDictionary.FromObjectAndKey (gameView, new NSString ("gameView"));
			NSDictionary metrics = NSDictionary.FromObjectsAndKeys (
				new NSObject[] { new NSNumber (topHeight + Margin),
				new NSNumber (Margin), new NSNumber (Margin)
			},
				new NSString [] { 
				new NSString ("topHeight"), 
				new NSString ("bottomHeight"),
				new NSString ("margin")
			});

			view.AddConstraints (NSLayoutConstraint.FromVisualFormat (
				"|-margin-[gameView]-margin-|", (NSLayoutFormatOptions)0,
				metrics, bindings));
			view.AddConstraints (NSLayoutConstraint.FromVisualFormat (
				"V:|-topHeight-[gameView]-bottomHeight-|", (NSLayoutFormatOptions)0,
				metrics, bindings));

			View = view;
		}

		void setGame (TTTGame value)
		{
			if (Game != value) {
				Game = value;
				gameView.Game = Game;
				ratingControl.Rating = Game.Rating;
			}
		}

		void changeRating (object sender, EventArgs e)
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

		void iconDidChange (NSNotification notification)
		{
			gameView.UpdateGameState ();
		}
		#endregion
	}
}

