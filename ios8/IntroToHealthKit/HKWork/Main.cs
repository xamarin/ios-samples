using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace HKWork
{
	public class Application
	{
		//Sample application that stores heart-rate data in Health Kit
		//Note: Must be used with a Health Kit enabled Provisioning Profile
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
