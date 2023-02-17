/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A basic app delegate implementation.
 Demonstrates how to return a particular UISceneConfiguration for a new scene session.
*/

using Foundation;
using UIKit;

namespace Gallery {
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register (nameof (AppDelegate))]
	public class AppDelegate : UIApplicationDelegate {
		// class-level declarations
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			return true;
		}

		public override UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
			=> new UISceneConfiguration ("Default Configuration", connectingSceneSession.Role);
	}
}

