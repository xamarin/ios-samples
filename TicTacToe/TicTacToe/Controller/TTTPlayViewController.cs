using System;
using UIKit;
using Foundation;

namespace TicTacToe
{
	public class TTTPlayViewController : UIViewController
	{
		const float ControllerMargin = 20f;
		public TTTProfile Profile;
		public string ProfilePath;
		TTTGameView gameView;

		public TTTPlayViewController ()
		{
			Title = "Play";
			TabBarItem.Image = UIImage.FromBundle ("playTab");
			TabBarItem.SelectedImage = UIImage.FromBundle ("playTabSelected");

			NSNotificationCenter.DefaultCenter.AddObserver ((NSString)TTTProfile.IconDidChangeNotification, iconDidChange);
		}

		public static TTTPlayViewController FromProfile (TTTProfile profile, string profilePath)
		{
			return new TTTPlayViewController () {
				Profile = profile,
				ProfilePath = profilePath
			};
		}

		public override void LoadView ()
		{
			UIView view = new UIView ();

			UIButton newButton = UIButton.FromType (UIButtonType.System);
			newButton.TranslatesAutoresizingMaskIntoConstraints = false;
			newButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			newButton.SetTitle ("New Game", UIControlState.Normal);
			newButton.TitleLabel.Font = UIFont.PreferredBody;
			newButton.TouchUpInside += newGame;
			view.AddSubview (newButton);

			UIButton pauseButton = UIButton.FromType (UIButtonType.System);
			pauseButton.TranslatesAutoresizingMaskIntoConstraints = false;
			pauseButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			pauseButton.SetTitle ("Pause", UIControlState.Normal);
			pauseButton.TitleLabel.Font = UIFont.PreferredBody;
			pauseButton.TouchUpInside += togglePause;
			view.AddSubview (pauseButton);

			gameView = new TTTGameView () {
				ImageForPlayer = ImageForPlayer,
				ColorForPlayer = ColorForPlayer,
				CanSelect = CanSelect,
				DidSelect = DidSelect,
				TranslatesAutoresizingMaskIntoConstraints = false,
				Game = Profile.CurrentGame
			};
			view.AddSubview (gameView);

			nfloat topHeight = UIApplication.SharedApplication.StatusBarFrame.Size.Height;
			UITabBar tabBar = TabBarController.TabBar;
			nfloat bottomHeight = tabBar.Translucent ? tabBar.Frame.Size.Height : 0;

			var mTopHeight = new NSNumber (topHeight + ControllerMargin);
			var mBottomHeight = new NSNumber (bottomHeight + ControllerMargin);
			var mMargin = new NSNumber (ControllerMargin);

			view.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|-margin-[gameView]-margin-|",
				(NSLayoutFormatOptions)0,
				"margin", mMargin,
				"gameView", gameView));
			view.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|-margin-[pauseButton(==newButton)]-[newButton]-margin-|",
				(NSLayoutFormatOptions)0,
				"margin", mMargin,
				"pauseButton", pauseButton,
				"newButton", newButton));
			view.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-topHeight-[gameView]-margin-[newButton]-bottomHeight-|",
				(NSLayoutFormatOptions)0,
				"topHeight", mTopHeight,
				"gameView", gameView,
				"margin", mMargin,
				"newButton", newButton,
				"bottomHeight", mBottomHeight));
			view.AddConstraint (NSLayoutConstraint.Create (
				pauseButton, NSLayoutAttribute.Baseline,
				NSLayoutRelation.Equal,
				newButton, NSLayoutAttribute.Baseline,
				1f, 0f));

			View = view;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			updateBackground ();
		}

		void saveProfile ()
		{
			Profile.WriteToPath (ProfilePath);
		}

		void newGame (object sender, EventArgs e)
		{
			UIView.Animate (0.3f, delegate {
				gameView.Game = Profile.StartNewGame ();
				saveProfile ();
				updateBackground ();
			});
		}

		void togglePause (object sender, EventArgs e)
		{
			UIButton button = (UIButton)sender;
			bool paused = !button.Selected;
			button.Selected = paused;
			gameView.UserInteractionEnabled = !paused;
			UIView.Animate (0.3f, delegate {
				gameView.Alpha = (paused ? 0.25f : 1f);
			});
		}

		void iconDidChange (NSNotification notification)
		{
			gameView.UpdateGameState ();
		}

		bool isOver ()
		{
			return gameView.Game.Result != TTTGameResult.InProgress;
		}

		#region Game View methods
		public UIImage ImageForPlayer (TTTGameView gameView, TTTMovePlayer player)
		{
			return Profile.ImageForPlayer (player);
		}

		public UIColor ColorForPlayer (TTTGameView gameView, TTTMovePlayer player)
		{
			return Profile.ColorForPlayer (player);
		}

		public bool CanSelect (TTTGameView gameView, TTTMoveXPosition xPosition,
		                       TTTMoveYPosition yPosition)
		{
			return gameView.Game.CanAddMove (xPosition, yPosition);
		}

		public void DidSelect (TTTGameView gameView, TTTMoveXPosition xPosition, TTTMoveYPosition yPosition)
		{
			UIView.Animate (0.3, delegate {
				gameView.Game.AddMove (xPosition, yPosition);
				gameView.UpdateGameState ();
				saveProfile ();
				updateBackground ();
			});
		}
		#endregion

		void updateBackground ()
		{
			bool gameOver = isOver ();
			gameView.GridColor = gameOver ? UIColor.White : UIColor.Black;
			View.BackgroundColor = (gameOver ? UIColor.Black : UIColor.White).ColorWithAlpha (0.75f);
			SetNeedsStatusBarAppearanceUpdate ();
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return isOver () ? UIStatusBarStyle.LightContent : UIStatusBarStyle.Default;
		}
	}
}

