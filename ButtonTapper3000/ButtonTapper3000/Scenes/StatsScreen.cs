using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.GameKit;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;

namespace ButtonTapper3000 {

	public class StatsScreen : BasicScene {

		SKLabelNode leaderboardsButton;
		SKLabelNode achievementsButton;
		SKLabelNode backButton;

		public StatsScreen (SizeF size) : base (size)
		{
			BackgroundColor = UIColor.FromRGBA (0.15f, 0.15f, 0.3f, 1f);

			var title = new SKLabelNode ("GillSans-Bold") {
				Text = "Stats",
				FontSize = 30,
				Position = new PointF (FrameMidX, FrameMidY + 60)
			};

			leaderboardsButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Leaderboards",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY)
			};

			achievementsButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Achievements",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 60)
			};

			backButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Back",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 200)
			};

			AddChild (title);
			AddChild (leaderboardsButton);
			AddChild (achievementsButton);
			AddChild (backButton);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			 foreach (var touch in touches) {
				var location = (touch as UITouch).LocationInNode (this);

				if (leaderboardsButton.ContainsPoint (location)) {
					PresentScene (new LeaderboardSetsScreen (View.Bounds.Size));
				} else if (achievementsButton.ContainsPoint (location)) {
					PresentScene (new AchievementsScreen (View.Bounds.Size));
				} else if (backButton.ContainsPoint (location)) {
					PresentScene (new MainMenu (View.Bounds.Size));
				}
			}
		}
	}

	public class AchievementsScreen : BasicScene {

		SKLabelNode backButton;

		public AchievementsScreen (SizeF size) : base (size)
		{
			var title = new SKLabelNode ("GillSans-Bold") {
				Text = "Achievements",
				FontSize = 30,
				Position = new PointF (FrameMidX, FrameMidY + 190)
			};

			backButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Back",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 200)
			};

			var incompleteLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Incomplete",
				FontSize = 18,
				Position = new PointF (FrameMidX + 75, FrameMidY + 150)
			};

			var completeLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Complete",
				FontSize = 18,
				Position = new PointF (FrameMidX - 75, FrameMidY + 150)
			};

			if (GKLocalPlayer.LocalPlayer.Authenticated)
				GKAchievementDescription.LoadAchievementDescriptions (LoadAchievementInfo);

			AddChild (title);
			AddChild (incompleteLabel);
			AddChild (completeLabel);
			AddChild (backButton);
		}

		void LoadAchievementInfo (GKAchievementDescription[] descriptions, NSError error)
		{
			if (descriptions == null)
				return;

			GKAchievement.LoadAchievements (delegate (GKAchievement[] achievements, NSError err) {
				int completeOffset = 0;
				int incompleteOffset = 0;

				foreach (var description in descriptions) {
					bool completed = false;
					foreach (var achievement in achievements) {
						if (description.Identifier == achievement.Identifier)
							completed |= achievement.Completed;
					}

					int xOffset = completed ? -75 : 75;
					int yOffset = completed ? completeOffset : incompleteOffset;

					var achievementLabel = new SKLabelNode ("GillSans-Bold") {
						Text = description.Title,
						FontSize = 10,
						Position = new PointF (FrameMidX + xOffset, FrameMidY + 50 + yOffset + 25)
					};
					AddChild (achievementLabel);

					description.LoadImage (delegate (UIImage image, NSError imageError) {
						if (image == null)
							image = UIImage.FromFile ("Images/DefaultPlayerPhoto.png");
						var sprite = SKSpriteNode.FromTexture (SKTexture.FromImage (image), new SizeF (32, 32));
						sprite.Position = new PointF (FrameMidX + xOffset, FrameMidY + 50 + yOffset + 50);
						AddChild (sprite);
					});

					if (completed)
						completeOffset -= 50;
					else
						incompleteOffset -= 50;
				}
			});
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				PointF location = (touch as UITouch).LocationInNode (this);
				if (backButton.ContainsPoint (location))
					PresentScene (new StatsScreen (new SizeF (View.Bounds.Size)));
			}
		}
	}

	public class LeaderboardSetsScreen : BasicScene {
		SKLabelNode backButton;
		GKLeaderboardSet[] leaderboardSets;
		List<SKLabelNode> leaderboardSetButtons;

		public LeaderboardSetsScreen (SizeF size) : base (size)
		{
			leaderboardSetButtons = new List<SKLabelNode> ();

			SKLabelNode title = new SKLabelNode ("GillSans-Bold") {
				Text = "Leaderboards Sets",
				FontSize = 30,
				Position = new PointF (FrameMidX, FrameMidY + 190)
			};

			backButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Back",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 200)
			};

			if (GKLocalPlayer.LocalPlayer.Authenticated)
				GKLeaderboardSet.LoadLeaderboardSets (LoadLeaderboardSets);

			AddChild (title);
			AddChild (backButton);
		}

		void LoadLeaderboardSets (GKLeaderboardSet[] leaderboardSets, NSError error)
		{
			this.leaderboardSets = leaderboardSets;
			if (leaderboardSets == null)
				return;

			int offset = 0;
			foreach (var leaderboardSet in leaderboardSets) {
				var leaderboardSetButton = new SKLabelNode ("GillSans-Bold") {
					Text = leaderboardSet.Title,
					FontSize = 18,
					Position = new PointF (FrameMidX, FrameMidY + 125 - offset)
				};
				offset += 50;

				AddChild (leaderboardSetButton);
				leaderboardSetButtons.Add (leaderboardSetButton);
			}
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				PointF location = (touch as UITouch).LocationInNode (this);

				if (backButton.ContainsPoint (location)) {
					PresentScene (new StatsScreen (View.Bounds.Size));
					break;
				}

				for (int i = 0; i < leaderboardSetButtons.Count; i++) {
					var button = leaderboardSetButtons [i];

					if (button.ContainsPoint (location)) {
						GameInfo.CurrentSet = leaderboardSets [i];
						PresentScene (new LeaderboardsScreen (View.Bounds.Size));
						break;
					}
				}
			}
		}
	}

	public class LeaderboardsScreen : BasicScene {

		SKLabelNode backButton;
		GKLeaderboard [] leaderboards;
		List<SKLabelNode> leaderboardButtons;

		public LeaderboardsScreen (SizeF size) : base (size)
		{
			leaderboardButtons = new List<SKLabelNode> ();

			var title = new SKLabelNode ("GillSans-Bold") {
				Text = "Leaderboards",
				FontSize = 30,
				Position = new PointF (FrameMidX, FrameMidY + 190)
			};

			backButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Back",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 200)
			};

			if (GKLocalPlayer.LocalPlayer.Authenticated)
				GameInfo.CurrentSet.LoadLeaderboards (LoadLeaderboard);

			AddChild (title);
			AddChild (backButton);
		}

		void LoadLeaderboard (GKLeaderboard[] leaderboards, NSError error)
		{
			this.leaderboards = leaderboards;
			int offset = 0;
			foreach (var leaderboard in leaderboards) {
				SKLabelNode leaderboardButton = new SKLabelNode ("GillSans-Bold") {
					Text = leaderboard.Title,
					FontSize = 18,
					Position = new PointF (FrameMidX, FrameMidY + 125 - offset)
				};
				offset += 50;

				AddChild (leaderboardButton);
				leaderboardButtons.Add (leaderboardButton);
			}
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				PointF location = (touch as UITouch).LocationInNode (this);

				if (backButton.ContainsPoint (location)) {
					PresentScene (new StatsScreen (View.Bounds.Size));
					break;
				}

				for (int i = 0; i < leaderboardButtons.Count; i++) {
					var button = leaderboardButtons [i];

					if (button.ContainsPoint (location)) {
						GameInfo.CurrentLeaderBoard = leaderboards [i];
						PresentScene (new LeaderboardScoresScreen (View.Bounds.Size));
						break;
					}
				}
			}
		}
	}

	public class LeaderboardScoresScreen : BasicScene {

		SKLabelNode backButton;

		public LeaderboardScoresScreen (SizeF size) : base (size)
		{
			SKLabelNode title = new SKLabelNode ("GillSans-Bold") {
				Text = GameInfo.CurrentLeaderBoard.Title,
				FontSize = 14,
				Position = new PointF (FrameMidX, FrameMidY + 190)
			};

			var podiumSprite = SKSpriteNode.FromTexture (SKTexture.FromImageNamed ("Images/Podium.png"));
			podiumSprite.Position = new PointF (FrameMidX, FrameMidY + 50);

			backButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Back",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 200)
			};

			if (GKLocalPlayer.LocalPlayer.Authenticated)
				LoadLeaderboardScoresInfo (GameInfo.CurrentLeaderBoard);

			AddChild (title);
			AddChild (backButton);
			AddChild (podiumSprite);
		}

		void DisplayScore (GKScore score, int rank, GKPlayer player)
		{
			PointF[] podiumPositions = new PointF[] {
				new PointF (0, 100),
				new PointF (-84, 75),
				new PointF (84, 50)
			};

			PointF currentPoint = podiumPositions [rank];

			SKLabelNode scoreLabel = new SKLabelNode ("GillSans-Bold") {
				Text = score.FormattedValue,
				FontSize = 14,
				Position = new PointF (FrameMidX + currentPoint.X, FrameMidY + currentPoint.Y - 32)
			};

			player.LoadPhoto (GKPhotoSize.Small, delegate (UIImage photo, NSError error) {
				if (photo == null)
					photo = UIImage.FromFile ("Images/DefaultPlayerPhoto.png");
				var image = SKSpriteNode.FromTexture (SKTexture.FromImage (photo), new SizeF (32, 32));
				image.Position = new PointF (FrameMidX + currentPoint.X, FrameMidY + currentPoint.Y + 16);
				AddChild (image);
			});

			AddChild (scoreLabel);
		}

		async Task LoadLeaderboardScoresInfo (GKLeaderboard leaderboard)
		{
			leaderboard.Range = new NSRange (1, 3);
			leaderboard.TimeScope = GKLeaderboardTimeScope.AllTime;
			leaderboard.PlayerScope = GKLeaderboardPlayerScope.Global;
			var scores = await leaderboard.LoadScoresAsync ();

			string [] identifiers = new string [scores.Length];
			int n = 0;
			foreach (var score in scores)
				identifiers [n++] = score.Player;

			GKPlayer.LoadPlayersForIdentifiers (identifiers, delegate (GKPlayer[] players, NSError error) {
				for (int i = 0; i < scores.Length; i++)
					DisplayScore (scores [i], i, players [i]);
			});
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				PointF location = (touch as UITouch).LocationInNode (this);
				if (backButton.ContainsPoint (location))
					PresentScene (new LeaderboardsScreen (View.Bounds.Size));
			}
		}
	}
}