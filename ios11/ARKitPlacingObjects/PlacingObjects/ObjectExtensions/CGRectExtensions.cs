using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public static class CGRectExtensions
	{
		public static CGPoint GetMidpoint(this CGRect rect) {
			return new CGPoint(rect.GetMidX(), rect.GetMidY());
		}
	}
}
