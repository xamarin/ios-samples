using Foundation;
using UIKit;
using Intents;
using System;
using ElizaCore;
using UserNotifications;

namespace ElizaChat {
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		#region Computed Properties
		public override UIWindow Window { get; set; }
		public ViewController Controller { get; set; }
		public INSiriAuthorizationStatus SiriAuthorizationStatus { get; set; }
		public ElizaAddressBook AddressBook { get; set; } = new ElizaAddressBook ();
		#endregion

		#region Override Methods
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Request access to Siri
			INPreferences.RequestSiriAuthorization ((INSiriAuthorizationStatus status) => {
				// Respond to returned status
				switch (status) {
				case INSiriAuthorizationStatus.Authorized:
					Console.WriteLine ("SiriKit Authorized");
					break;
				case INSiriAuthorizationStatus.Denied:
					Console.WriteLine ("SiriKit Denied");
					break;
				case INSiriAuthorizationStatus.NotDetermined:
					Console.WriteLine ("SiriKit Not Determined");
					break;
				case INSiriAuthorizationStatus.Restricted:
					Console.WriteLine ("SiriKit Restricted");
					break;
				}

				// Request notification permissions from the user
				UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert, (approved, err) => {
					// Handle approval
				});

				// Save status
				SiriAuthorizationStatus = status;
			});

			// Populate the address book
			AddressBook.LoadAddressBook ();

			// Inform system of successful launch
			return true;
		}

		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			// Take action based on the activity type
			switch (userActivity.ActivityType) {
			case "com.appracatappra.askquestion":
				// Pass question to Eliza to answer
				Controller.AskQuestion (userActivity.UserInfo.ValueForKey (new NSString ("question")).ToString (), true);
				break;
			}

			// Inform system this is handled
			return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
		#endregion
	}
}

