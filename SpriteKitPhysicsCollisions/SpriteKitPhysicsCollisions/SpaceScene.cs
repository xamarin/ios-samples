using System;

using UIKit;
using SpriteKit;
using Foundation;
using CoreGraphics;

namespace SpriteKitPhysicsCollisions
{
	public class SpaceScene : SKScene, ISKPhysicsContactDelegate
	{
		const float collisionDamageThreshold = 1.0f;
		const int missileDamage = 1;

		bool[] actions = new bool [5];
		bool contentCreated;
		ShipSprite controlledShip;

		static readonly Random rand = new Random ();

		static float myRand (float low, float high)
		{
			return (float)rand.NextDouble () * (high - low) + low;
		}

		public SpaceScene (CGSize size)
			: base (size)
		{
		}

		public override void DidMoveToView (SKView view)
		{
			if (!contentCreated) {
				CreateSceneContents ();
				contentCreated = true;
			}
		}

		void CreateSceneContents ()
		{
			BackgroundColor = UIColor.Black;
			ScaleMode = SKSceneScaleMode.AspectFit;

			// Give the scene an edge and configure other physics info on the scene.
			var body = SKPhysicsBody.CreateEdgeLoop (Frame);
			body.CategoryBitMask = Category.Edge;
			body.CollisionBitMask = 0;
			body.ContactTestBitMask = 0;
			PhysicsBody = body;

			PhysicsWorld.Gravity = new CGVector (0, 0);
			PhysicsWorld.ContactDelegate = this;

			// In this sample, the positions of everything is hard coded. In an actual game, you might implement this in an archive that is loaded from a file.
			controlledShip = new ShipSprite (new CGPoint (100, 300));
			AddChild (controlledShip);

			// this ship isn't connected to any controls so it doesn't move, except when it collides with something.
			AddChild (new ShipSprite (new CGPoint (200, 300)));

			AddChild (new AsteroidNode (new CGPoint (100, 200)));
			AddChild (new PlanetNode (new CGPoint (300, 100)));
		}

		#region Physics Handling and Game Logic

		void AttackTarget (SKPhysicsBody target, SKNode missile)
		{
			// Only ships take damage from missiles.
			if ((target.CategoryBitMask & Category.Ship) != 0)
				((ShipSprite)target.Node).ApplyDamage (missileDamage);

			DetonateMissile (missile);
		}

		void DetonateMissile(SKNode missile)
		{
			SKEmitterNode explosion = new ExplosionNode (this);
			explosion.Position = missile.Position;
			AddChild (explosion);
			missile.RemoveFromParent ();
		}

		[Export ("didBeginContact:")]
		public void DidBeginContact (SKPhysicsContact contact)
		{
			// Handle contacts between two physics bodies.

			// Contacts are often a double dispatch problem; the effect you want is based
			// on the type of both bodies in the contact. This sample  solves
			// this in a brute force way, by checking the types of each. A more complicated
			// example might use methods on objects to perform the type checking.

			SKPhysicsBody firstBody;
			SKPhysicsBody secondBody;

			// The contacts can appear in either order, and so normally you'd need to check
			// each against the other. In this example, the category types are well ordered, so
			// the code swaps the two bodies if they are out of order. This allows the code
			// to only test collisions once.

			if (contact.BodyA.CategoryBitMask < contact.BodyB.CategoryBitMask) {
				firstBody = contact.BodyA;
				secondBody = contact.BodyB;
			} else {
				firstBody = contact.BodyB;
				secondBody = contact.BodyA;
			}

			// Missiles attack whatever they hit, then explode.
			if ((firstBody.CategoryBitMask & Category.Missile) != 0)
				AttackTarget (secondBody, firstBody.Node);

			// Ships collide and take damage. The collision damage is based on the strength of the collision.
			if ((firstBody.CategoryBitMask & Category.Ship) != 0) {
				// The edge exists just to keep all gameplay on one screen,
				// so ships should not take damage when they hit the edge.
				if (contact.CollisionImpulse > collisionDamageThreshold && (secondBody.CategoryBitMask & Category.Edge) == 0) {
					int damage = (int) (contact.CollisionImpulse / collisionDamageThreshold);
					((ShipSprite)firstBody.Node).ApplyDamage (damage);

					if ((secondBody.CategoryBitMask == Category.Ship))
						((ShipSprite)secondBody.Node).ApplyDamage (damage);
				}
			}

			// Asteroids that hit planets are destroyed.
			if ((firstBody.CategoryBitMask & Category.Asteroid) != 0 &&
				(secondBody.CategoryBitMask & Category.Planet) != 0)
				firstBody.Node.RemoveFromParent ();
		}

		#endregion

		#region Controls and Control Logic

		public override void Update (double currentTime)
		{
			// This runs once every frame. Other sorts of logic might run from here. For example,
			// if the target ship was controlled by the computer, you might run AI from this routine.

			// Use the stored key information to control the ship.
			if (actions [(int)PlayerActions.Forward])
				controlledShip.ActivateMainEngine ();
			else
				controlledShip.DeactivateMainEngine ();

			if (actions [(int)PlayerActions.Back])
				controlledShip.ReverseThrust ();

			if (actions [(int)PlayerActions.Left])
				controlledShip.RotateShipLeft ();

			if (actions [(int)PlayerActions.Right])
				controlledShip.RotateShipRight ();

			if (actions [(int)PlayerActions.Action])
				controlledShip.AttemptMissileLaunch (currentTime);
		}

		#endregion

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			actions [(int)PlayerActions.Action] = false;
			actions [(int)PlayerActions.Left] = false;
			actions [(int)PlayerActions.Right] = false;
			actions [(int)PlayerActions.Forward] = false;
			actions [(int)PlayerActions.Back] = false;

			UITouch touch = (UITouch)touches.AnyObject;
			var location = touch.LocationInView (View);

			var deltaX = location.X - controlledShip.Position.X;
			var deltaY = (View.Bounds.Height - location.Y) - controlledShip.Position.Y;

			if (Math.Abs (deltaX) < 30 && Math.Abs (deltaY) < 30) {
				actions [(int)PlayerActions.Action] = true;
			} else if (Math.Abs (deltaX) > Math.Abs (deltaY)) {
				if (deltaX < 0)
					actions [(int)PlayerActions.Left] = true;
				else
					actions [(int)PlayerActions.Right] = true;
			} else {
				if (deltaY < 0)
					actions [(int)PlayerActions.Forward] = true;
				else
					actions [(int)PlayerActions.Back] = true;
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			actions [(int)PlayerActions.Action] = false;
			actions [(int)PlayerActions.Left] = false;
			actions [(int)PlayerActions.Right] = false;
			actions [(int)PlayerActions.Forward] = false;
			actions [(int)PlayerActions.Back] = false;

			UITouch touch = (UITouch)touches.AnyObject;
			var location = touch.LocationInView (View);

			var deltaX = location.X - controlledShip.Position.X;
			var deltaY = (View.Bounds.Height - location.Y) - controlledShip.Position.Y;

			if (Math.Abs (deltaX) < 30 && Math.Abs (deltaY) < 30)
				actions [(int)PlayerActions.Action] = true;
			else if (Math.Abs (deltaX) > Math.Abs (deltaY)) {
				if (deltaX < 0)
					actions [(int)PlayerActions.Left] = true;
				else
					actions [(int)PlayerActions.Right] = true;
			} else {
				if (deltaY < 0)
					actions [(int)PlayerActions.Forward] = true;
				else
					actions [(int)PlayerActions.Back] = true;
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			actions [(int)PlayerActions.Action] = false;
			actions [(int)PlayerActions.Left] = false;
			actions [(int)PlayerActions.Right] = false;
			actions [(int)PlayerActions.Forward] = false;
			actions [(int)PlayerActions.Back] = false;
		}
	}
}
