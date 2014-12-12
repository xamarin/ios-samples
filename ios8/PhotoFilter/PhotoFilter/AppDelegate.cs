using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace PhotoFilter
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// check is it 64bit or 32bit
			Console.WriteLine (IntPtr.Size);

			// Main app do nothing
			// Go to Photo > Edit then choose PhotoFilter to start app extension

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.BackgroundColor = UIColor.Blue;
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}

