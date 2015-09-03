using System;

using UIKit;
using Foundation;

namespace PhotoFilter
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// check is it 64bit or 32bit
			Console.WriteLine (IntPtr.Size);

			// Main app do nothing
			// Go to Photo > Edit then choose PhotoFilter to start app extension

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = new ViewController ();

			window.MakeKeyAndVisible ();
			return true;
		}
	}
}