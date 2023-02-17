
namespace XamarinShot.Models {
	using Foundation;
	using SceneKit;
	using XamarinShot.Models.Enums;
	using XamarinShot.Utils;
	using System;
	using System.Linq;

	public class VictoryInteraction : IInteraction {
		private const double TimeUntilPhysicsReleased = 10d;

		private const double FadeTime = 0.5d;

		private readonly NSLock @lock = new NSLock (); // need thread protection because main thread is the one that called didWin()

		private readonly SCNNode victoryNode;

		private Team teamWon = Team.None;

		private double activationStartTime;

		public VictoryInteraction (IInteractionDelegate @delegate)
		{
			this.Delegate = @delegate;
			this.victoryNode = SCNNodeExtensions.LoadSCNAsset ("victory");
		}

		public IInteractionDelegate Delegate { get; private set; }

		public bool DisplayedVictory { get; private set; }

		public bool GameDone { get; private set; }

		public void ActivateVictory ()
		{
			if (!this.DisplayedVictory) {
				this.@lock.Lock ();

				if (this.Delegate == null) {
					throw new Exception ("No Delegate");
				}

				this.Delegate.AddNodeToLevel (this.victoryNode);
				this.victoryNode.WorldPosition = new SCNVector3 (0f, 15f, 0f);

				var eulerAnglesY = teamWon == Team.TeamA ? (float) Math.PI : 0f; // Rotate Victory to face in the right direction
				this.victoryNode.EulerAngles = new SCNVector3 (this.victoryNode.EulerAngles.X, eulerAnglesY, this.victoryNode.EulerAngles.Z);
				foreach (var child in this.victoryNode.ChildNodes) {
					child.PhysicsBody?.ResetTransform ();
				}

				this.DisplayedVictory = true;
				this.activationStartTime = GameTime.Time;

				// Set color to that of winning team
				this.victoryNode.SetPaintColors (this.teamWon);

				this.Delegate.PlayWinSound ();

				this.@lock.Unlock ();
			}
		}

		public void Update (CameraInfo cameraInfo)
		{
			if (this.DisplayedVictory) {
				// Enlarge victory text before falling down
				this.victoryNode.Opacity = (float) (DigitExtensions.Clamp ((GameTime.Time - this.activationStartTime) / FadeTime, 0d, 1d));

				if (GameTime.Time - this.activationStartTime > TimeUntilPhysicsReleased) {
					foreach (var child in this.victoryNode.ChildNodes) {
						if (child.PhysicsBody != null) {
							child.PhysicsBody.VelocityFactor = SCNVector3.One;
							child.PhysicsBody.AngularVelocityFactor = SCNVector3.One;
						}
					}
				}
			} else {
				// Update win condition
				if (this.DidWin () && this.teamWon != Team.None) {
					this.ActivateVictory ();
				}
			}
		}

		public void HandleTouch (TouchType type, Ray camera) { }

		private bool DidWin ()
		{
			if (this.Delegate == null) {
				throw new Exception ("No Delegate");
			}

			var catapults = this.Delegate.Catapults;

			var teamToCatapultCount = new int [] { 0, 0, 0 };
			foreach (var catapult in catapults.Where (catapult => !catapult.Disabled)) {
				teamToCatapultCount [(int) catapult.Team] += 1;
			}

			var gameDone = true;
			if (teamToCatapultCount [1] == 0 && teamToCatapultCount [2] == 0) {
				this.teamWon = Team.None;
			} else if (teamToCatapultCount [1] == 0) {
				this.teamWon = Team.TeamB;
			} else if (teamToCatapultCount [2] == 0) {
				this.teamWon = Team.TeamA;
			} else {
				gameDone = false;
			}

			return gameDone;
		}

		public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }

		public void Handle (GameActionType gameAction, Player player) { }
	}
}
