using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace ImageInverter
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			return true;
		}
	}
}
