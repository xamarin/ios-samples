using System;
using System.Drawing;
using Foundation;
using UIKit;
using GameKit;

namespace MTGKTapper
{
	public partial class MTGKTapperViewController : UIViewController
	{
		public const string EasyLeaderboardID = "com.appledts.EasyTapList";
		public const string HardLeaderboardID = "com.appledts.HardTapList";
		public const string AwesomeLeaderboardID = "com.appledts.AwesomeTapList";

		public const string AchievementGotOneTap = "com.appletest.one_tap";
		public const string AchievementHidden20Taps = "com.appledts.twenty_taps";
		public const string AchievementBigOneHundred = "com.appledts.one_hundred_taps";

		public const string AchievementNameGotOneTap = "Just One Tap";
		public const string AchievementNameHidden20Taps = "Twenty Taps In";
		public const string AchievementNameBigOneHundred = "The Big One Hundred";

		GameCenterManager gameCenterManager;
		GKLeaderboard currentLeaderBoard;
		string currentCategory = EasyLeaderboardID;
		long currentScore = 0;

		public MTGKTapperViewController () : base ("MTGKTapperViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			
			// Perform any additional setup after loading the view, typically from a nib.
			initGameCenter ();

			string[] categories = {"Easy","Hard","Awesome"};
			UIActionSheet selectCategory = new UIActionSheet("Choose Leaderboard",null,"Cancel",null,categories);
			selectCategory.Dismissed += (sender, e) => {
				switch (e.ButtonIndex)
				{
				case 0:
					currentCategory = EasyLeaderboardID;
					this.selectLeaderBoardButton.SetTitle("Leaderboard: Easy",UIControlState.Normal);
					currentScore = 0;
					currentScoreTextField.Text = "0";
					break;
				case 1:
					currentCategory = HardLeaderboardID;
					this.selectLeaderBoardButton.SetTitle("Leaderboard: Hard",UIControlState.Normal);
					currentScore = 0;
					currentScoreTextField.Text = "0";
					break;
				case 2:
					currentCategory = AwesomeLeaderboardID;
					this.selectLeaderBoardButton.SetTitle("Leaderboard: Awesome",UIControlState.Normal);
					currentScore = 0;
					currentScoreTextField.Text = "0";
					break;
				default:
					break;
				}
				currentLeaderBoard = gameCenterManager.reloadLeaderboard(currentCategory);
				updateHighScore();
			};

			this.selectLeaderBoardButton.TouchUpInside += (sender, e) => {
				selectCategory.ShowInView(this.View);
			};

			this.showLeaderboardButton.TouchUpInside += (sender, e) => {
				GKLeaderboardViewController leaderboardController = new GKLeaderboardViewController();
				leaderboardController.Category = currentCategory;
				leaderboardController.TimeScope = GKLeaderboardTimeScope.AllTime;
				leaderboardController.DidFinish += (senderLeaderboard, eLeaderboard) => {
					leaderboardController.DismissViewController(true, null);
				};
				PresentViewController(leaderboardController, true,null);
			};

			this.showAchievementButton.TouchUpInside += (sender, e) => {
				GKAchievementViewController achievementController = new GKAchievementViewController();
				achievementController.DidFinish += (senderAchievement, eAchievement) => {
					achievementController.DismissViewController(true, null);
				};
				PresentViewController(achievementController, true, null);
			};

			this.incrementScoreButton.TouchUpInside += (sender, e) => {
				currentScore++;
				currentScoreTextField.Text = currentScore.ToString();
				checkAchievements();
			};

			this.submitScoreButton.TouchUpInside += (sender, e) => {
				if(currentScore >0)
					gameCenterManager.reportScore(currentScore,currentCategory,this);
			};

			this.resetButton.TouchUpInside += (sender, e) => {
				gameCenterManager.resetAchievement();
			};
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}


		void initGameCenter()
		{
			if (GameCenterManager.isGameCenterAvailable ()) {
				this.gameCenterManager = new GameCenterManager ();
				setAuthenticateHandler ();
			} else {
				new UIAlertView ("Game Center Support Required", "The current device does not support Game Center, which this sample requires.", null, "OK", null).Show ();
			}
		}

		void setAuthenticateHandler()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) => 
				{
					if (ui != null) {
						this.PresentViewController (ui, true, null);
					} 
					else if (GKLocalPlayer.LocalPlayer.Authenticated) {
						currentLeaderBoard = gameCenterManager.reloadLeaderboard (currentCategory);
						updateHighScore ();
					} else {
						var alert = new UIAlertView ("Game Center Account Required", "Need login the game center!", null, "Retry", null);
						alert.Clicked += (sender, e) => {
							//GKLocalPlayer.LocalPlayer.Authenticated();
						};
						alert.Show ();
					}
				};
			} 
			else 
			{
				GKLocalPlayer.LocalPlayer.Authenticate(new GKNotificationHandler(delegate(NSError error) {
					if (GKLocalPlayer.LocalPlayer.Authenticated) {
						currentLeaderBoard = gameCenterManager.reloadLeaderboard (currentCategory);
						updateHighScore ();
					}
					else
					{
						var alert = new UIAlertView ("Game Center Account Required", "Need login the game center!", null, "Retry", null);
						alert.Clicked += (sender, e) => {
							setAuthenticateHandler();
						};
						alert.Show ();

					}
				}));
			}
		}

		public void updateHighScore()
		{
			currentLeaderBoard.LoadScores (new GKScoresLoadedHandler (delegate(GKScore[] scoreArray, NSError error) {
				if(error == null){
					long personalBest;
					if(currentLeaderBoard.LocalPlayerScore != null)
						personalBest = currentLeaderBoard.LocalPlayerScore.Value;
					else
						personalBest = 0;
					playerBestScoreTextField.Text = personalBest.ToString ();
					if (currentLeaderBoard.Scores.Length > 0) {
						globalHighestScoreTextField.Text = currentLeaderBoard.Scores [0].Value.ToString ();
					}
				}
				else{
					playerBestScoreTextField.Text = "Unavailable";
					globalHighestScoreTextField.Text = "Unavailable";
				}
			}));
		}

		void checkAchievements()
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
			if (identifier != null) {
				gameCenterManager.submitAchievement (identifier, percentComplete, achievementName);
			}
		}
	}
}

