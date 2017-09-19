using Foundation;
using CoreGraphics;
using SceneKit;
using ARKit;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace PlacingObjects
{
	public class VirtualObject : SCNReferenceNode
	{
		public VirtualObjectDefinition Definition { get; protected set; }

		public VirtualObject(VirtualObjectDefinition definition) : base(NSUrl.FromString(
			NSBundle.MainBundle.BundleUrl.AbsoluteString + $"Models.scnassets/{definition.ModelName}/{definition.ModelName}.scn"))
		{
			this.Definition = definition;

		}

		// Use average of recent virtual object distances to avoid rapid changes in object scale.
		List<float> recentVirtualObjectDistances = new List<float>();
		public List<float> RecentVirtualObjectDistances { get => recentVirtualObjectDistances; }

		public void ReactToScale()
		{
			// Scale the particles in any descendant particle systems (Candle, 
			foreach (var kv in Definition.ParticleScaleInfo)
			{
				var nodeName = kv.Key;
				var particleSize = kv.Value;
				var node = this.FindChildNode(nodeName, true);
				if (node != null)
				{
					var particleSystems = node.ParticleSystems;
					if (particleSystems != null && particleSystems.Length > 0)
					{
						var particleSystem = particleSystems.First();
						particleSystem.Reset();
						particleSystem.ParticleSize = Scale.X * particleSize;
					}
				}
			}
		}

		public static bool IsNodePartOfVirtualObject(SCNNode node)
		{
			// End recursion on success
			if (node is VirtualObject)
			{
				return true;
			}
			// End recursion because we've gotten to the root object with no parent
			if (node.ParentNode == null)
			{
				return false;
			}
			// Recurse up the scene graph
			return IsNodePartOfVirtualObject(node.ParentNode);
		}

		internal static VirtualObject ForChildNode(SCNNode node)
		{
			if (node is VirtualObject)
			{
				return node as VirtualObject;
			}
			// End recursion if gotten to root object with no parent
			if (node.ParentNode == null)
			{
				return null;
			}
			// Recurse up the scene graph
			return ForChildNode(node.ParentNode);
		}
	}
}
