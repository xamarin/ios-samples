using System;
using Foundation;
using UIKit;

namespace PlacingObjects
{
	public static class AppSettings
	{
		public static bool ScaleWithPinchGesture
		{
			get { return NSUserDefaults.StandardUserDefaults.BoolForKey("ScaleWithPinchGesture"); }
			set { NSUserDefaults.StandardUserDefaults.SetBool(value, "ScaleWithPinchGesture"); }
		}

		public static bool DragOnInfinitePlanes
		{
			get { return NSUserDefaults.StandardUserDefaults.BoolForKey("DragOnInfinitePlanes"); }
			set { NSUserDefaults.StandardUserDefaults.SetBool(value, "DragOnInfinitePlanes"); }
		}

		public static void RegisterDefaults() {
			DragOnInfinitePlanes = true;
			ScaleWithPinchGesture = true;
		}
	}
}
