using System;

using SpriteKit;
using Foundation;
using CoreGraphics;

namespace SpriteKitPhysicsCollisions
{
	public static class NodeFactory
	{
		static readonly nfloat shotSize = 4;

		static SKEmitterNode UnarchiveEmitterNode (string name)
		{
			var path = NSBundle.MainBundle.PathForResource (name, "sks");
			return (SKEmitterNode) NSKeyedUnarchiver.UnarchiveFile (path);
		}

		public static SKEmitterNode CreateExplosionNode (SKNode target, double duration)
		{
			SKEmitterNode emitter = UnarchiveEmitterNode ("explosion");

			// Explosions always place their particles into the scene.
			emitter.TargetNode = target;

			// Stop spawning particles after enough have been spawned.
			emitter.NumParticlesToEmit =(nuint)(duration * emitter.ParticleBirthRate);

			// Calculate a time value that allows all the spawned particles to die. After this, the emitter node can be removed.

			double totalTime = duration + emitter.ParticleLifetime + emitter.ParticleLifetimeRange / 2;
			emitter.RunAction (SKAction.Sequence (SKAction.WaitForDuration (totalTime), SKAction.RemoveFromParent ()));
			return emitter;
		}

		public static SKNode CreateMissileNode(SKNode target)
		{
			// Creates and returns a new missile game object.
			// This method loads a preconfigured emitter from an archive, and then configures it with a physics body.
			SKEmitterNode missile = UnarchiveEmitterNode("missile");

			// The missile particles should be spawned in the scene, not on the missile object.
			missile.TargetNode = target;

			var physicsBody = SKPhysicsBody.CreateCircularBody (shotSize);
			physicsBody.CategoryBitMask = Category.Missile;
			physicsBody.ContactTestBitMask = Category.Ship | Category.Asteroid | Category.Planet | Category.Edge;
			physicsBody.CollisionBitMask = 0;

			missile.PhysicsBody = physicsBody;
			return missile;
		}
	}
}
