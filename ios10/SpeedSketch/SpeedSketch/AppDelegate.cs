using Foundation;
using UIKit;

namespace SpeedSketch
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Minimal basic setup without a storyboard.
			var localWindow = new UIWindow (UIScreen.MainScreen.Bounds);
			localWindow.RootViewController = new CanvasMainViewController ();
			localWindow.MakeKeyAndVisible ();
			Window = localWindow;
			return true;
		}
	}
}
