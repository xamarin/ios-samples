using System;
using SpriteKit;
using Foundation;

namespace SpriteKitPhysicsCollisions
{
	public class DamageNode : ArchiveBasedNode
	{
		static readonly SKEmitterNode template;

		static DamageNode ()
		{
			template = UnarchiveNode ("damage", "sks");
			template.Name = "damaged";
		}

		public DamageNode (SKNode target)
			: base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
		}
	}
}