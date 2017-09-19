using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreFoundation;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public class Plane : SCNNode
	{
		public ARPlaneAnchor Anchor { get; set; }

		public Plane(ARPlaneAnchor anchor)
		{
			// Initialize
			Anchor = anchor;

			base.Init();
		}

		public Plane(NSCoder coder) : base(coder)
		{

		}

		public void Update(ARPlaneAnchor anchor)
		{

			Anchor = anchor;
		}

	}
}
