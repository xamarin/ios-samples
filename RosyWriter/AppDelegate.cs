using Foundation;
using UIKit;

namespace RosyWriter
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = new RosyWriterViewControllerUniversal ()
			};
			Window.MakeKeyAndVisible ();
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}