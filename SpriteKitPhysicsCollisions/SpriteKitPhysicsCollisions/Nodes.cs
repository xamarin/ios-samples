using System;
using CoreGraphics;

using Foundation;
using SpriteKit;
using UIKit;

namespace SpriteKitPhysicsCollisions {

	// an enum [Flag] could be used but would require more casts
	public static class Category {
		public const uint Missile  = 0x1 << 0;
		public const uint Ship     = 0x1 << 1;
		public const uint Asteroid = 0x1 << 2;
		public const uint Planet   = 0x1 << 3;
		public const uint Edge     = 0x1 << 4;
	}

	public class AsteroidNode : SKShapeNode {

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
			var body = SKPhysicsBody.BodyWithCircleOfRadius (size);
			body.CategoryBitMask = Category.Asteroid;
			body.CollisionBitMask = Category.Ship | Category.Asteroid | Category.Edge;
			body.ContactTestBitMask = Category.Planet;
			PhysicsBody = body;
		}
	}

	public class PlanetNode : SKShapeNode {

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
			var body = SKPhysicsBody.BodyWithCircleOfRadius (size);
			body.CategoryBitMask = Category.Planet;
			body.CollisionBitMask = Category.Planet | Category.Edge;
			body.ContactTestBitMask = 0;
			PhysicsBody = body;
		}
	}

	public abstract class ArchiveBasedNode : SKEmitterNode {

		static public SKEmitterNode UnarchiveNode (string name, string type)
		{
			var path = NSBundle.MainBundle.PathForResource (name, type);
			return (SKEmitterNode) NSKeyedUnarchiver.UnarchiveFile (path);
		}

		public ArchiveBasedNode (IntPtr template) : base (template)
		{
			// calling the base .ctor with the Handle of the Copy will add an extra Retain
			//Release ();
		}
	}

	public class MissileNode : ArchiveBasedNode {

		const float defaultSize = 4f;

		static SKEmitterNode template;

		static MissileNode ()
		{
			template = UnarchiveNode ("missile", "sks");
			// use a local variable to avoid multiple virtual call to the `PhysicsBody` property
			var body = SKPhysicsBody.BodyWithCircleOfRadius (defaultSize);
			body.CategoryBitMask = Category.Missile;
			body.ContactTestBitMask = Category.Ship | Category.Asteroid | Category.Planet | Category.Edge;
			body.CollisionBitMask = 0;
			template.PhysicsBody = body;
		}

		public MissileNode (SKNode target, CGPoint initialPosition) : base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
			Position = initialPosition;
		}
	}

	public class ExplosionNode : ArchiveBasedNode {

		const double defaultDuration = 0.1f;

		static SKEmitterNode template = UnarchiveNode ("explosion", "sks");

		public ExplosionNode (SKNode target, CGPoint initialPosition, double duration = defaultDuration) : 
			base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
			Position = initialPosition;
			NumParticlesToEmit = (uint) (duration * ParticleBirthRate);
			double totalTime = duration + ParticleLifetime + ParticleLifetimeRange / 2;
			RunAction (SKAction.Sequence (
				SKAction.WaitForDuration (totalTime),
				SKAction.RemoveFromParent ()
			));
		}
	}

	public class DamageNode : ArchiveBasedNode {

		static SKEmitterNode template;

		static DamageNode ()
		{
			template = UnarchiveNode ("damage", "sks");
			template.Name = "damaged";
		}

		public DamageNode (SKNode target) : base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
		}
	}

	public class ExhaustNode : ArchiveBasedNode {

		public const float IdleAlpha = 0.05f;

		static SKEmitterNode template;

		static ExhaustNode ()
		{
			template = UnarchiveNode ("exhaust", "sks");
			template.Position = new CGPoint (0f, -40f);
			template.Name = "exhaust";
			template.ParticleAlpha = IdleAlpha;
		}

		public ExhaustNode (SKNode target) : base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
		}
	}
}