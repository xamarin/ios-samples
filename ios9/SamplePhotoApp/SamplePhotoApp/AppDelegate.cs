using Foundation;
using UIKit;

namespace SamplePhotoApp {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// Code to start the Xamarin Test Cloud Agent

			Xamarin.Calabash.Start ();

			return true;
		}
	}
}


