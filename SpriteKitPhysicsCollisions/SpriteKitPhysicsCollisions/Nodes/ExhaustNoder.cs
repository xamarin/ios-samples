using System;

using SpriteKit;
using Foundation;
using CoreGraphics;

namespace SpriteKitPhysicsCollisions
{
	public class ExhaustNode : ArchiveBasedNode
	{
		public const float IdleAlpha = 0.05f;

		static SKEmitterNode template;

		static ExhaustNode ()
		{
			template = UnarchiveNode ("exhaust", "sks");
			template.Position = new CGPoint (0f, -40f);
			template.Name = "exhaust";
			template.ParticleAlpha = IdleAlpha;
		}

		public ExhaustNode (SKNode target)
			: base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
		}
	}
}
