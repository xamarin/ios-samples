using System;

using SpriteKit;
using Foundation;

namespace SpriteKitPhysicsCollisions
{
	public abstract class ArchiveBasedNode : SKEmitterNode
	{
		public ArchiveBasedNode (IntPtr template)
			: base (template)
		{
			// calling the base .ctor with the Handle of the Copy will add an extra Retain
			//Release ();
		}
	
		protected static SKEmitterNode UnarchiveNode (string name, string type)
		{
			var path = NSBundle.MainBundle.PathForResource (name, type);
			return (SKEmitterNode) NSKeyedUnarchiver.UnarchiveFile (path);
		}
	}
}

