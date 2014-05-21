using System;
using CoreGraphics;
using Foundation;
using GameKit;
using SpriteKit;
using UIKit;

namespace ButtonTapper3000 {

	public class MainMenu : BasicScene {

		SKLabelNode startButton;
		SKLabelNode gameCenterButton;
		SKLabelNode gameStatsButton;
		SKLabelNode playChallengeButton;
		SKLabelNode challengeFriendsButton;

		public MainMenu (CGSize size) : base (size)
		{
			var title = new SKLabelNode ("GillSans-Bold") {
				Text = "Button Tapper",
				FontSize = 30f,
				Position = new CGPoint (FrameMidX, FrameMidY + 60)
			};

			startButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Start Game",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY)
			};

			gameCenterButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Game Center",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY - 60)
			};

			gameStatsButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Game Stats",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY - 120)
			};

			challengeFriendsButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Challenge Friends",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY - 180)
			};

			playChallengeButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Play Challenge",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY - 240)
			};

			SetupChallenge ();

			AddChild (title);
			AddChild (startButton);
			AddChild (gameCenterButton);
			AddChild (gameStatsButton);
			AddChild (challengeFriendsButton);
			AddChild (playChallengeButton);
		}

		async void SetupChallenge ()
		{
			playChallengeButton.Hidden = true;
			if (!GKLocalPlayer.LocalPlayer.Authenticated)
				return;

			var challenges = await GKChallenge.LoadReceivedChallengesAsync ();
			if (challenges != null) {
				foreach (var challenge in challenges) {
					var c = challenge as GKScoreChallenge;
					if (c != null) {
						SelectChallenge (c);
						break;
					}
				}
			}

			var listener = new PlayerListener ();
			listener.DidReceiveChallengeAction = (player, challenge) => {
				if (player == GKLocalPlayer.LocalPlayer) {
					var c = challenge as GKScoreChallenge;
					if (c != null)
						SelectChallenge (c);
				}
			};
			GKLocalPlayer.LocalPlayer.RegisterListener (listener);
		}

		void SelectChallenge (GKScoreChallenge challenge)
		{
			string leaderboardID = challenge.Score.LeaderboardIdentifier;
			string[] substrings = leaderboardID.Split ('.');
			string leaderboardSubstring = substrings [substrings.Length - 1];
			string timeString = leaderboardSubstring.Substring (0, 9);
			string modeString = leaderboardSubstring.Substring (9);

			switch (timeString) {
			case "15seconds":
				GameInfo.GameTime = GameTime.Fifteen;
				break;
			case "30seconds":
				GameInfo.GameTime = GameTime.Thirty;
				break;
			case ("45seconds"):
				GameInfo.GameTime = GameTime.FourtyFive;
				break;
			default:
				GameInfo.GameTime = GameTime.Max;
				break;
			}

			GameInfo.GameMode = modeString == "hardmode" ? GameMode.Hard : GameMode.Easy;
			GameInfo.Challenge = challenge;

			playChallengeButton.Hidden = false;
		}

		void ShowGameCenter ()
		{
			GKGameCenterViewController controller = new GKGameCenterViewController ();
			controller.Finished += (object sender, EventArgs e) => {
				controller.DismissViewController (true, null);
			};
			AppDelegate.Shared.ViewController.PresentViewController (controller, true, null);
		}

		async void ChallengeFriends ()
		{
			if (!GKLocalPlayer.LocalPlayer.Authenticated) {
				new UIAlertView ("Player not logged in", "Must be logged into Game Center to challenge friends",
					null, "Okay", null).Show ();
				return;
			}

			if (GKLocalPlayer.LocalPlayer.Friends == null)
				await GKLocalPlayer.LocalPlayer.LoadFriendsAsync ();

			GKScore score = new GKScore () {
				LeaderboardIdentifier = MainGame.Prefix + "15secondseasymode",
				Context = (ulong)GameTypePlayed.Easy15,
				Value = 10
			};

			UIViewController challengeController = score.ChallengeComposeController (
				GKLocalPlayer.LocalPlayer.Friends, "Beat it!", 
				delegate (UIViewController composeController, bool didIssueChallenge, string[] sentPlayerIDs) {
					AppDelegate.Shared.ViewController.DismissViewController (true, null);
				}
			);
			AppDelegate.Shared.ViewController.PresentViewController (challengeController, true, null);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				CGPoint location = (touch as UITouch).LocationInNode (this);
				if (startButton.Frame.Contains (location)) {
					PresentScene (new GameSetupMenu (View.Bounds.Size));
				} else if (gameCenterButton.Frame.Contains (location)) {
					ShowGameCenter ();
				} else if (gameStatsButton.Frame.Contains (location)) {
					PresentScene (new StatsScreen (View.Bounds.Size));
				} else if (challengeFriendsButton.Frame.Contains (location)) {
					ChallengeFriends ();
				} else if (playChallengeButton.Frame.Contains (location)) {
					PresentScene (new MainGame (View.Bounds.Size));
				}
			}
		}
	}

	class PlayerListener : GKLocalPlayerListener
	{
		public Action<GKPlayer, GKChallenge> DidReceiveChallengeAction;

		public override void DidReceiveChallenge (GKPlayer player, GKChallenge challenge)
		{
			var action = DidReceiveChallengeAction;
			if (action != null)
				action (player, challenge);
		}
	}
}