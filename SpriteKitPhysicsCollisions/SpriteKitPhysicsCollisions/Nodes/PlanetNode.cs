using System;

using UIKit;
using SpriteKit;
using CoreGraphics;

namespace SpriteKitPhysicsCollisions
{
	public class PlanetNode : SKShapeNode
	{
		const float defaultSize = 64f;

		public PlanetNode (CGPoint initialPosition, float size = defaultSize)
		{
			var path = new CGPath ();
			path.AddArc (0, 0, size, 0, (float)Math.PI * 2f, true);
			Path = path;
			StrokeColor = UIColor.Clear;
			FillColor = UIColor.Green;
			Position = initialPosition;
			// use a local variable to avoid multiple virtual call to the `PhysicsBody` property
			var body = SKPhysicsBody.CreateCircularBody (size);
			body.CategoryBitMask = Category.Planet;
			body.CollisionBitMask = Category.Planet | Category.Edge;
			body.ContactTestBitMask = 0;
			PhysicsBody = body;
		}
	}
}