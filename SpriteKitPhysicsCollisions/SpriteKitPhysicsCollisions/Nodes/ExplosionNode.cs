using System;

using SpriteKit;
using Foundation;
using CoreGraphics;

namespace SpriteKitPhysicsCollisions
{
	public class ExplosionNode : ArchiveBasedNode
	{
		const double defaultDuration = 0.1f;

		static SKEmitterNode template = UnarchiveNode ("explosion", "sks");

		public ExplosionNode (SKNode target)
			: base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
			NumParticlesToEmit = (uint) (defaultDuration * ParticleBirthRate);
			double totalTime = defaultDuration + ParticleLifetime + ParticleLifetimeRange / 2;

			RunAction (SKAction.Sequence (
				SKAction.WaitForDuration (totalTime),
				SKAction.RemoveFromParent ()
			));
		}
	}
}

