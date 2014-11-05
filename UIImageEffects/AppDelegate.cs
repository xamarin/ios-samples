using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace UIImageEffects
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

