using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.GameKit;

namespace GameCenterSample
{
	public partial class MainViewController : UIViewController
	{
		public PlayerModel player;
		int achievementsPercentageComplete = 0;

		public MainViewController () : base ("MainViewController", null)
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
			player.resetAchievements ();
		}

		void submitAchievementHandleTouchUpInside (object sender, EventArgs e)
		{
			//Create the achievement we want to submit.
			NSString identifier = new NSString ("com.appledts.GameCenterSampleApps.achievement");

			GKAchievement achievement = new GKAchievement ("com.appledts.GameCenterSampleApps.achievement");

			achievementsPercentageComplete += 25;
			achievement.PercentComplete = achievementsPercentageComplete;

			player.submitAchievement (achievement);
		}



		void submitScoreHandleTouchUpInside (object sender, EventArgs e)
		{
			GKScore submitScore = new GKScore ("leaderboard");
			submitScore.Init ();
			submitScore.Value = Convert.ToInt64(this.scoreTextField.Text);

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

