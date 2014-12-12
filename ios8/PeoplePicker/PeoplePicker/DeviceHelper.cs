using System;
using UIKit;

namespace PeoplePicker
{
	public static class DeviceHelper
	{
		public static bool IsRunningOn8 ()
		{
			bool result = true;
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				result = false;
				UIAlertView alert = new UIAlertView ("Error",
					                    "This picker sample can only run on iOS 8 or later.",
					                    null, "OK");
				alert.Show ();
			}

			return result;
		}
	}
}

