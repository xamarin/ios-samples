using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public class HitTestRay : NSObject
	{
		public SCNVector3 Origin { get; set; }

		public SCNVector3 Direction { get; set; }
		public HitTestRay()
		{
		}

		public HitTestRay(SCNVector3 origin, SCNVector3 direction)
		{
			// Initialize
			Origin = origin;
			Direction = direction;
		}
	}
}
