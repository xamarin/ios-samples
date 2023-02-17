
namespace XamarinShot.Models.GameplayState {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Foundation;
	using GameplayKit;
	using SceneKit;

	public interface IGameAudioComponentDelegate {
		void CollisionEventOccurred (GameAudioComponent component, CollisionEvent collisionEvent);
	}

	public class GameAudioComponent : GKComponent, ICollisionHandlerComponent, ITouchableComponent {
		private const double CollisionCooldown = 0.5d;

		private readonly SCNNode node;

		private readonly bool hasCollisionSounds;

		private readonly CollisionAudioSampler.Config configuration;

		private SFXCoordinator sfxCoordinator;

		private CollisionAudioSampler audioSampler;

		private DateTime lastCollisionTime = DateTime.MinValue;

		public GameAudioComponent (SCNNode node, Dictionary<string, object> properties) : base ()
		{
			this.node = node;
			this.configuration = CollisionAudioSampler.Config.Create (properties);

			if (properties.TryGetValue ("collision", out object collision) && collision is bool) {
				this.hasCollisionSounds = (bool) collision;
			}
		}

		public GameAudioComponent (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

		public IGameAudioComponentDelegate Delegate { get; set; }

		public SFXCoordinator SfxCoordinator {
			get {
				return this.sfxCoordinator;
			}

			set {
				this.sfxCoordinator = value;
				if (this.sfxCoordinator != null && this.hasCollisionSounds && this.configuration != null) {
					this.audioSampler = new CollisionAudioSampler (this.node, this.configuration, this.sfxCoordinator);
				}
			}
		}

		/// <summary>
		/// this is what is called if custom collision response is active
		/// </summary>
		public void DidCollision (GameManager manager, SCNNode node, SCNNode otherNode, SCNVector3 pos, float impulse)
		{
			// we don't play sound if this is a triggerVolume
			if (!string.IsNullOrEmpty (node.Name) && node.Name.StartsWith ("CollisionTrigger", StringComparison.Ordinal)) {
				return;
			}

			if (!string.IsNullOrEmpty (otherNode.Name) && otherNode.Name.StartsWith ("CollisionTrigger", StringComparison.Ordinal)) {
				return;
			}

			var names = new string [] { node.Name, otherNode.Name };
			var withBall = names.Contains ("ball");
			var withTable = names.Contains ("OcclusionBox");

			if (node.Name == "OcclusionBox") {
				// don't play any sounds on the table.
				return;
			}

			var effectiveImpulse = impulse;
			if (withTable) {
				// the table does not move, so the calculated impulse is zero (sometimes?).
				// Ensure that the ball has an impulse value, so if it happens to be zero,
				// fake one based on its velocity
				if (impulse == 0f && withBall) {
					if (node.PhysicsBody != null) {
						var v = node.PhysicsBody.Velocity;
						var factor = 1.5f;
						var velocity = new SCNVector3 (v.X, v.Y, v.Z).Length;
						effectiveImpulse = factor * velocity;
					} else {
						effectiveImpulse = 0;
					}
				}
			}

			this.PrepareCollisionSound (effectiveImpulse, withBall, withTable);
		}

		/// <summary>
		/// when this object collides with something else. (runs only on the server)
		/// </summary>
		public void PrepareCollisionSound (float impulse, bool withBall, bool withTable)
		{
			if (this.audioSampler != null) {
				var now = DateTime.UtcNow;
				CollisionEvent collisionEvent = null;

				if (withBall) {
					// always play a collision sound with the ball
					collisionEvent = this.audioSampler.CreateCollisionEvent (impulse, true, withTable);
					this.lastCollisionTime = DateTime.UtcNow;
				} else {
					// check cooldown-time.
					if (this.lastCollisionTime == DateTime.MinValue || (now - this.lastCollisionTime).TotalSeconds > CollisionCooldown) {
						this.lastCollisionTime = now;
						collisionEvent = this.audioSampler.CreateCollisionEvent (impulse, false, withTable);
					}
				}

				if (collisionEvent != null) {
					this.Delegate?.CollisionEventOccurred (this, collisionEvent);
				}
			}
		}

		/// <summary>
		/// Play the collision event on the sampler. (Server and client)
		/// </summary>
		public void PlayCollisionSound (CollisionEvent collisionEvent)
		{
			if (this.audioSampler != null) {
				this.audioSampler.Play (collisionEvent);
			}
		}
	}
}
