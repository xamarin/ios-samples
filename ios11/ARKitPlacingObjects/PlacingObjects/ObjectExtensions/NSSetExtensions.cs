using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace PlacingObjects
{
	public static class NSSetExtensions
	{
		public static List<UITouch> ToTouchList(this NSSet touches) {

			var touchArray = touches.ToArray<UITouch>();
			var touchList = new List<UITouch>(touchArray);
			return touchList;
		}
	}
}
