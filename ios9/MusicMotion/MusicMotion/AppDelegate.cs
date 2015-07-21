using Foundation;
using UIKit;

namespace MusicMotion {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
			return true;
		}
	}
}


