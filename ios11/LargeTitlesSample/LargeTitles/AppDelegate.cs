using Foundation;
using UIKit;

namespace largetitles {
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		// class-level declarations

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// < Back button color
			UINavigationBar.Appearance.TintColor = UIColor.FromRGB (0xE7, 0x63, 0x3B); // e7963b dark
																					   // Title bar background color
			UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB (0xF7, 0xE2, 0x8B); // f7e28b light
																						  // 'small' Title bar text
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes () {
				TextColor = UIColor.FromRGB (0xE7, 0x63, 0x3B), // e7963b dark
				TextShadowColor = UIColor.Clear
			});
			// 'Large' Title bar text
			UINavigationBar.Appearance.LargeTitleTextAttributes = new UIStringAttributes {
				ForegroundColor = UIColor.FromRGB (0xE7, 0x63, 0x3B), // e7963b dark
			};


			return true;
		}
	}
}

