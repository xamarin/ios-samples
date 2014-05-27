using System;
using System.Drawing;
using Foundation;
using UIKit;
using GameKit;

namespace GameCenterSample
{
	public partial class MainViewController : UIViewController
	{

		public GKNotificationHandler authenticatedHandler;
		public PlayerModel player;
		string currentPlayerID;
		int achievementsPercentageComplete = 0;

		public MainViewController () : base ("MainViewController", null)
		{
			authenticatedHandler = new GKNotificationHandler (delegate(NSError error) {
				if (GKLocalPlayer.LocalPlayer.Authenticated) {
					//Switching Users
					if(currentPlayerID != null || currentPlayerID != GKLocalPlayer.LocalPlayer.PlayerID)
					{
						currentPlayerID = GKLocalPlayer.LocalPlayer.PlayerID;
						player = new PlayerModel();
						player.loadStoredScores();
						player.loadSotredAchievements();
					}
				} else {
					var alert = new UIAlertView ("Game Center Account Required", "Need login the game center!", null, "Retry", null);
					alert.Clicked += delegate {
						GKLocalPlayer.LocalPlayer.Authenticate (authenticatedHandler);
					};
					alert.Show ();

				}
			});
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
			this.scoreTextField.EditingDidEndOnExit += (object sender, EventArgs e) => {
				this.scoreTextField.EndEditing(true);
			};

			//Add event handler for our buttons

			this.showLeaderBoardButton.TouchUpInside += leaderBoardButtonTouchUpInsideHander;

			this.submitScoreButton.TouchUpInside += submitScoreHandleTouchUpInside;

			this.showAchievementsButton.TouchUpInside += showAchievementsHandleTouchUpInside;

			this.submitAchievementButton.TouchUpInside += submitAchievementHandleTouchUpInside;

			this.resetAchievementsButton.TouchUpInside += resetAchievementsButtonHandleTouchUpInside;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}




		void resetAchievementsButtonHandleTouchUpInside (object sender, EventArgs e)
		{
			if (!GKLocalPlayer.LocalPlayer.Authenticated) {
				new UIAlertView ("Error", "Need sign in Game Center to reset the achievement", null, "OK", null).Show();
				GKLocalPlayer.LocalPlayer.Authenticate (authenticatedHandler);
				return;
			}
			player.resetAchievements ();
		}

		void submitAchievementHandleTouchUpInside (object sender, EventArgs e)
		{
			if (!GKLocalPlayer.LocalPlayer.Authenticated) {
				new UIAlertView ("Error", "Need sign in Game Center to submit the achievement", null, "OK", null).Show();
				GKLocalPlayer.LocalPlayer.Authenticate (authenticatedHandler);
				return;
			}

			//Create the achievement we want to submit.
			NSString identifier = new NSString ("com.appledts.GameCenterSampleApps.achievement");

			GKAchievement achievement = new GKAchievement ("com.appledts.GameCenterSampleApps.achievement");

			achievementsPercentageComplete += 25;
			achievement.PercentComplete = achievementsPercentageComplete;

			player.submitAchievement (achievement);
		}



		void submitScoreHandleTouchUpInside (object sender, EventArgs e)
		{
			if (!GKLocalPlayer.LocalPlayer.Authenticated) {
				new UIAlertView ("Error", "Need sign in Game Center to submit the score", null, "OK", null).Show();
				GKLocalPlayer.LocalPlayer.Authenticate (authenticatedHandler);
				return;
			}


			GKScore submitScore = new GKScore ("leaderboard");
			submitScore.Init ();
			try{
				submitScore.Value = Convert.ToInt64(this.scoreTextField.Text);
			}
			catch{
				new UIAlertView ("Error", "Score should be a number", null, "OK", null).Show();
				return;
			}

			submitScore.ShouldSetDefaultLeaderboard = true;
			submitScore.Context = 100;
			player.submitScore (submitScore);
		}

		void leaderBoardButtonTouchUpInsideHander (object sender, EventArgs e)
		{
			showLeaderboard ();
		}

		void showLeaderboard()
		{
			GKLeaderboardViewController leaderboardViewController = new GKLeaderboardViewController ();
			leaderboardViewController.Category = "Leaderboard";
			leaderboardViewController.DidFinish += (object sender, EventArgs e) => {
				leaderboardViewController.DismissViewController(true, null);
			};
			this.PresentViewController (leaderboardViewController, true, null);
		}


		void showAchievementsHandleTouchUpInside (object sender, EventArgs e)
		{

			showAchievements ();
		}

		void showAchievements()
		{
			GKAchievementViewController achievementViewController = new GKAchievementViewController ();
			achievementViewController.DidFinish += (object sender, EventArgs e) => {
				achievementViewController.DismissViewController(true, null);
			};
			this.PresentViewController(achievementViewController, true, null);
		}


	}
}

