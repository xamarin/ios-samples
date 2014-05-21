using System;
using CoreGraphics;
using System.Threading.Tasks;
using CoreText;
using Foundation;
using GameKit;
using SpriteKit;
using UIKit;

namespace ButtonTapper3000 {

	public class MainGame : BasicScene {

		public const string Prefix = "com.xamarin.sample.gamekitsamplewwdc2013.";
		const string TapOnceId = Prefix + "taponce";
		const string PlayHardId = Prefix + "playhardmode";
		const string PlayAllId = Prefix + "playallgametypes";
		const string AverageTapTimeId = Prefix + "averagetaptime";
		const string Tap100Id = Prefix + "tapahundred";
		const string Tap1000Id = Prefix + "tapathousand";

		SKLabelNode button;
		SKLabelNode timerLabel;
		SKLabelNode clicksLabel;
		NSTimer gameTimer;
		NSTimer tickTimer;

		Random rand;

		public MainGame (CGSize size) : base (size)
		{
			rand = new Random ();

			GameInfo.CurrentTaps = 0;

			gameTimer = NSTimer.CreateScheduledTimer (GameInfo.GameTimeInSeconds, async delegate {
				await TimerDone ();
			});

			tickTimer = NSTimer.CreateRepeatingScheduledTimer (1.0, async delegate {
				GameInfo.CurrentTicks --;
				if (GameInfo.CurrentTicks < 0) {
					GameInfo.CurrentTicks = 0;
					await TimerDone ();
				}
			});

			button = new SKLabelNode ("GillSans-Bold") {
				Text = "Tap Me!",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY)
			};

			clicksLabel = new SKLabelNode ("AvenirNext-Bold") {
				Text = GameInfo.CurrentTaps.ToString (),
				FontSize = 45,
				Position = new CGPoint (FrameMidX, FrameMidY - 120)
			};

			timerLabel = new SKLabelNode ("HelveticaNeue-CondensedBlack") {
				Text = GameInfo.CurrentTicks.ToString (),
				FontSize = 45,
				Position = new CGPoint (FrameMidX, FrameMidY + 120)
			};

			AddChild (button);
			AddChild (clicksLabel);
			AddChild (timerLabel);
		}

		public override async void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				var location = (touch as UITouch).LocationInNode (this);

				if (button.ContainsPoint (location)) {
					GameInfo.CurrentTaps ++;

					if (GameInfo.GameMode == GameMode.Hard) {
						int x = rand.Next (100) - 50;
						int y = rand.Next (100) - 50;
						button.Position = new CGPoint (FrameMidX + x, FrameMidY + y);
					}
				}
			}

			if (GKLocalPlayer.LocalPlayer.Authenticated) {
				var tapOnceAchievement = new GKAchievement (TapOnceId, GKLocalPlayer.LocalPlayer.PlayerID) {
					PercentComplete = 100
				};
				await GKAchievement.ReportAchievementsAsync (new [] { tapOnceAchievement }, null);
			}
		}

		public override void Update (double currentTime)
		{
			clicksLabel.Text = GameInfo.CurrentTaps.ToString ();
			timerLabel.Text = GameInfo.CurrentTicks.ToString ();
		}

		async Task TimerDone ()
		{
			tickTimer.Invalidate ();
			await ReportScore ();
			PresentScene (new ResultsScreen (View.Bounds.Size));
		}

		async Task ReportScore ()
		{
			string leaderboardIdentifier = null;
			string gameTypeString = null;
			GameTypePlayed gameType = GameTypePlayed.Invalid;

			if (GameInfo.GameTime == GameTime.Fifteen) {
				if (GameInfo.GameMode == GameMode.Easy) {
					gameTypeString = "15secondseasymode";	
					gameType = GameTypePlayed.Easy15;
				} else if (GameInfo.GameMode == GameMode.Hard) {
					gameTypeString = "15secondshardmode";
					gameType = GameTypePlayed.Hard15;
				}
			} else if (GameInfo.GameTime == GameTime.Thirty) {
				if (GameInfo.GameMode == GameMode.Easy) {
					gameTypeString = "30secondseasymode";
					gameType = GameTypePlayed.Easy30;
				} else if (GameInfo.GameMode == GameMode.Hard) {
					gameTypeString = "30secondshardmode";
					gameType = GameTypePlayed.Hard30;
				}
			} else if (GameInfo.GameTime == GameTime.FourtyFive) {
				if (GameInfo.GameMode == GameMode.Easy) {
					gameTypeString = "45secondseasymode";
					gameType = GameTypePlayed.Easy45;
				} else if (GameInfo.GameMode == GameMode.Hard) {
					gameTypeString = "45secondshardmode";
					gameType = GameTypePlayed.Hard45;
				}
			}

			if (gameTypeString != null)
				leaderboardIdentifier = Prefix + gameTypeString;

			if (leaderboardIdentifier != null) {
				GKScore score = new GKScore (leaderboardIdentifier) {
					Value = GameInfo.CurrentTaps,
					Context = 0
				};
				var challenges = GameInfo.Challenge == null ? null : new [] { GameInfo.Challenge };
				await GKScore.ReportScoresAsync (new [] { score }, challenges);
			}

			if (GKLocalPlayer.LocalPlayer.Authenticated) {
				if (GameInfo.GameMode == GameMode.Hard) {
					var playhard = new GKAchievement (PlayHardId, GKLocalPlayer.LocalPlayer.PlayerID) {
						PercentComplete = 100
					};
					await GKAchievement.ReportAchievementsAsync (new [] { playhard });
				}

				int playedGameTypesBitField;
				using (NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults) {
					playedGameTypesBitField = (int) defaults.IntForKey ("playedGameTypes") | (int) gameType;
					defaults.SetInt (playedGameTypesBitField, "playedGameTypes");
					defaults.Synchronize ();
				}

				int numTypesPlayed = 0;
				for (int i = 0; i < 6; i++) {
					if ((playedGameTypesBitField & 0x01) != 0)
						numTypesPlayed++;
					playedGameTypesBitField >>= 1;
				}

				GKAchievement playAllModesAchievement = new GKAchievement (PlayAllId) {
					PercentComplete = numTypesPlayed / 6.0 * 100.0
				};
				await GKAchievement.ReportAchievementsAsync (new [] { playAllModesAchievement });

				await UpdateCurrentTapsLeaderboardAndTapAchievements ();
			}
		}

		async Task UpdateCurrentTapsLeaderboardAndTapAchievements ()
		{
			GKLeaderboard averageTapLeaderboard = new GKLeaderboard (new [] { GKLocalPlayer.LocalPlayer.PlayerID }) {
				Identifier = AverageTapTimeId
			};
			var scores = await averageTapLeaderboard.LoadScoresAsync ();

			GKScore currentScore;
			GKScore newScore = new GKScore (AverageTapTimeId);

			if (scores != null && scores.Length > 1) {
				currentScore = scores [0];
				int oldTaps = (int)currentScore.Context;
				int oldTime = (int)currentScore.Value * oldTaps;

				int newTime = oldTime + GameInfo.GameTimeInSeconds * 100;
				int newTaps = oldTaps + GameInfo.CurrentTaps;

				newScore.Value = newTime / newTaps;
				newScore.Context = (ulong)newTaps;
			} else {
				newScore.Value = GameInfo.GameTimeInSeconds / Math.Max (GameInfo.CurrentTaps, 1) * 100;
				newScore.Context = (ulong)GameInfo.CurrentTaps;
			}

			GKAchievement playAHundred = new GKAchievement (Tap100Id, GKLocalPlayer.LocalPlayer.PlayerID) {
				PercentComplete = (float) newScore.Context / 100f * 100f
			};

			GKAchievement playAThousand = new GKAchievement (Tap1000Id, GKLocalPlayer.LocalPlayer.PlayerID) {
				PercentComplete = (float) newScore.Context / 1000f * 100f
			};

			await GKAchievement.ReportAchievementsAsync (new [] { playAHundred, playAThousand });

			await GKScore.ReportScoresAsync (new [] { newScore });
		}
	}
}