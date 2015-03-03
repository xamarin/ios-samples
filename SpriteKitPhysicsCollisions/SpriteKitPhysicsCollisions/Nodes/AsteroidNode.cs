using System;

using UIKit;
using SpriteKit;
using CoreGraphics;

namespace SpriteKitPhysicsCollisions
{
	public class AsteroidNode : SKShapeNode
	{
		const float defaultSize = 18f;

		public AsteroidNode (CGPoint initialPosition, float size = defaultSize)
		{
			var path = new CGPath ();
			path.AddArc (0, 0, size, 0, (float)Math.PI * 2f, true);
			Path = path;
			StrokeColor = UIColor.Clear;
			FillColor = UIColor.Brown;
			Position = initialPosition;

			// use a local variable to avoid multiple virtual call to the `PhysicsBody` property
			var body = SKPhysicsBody.CreateCircularBody (size);
			body.CategoryBitMask = Category.Asteroid;
			body.CollisionBitMask = Category.Ship | Category.Asteroid | Category.Edge;
			body.ContactTestBitMask = Category.Planet;
			PhysicsBody = body;
		}
	}
}

