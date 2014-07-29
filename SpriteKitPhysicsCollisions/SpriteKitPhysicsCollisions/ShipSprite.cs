using System;
using CoreGraphics;

using Foundation;
using SpriteKit;
using UIKit;

namespace SpriteKitPhysicsCollisions {

	public class ShipSprite : SKSpriteNode {

		const int startingShipHealth = 10;
		const int showDamageBelowHealth = 4;
		const double shipExplosionDuration = 0.6;
		const float shipChunkMinimumSpeed = 300f;
		const float shipChunkMaximumSpeed = 750f;

		nfloat shipChunkDispersion = 30f;
		nint numberOfChunks = 30;
		nfloat removeShipTime = 0.35f;

		nfloat mainEngineThrust = 0.12f;
		nfloat reverseThrust = 0.03f;
		nfloat lateralThrust = 0.01f;
		nfloat firingInterval = 0.1f;
		nfloat missileLaunchDistance = 45f;
		nfloat missileLaunchImpulse = 0.5f;

		ExhaustNode exhaustNode;
		SKEmitterNode visibleDamageNode;
		nfloat engineEngagedAlpha;
		double timeLastFiredMissile;
	
		nint health;

		static Random rand = new Random ();
		static nfloat myRand (nfloat low, nfloat high)
		{
			return (float)rand.NextDouble () * (high - low) + low;
		}

		public ShipSprite (CGPoint initialPosition)
			: base (NSBundle.MainBundle.PathForResource ("spaceship", "png"))
		{
			CGPath boundingPath = new CGPath ();
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
			var body = SKPhysicsBody.BodyWithPolygonFromPath (boundingPath);
			body.CategoryBitMask = Category.Ship;
			body.CollisionBitMask = Category.Ship | Category.Asteroid | Category.Planet | Category.Edge; 
			body.ContactTestBitMask = body.CollisionBitMask;
			body.LinearDamping = 0;
			body.AngularDamping = 0.5f;

			PhysicsBody = body;
			Position = initialPosition;
		}

		public ExhaustNode ExhaustNode {
			get {
				if (exhaustNode == null) {
					var emitter = new ExhaustNode (Scene);
					engineEngagedAlpha = emitter.ParticleAlpha;
					AddChild (emitter);
					exhaustNode = emitter;
				}
				return exhaustNode;
			}
		}

		public void ApplyDamage (int amount)
		{
			if (amount >= health) {
				if (health >= 0) {
					health = 0;
					Explode ();
				}
			} else {
				health -= amount;
				if (health < showDamageBelowHealth) {
					// Show (increasing from none) damage to the ship
					if (visibleDamageNode == null) {
						visibleDamageNode = new DamageNode (Scene);
						AddChild (visibleDamageNode);
					} else
						visibleDamageNode.ParticleBirthRate = visibleDamageNode.ParticleBirthRate * 2;
				}
			}
		}

		void Explode ()
		{
			for (int i = 0; i < numberOfChunks; i++) {
				nfloat angle = myRand (0, (nfloat) Math.PI * 2);
				nfloat speed = myRand (shipChunkMinimumSpeed, shipChunkMaximumSpeed);
				var position = new CGPoint (myRand (Position.X - shipChunkDispersion, Position.Y + shipChunkDispersion),
				                           myRand (Position.Y - shipChunkDispersion, Position.Y + shipChunkDispersion));
				var explosion = new ExplosionNode (Scene, position);
				var body = SKPhysicsBody.BodyWithCircleOfRadius (0.25f);
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

		public nfloat ShipOrientation {
			get {
				return ZRotation + (float)Math.PI / 2;
			}
		}

		public nfloat ShipExhaustAngle {
			get {
				return ZRotation - (float)Math.PI / 2;
			}
		}

		public void ActivateMainEngine ()
		{
			nfloat shipDirection = ShipOrientation;
			PhysicsBody.ApplyImpulse (new CGVector (mainEngineThrust * (float) Math.Cos (shipDirection),
				mainEngineThrust * (float) Math.Sin (shipDirection)));
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
			PhysicsBody.ApplyImpulse (new CGVector (reverseThrust * (float) Math.Cos (reverseDirection),
				reverseThrust * (float) Math.Sin (reverseDirection)));
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
			double timeSinceLastFired = currentTime - timeLastFiredMissile;
			if (timeSinceLastFired > firingInterval) {
				timeLastFiredMissile = currentTime;
				// avoid duplicating costly math ops
				nfloat shipDirection = ShipOrientation;
				nfloat cos = (nfloat) Math.Cos (shipDirection);
				nfloat sin = (nfloat) Math.Sin (shipDirection);

				var position = new CGPoint (Position.X + missileLaunchDistance * cos,
					Position.Y + missileLaunchDistance * sin);
				SKNode missile = new MissileNode (this, position);
				Scene.AddChild (missile);

				missile.PhysicsBody.Velocity = PhysicsBody.Velocity;
				var vector = new CGVector (missileLaunchImpulse * cos, missileLaunchImpulse * sin);
				missile.PhysicsBody.ApplyImpulse (vector);
			}
		}
	}
}