using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace SpriteKitPhysicsCollisions
{
	public class ShipSprite : SKSpriteNode
	{
		const int startingShipHealth = 10;
		const int showDamageBelowHealth = 4;

		// Used to configure a ship explosion.
		const double shipExplosionDuration = 0.6;
		const float shipChunkMinimumSpeed = 300f;
		const float shipChunkMaximumSpeed = 750f;
		const float shipChunkDispersion = 30f;
		const int numberOfChunks = 30;
		const float removeShipTime = 0.35f;

		// Used to control the ship, usually by applying physics forces to the ship.
		float mainEngineThrust = 5f;
		float reverseThrust = 1f;
		float lateralThrust = 0.005f;
		float firingInterval = 0.1f;
		float missileLaunchDistance = 45f;
		const float engineIdleAlpha = 0.05f;
		float missileLaunchImpulse = 0.5f;

		ExhaustNode exhaustNode;
		SKEmitterNode visibleDamageNode;
		nfloat engineEngagedAlpha;
		double timeLastFiredMissile;

		int health;

		static readonly Random rand = new Random ();
		static float myRand (float low, float high)
		{
			return (float)rand.NextDouble () * (high - low) + low;
		}

		public ShipSprite (CGPoint initialPosition)
			: base (NSBundle.MainBundle.PathForResource ("spaceship", "png"))
		{
			health = startingShipHealth;

			using (CGPath boundingPath = new CGPath ()) {
				boundingPath.MoveToPoint (-12f, -38f);
				boundingPath.AddLineToPoint (12f, -38f);
				boundingPath.AddLineToPoint (9f, 18f);
				boundingPath.AddLineToPoint (2f, 38f);
				boundingPath.AddLineToPoint (-2f, 38f);
				boundingPath.AddLineToPoint (-9f, 18f);
				boundingPath.AddLineToPoint (-12f, -38f);
#if false
			// Debug overlay
			SKShapeNode shipOverlayShape = new SKShapeNode () {
				Path = boundingPath,
				StrokeColor = UIColor.Clear,
				FillColor = UIColor.FromRGBA (0f, 1f, 0f, 0.5f)
			};
			ship.AddChild (shipOverlayShape);
#endif
				var body = SKPhysicsBody.CreateBodyFromPath (boundingPath);
				body.CategoryBitMask = Category.Ship;
				body.CollisionBitMask = Category.Ship | Category.Asteroid | Category.Planet | Category.Edge;
				body.ContactTestBitMask = body.CollisionBitMask;

				// The ship doesn't slow down when it moves forward, but it does slow its angular rotation. In practice,
				// this feels better for a game.
				body.LinearDamping = 0;
				body.AngularDamping = 0.5f;

				PhysicsBody = body;
			}
			Position = initialPosition;
		}

		public ExhaustNode ExhaustNode {
			get {
				if (exhaustNode == null) {
					var emitter = new ExhaustNode (Scene);
					engineEngagedAlpha = (float)emitter.ParticleAlpha;
					AddChild (emitter);
					exhaustNode = emitter;
				}
				return exhaustNode;
			}
		}

		void ShowDamage()
		{
			// When the ship first shows damage, a damage node is created and added as a child.
			// If it takes more damage, then the number of particles is increased.

			if (visibleDamageNode == null)
			{
				visibleDamageNode = (SKEmitterNode)NSKeyedUnarchiver.UnarchiveFile (NSBundle.MainBundle.PathForResource ("damage", "sks"));
				visibleDamageNode.Name = @"damaged";

				// Make the scene the target node because the ship is moving around in the scene. Smoke particles
				// should be spawned based on the ship, but should otherwise exist independently of the ship.
				visibleDamageNode.TargetNode = Scene;

				AddChild(visibleDamageNode);
			} else {
				visibleDamageNode.ParticleBirthRate = visibleDamageNode.ParticleBirthRate * 2;
			}
		}

		void MakeExhaustNode()
		{
			var emitter = (ExhaustNode)NSKeyedUnarchiver.UnarchiveFile (NSBundle.MainBundle.PathForResource ("exhaust", "sks"));

			// Hard coded position at the back of the ship.
			emitter.Position = new CGPoint(0, -40);
			emitter.Name = "exhaust";

			// Make the scene the target node because the ship is moving around in the scene. Exhaust particles
			// should be spawned based on the ship, but should otherwise exist independently of the ship.

			emitter.TargetNode = Scene;

			// The exhaust node is always emitting particles, but the alpha of the particles is adjusted depending on whether
			// the engines are engaged or not. This adds a subtle effect when the ship is idling.

			engineEngagedAlpha = emitter.ParticleAlpha;
			emitter.ParticleAlpha = engineIdleAlpha;

			AddChild (emitter);
			exhaustNode = emitter;
		}

		void MakeExhaustNodeIfNeeded()
		{
			if (exhaustNode == null)
				MakeExhaustNode ();
		}

		public void ApplyDamage (int amount)
		{
			// If the ship takes too much damage, blow it up. Otherwise, decrement the health (and show damage if necessary).
			if (amount >= health) {
				if (health >= 0) {
					health = 0;
					Explode ();
				}
			} else {
				health -= amount;
				if (health < showDamageBelowHealth)
					ShowDamage ();
			}
		}

		void Explode ()
		{
			// Create a bunch of explosion emitters and send them flying in all directions. Then remove the ship from the scene.
			for (int i = 0; i < numberOfChunks; i++) {
				SKEmitterNode explosion = NodeFactory.CreateExplosionNode(Scene, shipExplosionDuration);

				float angle = myRand (0, (float) Math.PI * 2);
				float speed = myRand (shipChunkMinimumSpeed, shipChunkMaximumSpeed);
				var x = myRand ((float)Position.X - shipChunkDispersion, (float)Position.X + shipChunkDispersion);
				var y = myRand ((float)Position.Y - shipChunkDispersion, (float)Position.Y + shipChunkDispersion);
				explosion.Position = new CGPoint (x, y);

				var body = SKPhysicsBody.CreateCircularBody (0.25f);
				body.CollisionBitMask = 0;
				body.ContactTestBitMask = 0;
				body.CategoryBitMask = 0;
				body.Velocity = new CGVector ((float) Math.Cos (angle) * speed, (float) Math.Sin (angle) * speed);
				explosion.PhysicsBody = body;

				Scene.AddChild (explosion);
			}

			RunAction (SKAction.Sequence (
				SKAction.WaitForDuration (removeShipTime),
				SKAction.RemoveFromParent ()
			));
		}

		public float ShipOrientation {
			get {
				// The ship art is oriented so that it faces the top of the scene, but Sprite Kit's rotation default is to the right.
				// This method calculates the ship orientation for use in other calculations.
				return (float)ZRotation + (float)Math.PI / 2;
			}
		}

		public float ShipExhaustAngle {
			get {
				// The ship art is oriented so that it faces the top of the scene, but Sprite Kit's rotation default is to the right.
				// This method calculates the direction for the ship's rear.
				return (float)ZRotation - (float)Math.PI / 2;
			}
		}

		public void ActivateMainEngine ()
		{
			// Add flames out the back and apply thrust to the ship.

			float shipDirection = ShipOrientation;
			var dx = mainEngineThrust * (float)Math.Cos (shipDirection);
			var dy = mainEngineThrust * (float)Math.Sin (shipDirection);
			PhysicsBody.ApplyForce (new CGVector (dx, dy));

			MakeExhaustNodeIfNeeded ();
			ExhaustNode.ParticleAlpha = engineEngagedAlpha;
			ExhaustNode.EmissionAngle = ShipExhaustAngle;
		}

		public void DeactivateMainEngine ()
		{
			ExhaustNode.ParticleAlpha = ExhaustNode.IdleAlpha;
			ExhaustNode.EmissionAngle = ShipExhaustAngle;
		}

		public void ReverseThrust ()
		{
			double reverseDirection = ShipOrientation + Math.PI;
			var dx = reverseThrust * (float)Math.Cos (reverseDirection);
			var dy = reverseThrust * (float)Math.Sin (reverseDirection);
			PhysicsBody.ApplyForce (new CGVector (dx, dy));
		}

		public void RotateShipLeft ()
		{
			PhysicsBody.ApplyTorque (lateralThrust);
		}

		public void RotateShipRight ()
		{
			PhysicsBody.ApplyTorque (-lateralThrust);
		}

		public void AttemptMissileLaunch (double currentTime)
		{
			if (health <= 0)
				return;

			double timeSinceLastFired = currentTime - timeLastFiredMissile;
			if (timeSinceLastFired > firingInterval) {
				timeLastFiredMissile = currentTime;
				// avoid duplicating costly math ops
				float shipDirection = ShipOrientation;
				float cos = (float) Math.Cos (shipDirection);
				float sin = (float) Math.Sin (shipDirection);

				var position = new CGPoint (Position.X + missileLaunchDistance * cos,
					Position.Y + missileLaunchDistance * sin);
				SKNode missile = NodeFactory.CreateMissileNode (this);
				missile.Position = position;
				Scene.AddChild (missile);

				missile.PhysicsBody.Velocity = PhysicsBody.Velocity;
				var vector = new CGVector (missileLaunchImpulse * cos, missileLaunchImpulse * sin);
				missile.PhysicsBody.ApplyImpulse (vector);
			}
		}
	}
}