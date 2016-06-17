using System;
using CoreGraphics;
using Foundation;
using UIKit;
using GameKit;

namespace MTGKTapper
{
	public partial class MTGKTapperViewController : UIViewController
	{
		const string EasyLeaderboardID = "com.appledts.EasyTapList";
		const string HardLeaderboardID = "com.appledts.HardTapList";
		const string AwesomeLeaderboardID = "com.appledts.AwesomeTapList";

		const string AchievementGotOneTap = "com.appletest.one_tap";
		const string AchievementHidden20Taps = "com.appledts.twenty_taps";
		const string AchievementBigOneHundred = "com.appledts.one_hundred_taps";

		const string AchievementNameGotOneTap = "Just One Tap";
		const string AchievementNameHidden20Taps = "Twenty Taps In";
		const string AchievementNameBigOneHundred = "The Big One Hundred";

		GameCenterManager gameCenterManager;
		GKLeaderboard currentLeaderBoard;
		string currentCategory = EasyLeaderboardID;
		long currentScore = 0;

		public MTGKTapperViewController () : base ("MTGKTapperViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			InitGameCenter ();

			string[] categories = { "Easy", "Hard", "Awesome" };
			var selectCategory = new UIActionSheet ("Choose Leaderboard", null, "Cancel", null, categories);
			selectCategory.Dismissed += (sender, e) => {
				switch (e.ButtonIndex) {
				case 0:
					currentCategory = EasyLeaderboardID;
					this.selectLeaderBoardButton.SetTitle ("Leaderboard: Easy", UIControlState.Normal);
					currentScore = 0;
					currentScoreTextField.Text = "0";
					break;
				case 1:
					currentCategory = HardLeaderboardID;
					this.selectLeaderBoardButton.SetTitle ("Leaderboard: Hard", UIControlState.Normal);
					currentScore = 0;
					currentScoreTextField.Text = "0";
					break;
				case 2:
					currentCategory = AwesomeLeaderboardID;
					this.selectLeaderBoardButton.SetTitle ("Leaderboard: Awesome", UIControlState.Normal);
					currentScore = 0;
					currentScoreTextField.Text = "0";
					break;
				default:
					break;
				}
				currentLeaderBoard = gameCenterManager.ReloadLeaderboard (currentCategory);
				UpdateHighScore ();
			};

			selectLeaderBoardButton.TouchUpInside += (sender, e) => selectCategory.ShowInView (this.View);

			showLeaderboardButton.TouchUpInside += (sender, e) => {
				var leaderboardController = new GKLeaderboardViewController ();
				leaderboardController.Category = currentCategory;
				leaderboardController.TimeScope = GKLeaderboardTimeScope.AllTime;
				leaderboardController.DidFinish += (senderLeaderboard, eLeaderboard) => leaderboardController.DismissViewController (true, null);
				PresentViewController (leaderboardController, true, null);
			};

			showAchievementButton.TouchUpInside += (sender, e) => {
				var achievementController = new GKAchievementViewController ();
				achievementController.DidFinish += (senderAchievement, eAchievement) => achievementController.DismissViewController (true, null);
				PresentViewController (achievementController, true, null);
			};

			incrementScoreButton.TouchUpInside += (sender, e) => {
				currentScore++;
				currentScoreTextField.Text = currentScore.ToString ();
				CheckAchievements ();
			};

			submitScoreButton.TouchUpInside += (sender, e) => {
				if (currentScore > 0)
					gameCenterManager.ReportScore (currentScore, currentCategory, this);
			};

			resetButton.TouchUpInside += (sender, e) => gameCenterManager.ResetAchievement ();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		void InitGameCenter ()
		{
			gameCenterManager = new GameCenterManager ();
			SetAuthenticateHandler ();
		}

		void SetAuthenticateHandler ()
		{
			GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) => {
				if (ui != null) {
					PresentViewController (ui, true, null);
				} else if (GKLocalPlayer.LocalPlayer.Authenticated) {
					currentLeaderBoard = gameCenterManager.ReloadLeaderboard (currentCategory);
					UpdateHighScore ();
				} else {
					var alert = new UIAlertView ("Game Center Account Required", "Need login the game center!", null, "Retry", null);
					alert.Clicked += (sender, e) => {
						//GKLocalPlayer.LocalPlayer.Authenticated();
					};
					alert.Show ();
				}
			};
		}

		public void UpdateHighScore ()
		{
			currentLeaderBoard.LoadScores ((scoreArray, error) => {
				if (error == null) {
					long personalBest;
					if (currentLeaderBoard.LocalPlayerScore != null)
						personalBest = currentLeaderBoard.LocalPlayerScore.Value;
					else
						personalBest = 0;
					playerBestScoreTextField.Text = personalBest.ToString ();
					Console.WriteLine (currentLeaderBoard.Title);

					var scores = currentLeaderBoard.Scores;
					if (scores != null && scores.Length > 0)
						globalHighestScoreTextField.Text = currentLeaderBoard.Scores [0].Value.ToString ();
				} else {
					playerBestScoreTextField.Text = "Unavailable";
					globalHighestScoreTextField.Text = "Unavailable";
				}
			});
		}

		void CheckAchievements ()
		{
			string identifier = null;
			string achievementName = null;
			double percentComplete = 0;
			switch (currentScore) {
			case 1:
				identifier = AchievementGotOneTap;
				percentComplete = 100.0;
				achievementName = AchievementNameGotOneTap;
				break;
			case 10:
				identifier = AchievementHidden20Taps;
				percentComplete = 50.0;
				achievementName = AchievementNameHidden20Taps;
				break;
			case 20:
				identifier = AchievementHidden20Taps;
				percentComplete = 100.0;
				achievementName = AchievementNameHidden20Taps;
				break;
			case 50:
				identifier = AchievementBigOneHundred;
				percentComplete = 50.0;
				achievementName = AchievementNameHidden20Taps;
				break;
			case 75:
				identifier = AchievementBigOneHundred;
				percentComplete = 75.0;
				achievementName = AchievementNameBigOneHundred;
				break;
			case 100:
				identifier = AchievementBigOneHundred;
				percentComplete = 100.0;
				achievementName = AchievementNameBigOneHundred;
				break;
			}

			if (identifier != null)
				gameCenterManager.SubmitAchievement (identifier, percentComplete, achievementName);
		}
	}
}