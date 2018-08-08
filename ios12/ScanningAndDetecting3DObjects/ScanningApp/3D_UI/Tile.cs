using System;
using SceneKit;
using System.Linq;
using System.Net;
namespace ScanningAndDetecting3DObjects
{
	internal class Tile : SCNNode
	{
		internal bool Captured { get;  set; }

		internal bool Highlighted { get;  set;  }

		internal Tile(SCNPlane plane)
		{
			base.Init();
			Geometry = plane;
			Opacity = 0F; 

			// Create a child node with another plane of the same size, but a darker color to stand out better
			// This helps users see captured tiles from the back.
			if (ChildNodes.Length == 0)
			{
				var innerPlane = SCNPlane.Create(plane.Width, plane.Height);
				innerPlane.Materials = new[] { Utilities.Material(Utilities.AppBrown.ColorWithAlpha(0.8F), false, false) };
				var innerNode = SCNNode.FromGeometry(innerPlane);
				innerNode.EulerAngles = new SCNVector3(0, (float) Math.PI, 0);
				AddChildNode(innerNode);
			}
		}

		internal void UpdateVisualization()
		{
			var newOpacity = Captured ? 0.5 : 0.0;
			newOpacity += Highlighted ? 0.35 : 0.0;
			Opacity = (System.nfloat) newOpacity;
		}
	}
}