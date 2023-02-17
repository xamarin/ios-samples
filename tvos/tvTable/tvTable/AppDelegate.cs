using Foundation;
using UIKit;

namespace tvTable {
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		#region Computed Properties
		/// <summary>
		/// Gets or sets main window for the app.
		/// </summary>
		/// <value>The window.</value>
		public override UIWindow Window {
			get;
			set;
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Override point for customization after application launch.
		/// </summary>
		/// <returns><c>true</c> if the app has successfully launched, else <c>false</c>.</returns>
		/// <param name="application">The <c>UIApplication</c> being launched.</param>
		/// <param name="launchOptions">A <c>NSDictionary</c> of Launch options.</param>
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			return true;
		}

		/// <summary>
		/// Invoked when the application is about to move from active to inactive state.
		/// </summary>
		/// <param name="application">The <c>UIApplication</c>.</param>
		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		/// <summary>
		/// Use this method to release shared resources, save user data, invalidate timers 
		/// and store the application state.
		/// </summary>
		/// <param name="application">The <c>UIApplication</c>.</param>
		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		/// <summary>
		/// Called as part of the transiton from background to active state.
		/// </summary>
		/// <param name="application">The <c>UIApplication</c>.</param>
		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		/// <summary>
		/// Ons the activated.
		/// </summary>
		/// <param name="application">The <c>UIApplication</c>.</param>
		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		/// <summary>
		/// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		/// </summary>
		/// <param name="application">The <c>UIApplication</c>.</param>
		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
		#endregion
	}
}


