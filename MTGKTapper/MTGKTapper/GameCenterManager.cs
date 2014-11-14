using System;
using Foundation;
using GameKit;
using UIKit;

namespace MTGKTapper
{
	public class GameCenterManager
	{

		NSMutableDictionary earnedAchievementCache;

		public GameCenterManager ()
		{
		}

		public static bool isGameCenterAvailable()
		{

			return UIDevice.CurrentDevice.CheckSystemVersion (4, 1);
		}

		public GKLeaderboard reloadLeaderboard(string category)
		{
			GKLeaderboard leaderboard = new GKLeaderboard ();
			leaderboard.Category = category;
			leaderboard.TimeScope = GKLeaderboardTimeScope.AllTime;
			leaderboard.Range = new NSRange (1, 1);
			return leaderboard;

		}

		public void reportScore(long score, string category, MTGKTapperViewController controller)
		{
			GKScore scoreReporter = new GKScore (category);
			scoreReporter.Value = score;
			scoreReporter.ReportScore (new GKNotificationHandler ((error) => {
				if(error == null){
					new UIAlertView ("Score reported", "Score Reported successfully", null, "OK", null).Show ();
				}
				else{
					new UIAlertView ("Score Reported Failed", "Score Reported Failed", null, "OK", null).Show ();
				}
				NSThread.SleepFor(1);
				controller.updateHighScore();
			}));
		}


		public void submitAchievement(string identifier, double percentComplete, string achievementName)
		{
			if(earnedAchievementCache == null)
			{
				GKAchievement.LoadAchievements (new GKCompletionHandler (delegate(GKAchievement[] achievements, NSError error) {
					NSMutableDictionary tempCache = new NSMutableDictionary();
					if(achievements !=null)
					{
						foreach(var achievement in achievements)
						{
							tempCache.Add(new NSString(achievement.Identifier), achievement);
						}
					}
					earnedAchievementCache = tempCache;
					submitAchievement(identifier,percentComplete,achievementName);
				}));
			}
			else
			{
				GKAchievement achievement =(GKAchievement) earnedAchievementCache.ValueForKey (new NSString(identifier));
				if (achievement != null)
				{

					if (achievement.PercentComplete >= 100.0 || achievement.PercentComplete >= percentComplete) 
					{
						achievement = null;
					}
					else
						achievement.PercentComplete = percentComplete;
				}
				else
				{
					achievement = new GKAchievement(identifier);
					achievement.PercentComplete = percentComplete;
					earnedAchievementCache.Add(new NSString(achievement.Identifier),achievement);
				}
				if(achievement != null)
				{
					achievement.ReportAchievement (new GKNotificationHandler (delegate(NSError error) {
						if(error == null && achievement != null)
						{
							if(percentComplete == 100)
							{
								new UIAlertView ("Achievement Earned", "Great job!  You earned an achievement: " + achievementName, null, "OK", null).Show ();
							}
							else if(percentComplete >0)
							{
								new UIAlertView ("Achievement Progress", "Great job!  You're "+percentComplete+" % of the way to " + achievementName, null, "OK", null).Show ();
							}
						}
						else
						{
							new UIAlertView ("Achievement submittion failed", "Submittion failed because: " + error, null, "OK", null).Show ();
						}
					}));
				}
			}
		}

		public void resetAchievement()
		{
			earnedAchievementCache = null;
			GKAchievement.ResetAchivements (new GKNotificationHandler (delegate(NSError error) {
				if(error == null)
					new UIAlertView ("Achievement reset", "Achievement reset successfully", null, "OK", null).Show ();
				else
					new UIAlertView ("Reset failed", "Reset failed because: " + error, null, "OK", null).Show ();
			}));
		}

	}
}

