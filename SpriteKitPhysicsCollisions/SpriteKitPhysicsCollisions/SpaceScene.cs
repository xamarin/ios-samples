using System;
using SpriteKit;
using CoreGraphics;
using UIKit;
using Foundation;


namespace SpriteKitPhysicsCollisions
{
	public class SpaceScene : SKScene
	{
		public enum PlayerActions {
			Forward = 0,
			Left = 1,
			Right = 2,
			Back = 3,
			Action = 4,
		}

		const float collisionDamageThreshold = 3.0f;
		const int missileDamage = 1;

		bool[] actions = new bool [5];
		bool contentCreated;
		ShipSprite controlledShip;

		static Random rand = new Random ();
		static float myRand (float low, float high)
		{
			return (float)rand.NextDouble () * (high - low) + low;
		}

		public SpaceScene (CGSize size) : base (size)
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

			var body = SKPhysicsBody.BodyWithEdgeLoopFromRect (Frame);
			body.CategoryBitMask = Category.Edge;
			body.CollisionBitMask = 0;
			body.ContactTestBitMask = 0;
			PhysicsBody = body;

			PhysicsWorld.Gravity = new CGVector (0, 0);
			PhysicsWorld.ContactDelegate = new PhysicsDelegate (DidBeginContact);

			controlledShip = new ShipSprite (new CGPoint (100, 300));
			AddChild (controlledShip);
			AddChild (new ShipSprite (new CGPoint (200, 300)));
			AddChild (new AsteroidNode (new CGPoint (100, 200)));
			AddChild (new PlanetNode (new CGPoint (300, 100)));
		}

		void AttackTarget (SKPhysicsBody target, SKNode missile)
		{
			if ((target.CategoryBitMask & Category.Ship) != 0)
				(target.Node as ShipSprite).ApplyDamage (missileDamage);

			// Detonate! i.e. add an explostion at missile's position and remove the missile
			AddChild (new ExplosionNode (this, missile.Position));
			missile.RemoveFromParent ();
		}

		void DidBeginContact (SKPhysicsContact contact)
		{
			SKPhysicsBody firstBody;
			SKPhysicsBody secondBody;

			if (contact.BodyA.CategoryBitMask < contact.BodyB.CategoryBitMask) {
				firstBody = contact.BodyA;
				secondBody = contact.BodyB;
			} else {
				firstBody = contact.BodyB;
				secondBody = contact.BodyA;
			}

			if ((firstBody.CategoryBitMask & Category.Missile) != 0)
				AttackTarget (secondBody, firstBody.Node);

			if ((firstBody.CategoryBitMask & Category.Ship) != 0) {
				if (contact.CollisionImpulse > collisionDamageThreshold && 
				    (secondBody.CategoryBitMask & Category.Edge) == 0) {

					int damage = (int) (contact.CollisionImpulse / collisionDamageThreshold);
					(firstBody.Node as ShipSprite).ApplyDamage (damage);

					if ((secondBody.CategoryBitMask == Category.Ship))
						(secondBody.Node as ShipSprite).ApplyDamage (damage);
				}
			}

			if ((firstBody.CategoryBitMask & Category.Asteroid) != 0 &&
			    (secondBody.CategoryBitMask & Category.Planet) != 0)
				firstBody.Node.RemoveFromParent ();
		}

		public override void Update (double currentTime)
		{
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
			var deltaY = (480 - location.Y) - controlledShip.Position.Y;

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
			var deltaY = (480 - location.Y) - controlledShip.Position.Y;

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

		class PhysicsDelegate : SKPhysicsContactDelegate {

			public PhysicsDelegate (Action<SKPhysicsContact> action)
			{
				DidBeginContactAction = action;
			}

			public Action<SKPhysicsContact> DidBeginContactAction;

			public override void DidBeginContact (SKPhysicsContact contact)
			{
				if (DidBeginContactAction != null)
					DidBeginContactAction (contact);
			}
		}
	}
}