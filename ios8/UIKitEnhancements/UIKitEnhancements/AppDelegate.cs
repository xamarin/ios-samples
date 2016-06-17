using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace UIKitEnhancements
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		#region Computed Properties
		public override UIWindow Window { get; set;}
		public DetailsViewController iPadViewController { get; set; }
		#endregion

		#region Override Methods
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Create a Accept action
			UIMutableUserNotificationAction acceptAction = new UIMutableUserNotificationAction (){
				Identifier = "ACCEPT_ID",
				Title = "Accept",
				ActivationMode = UIUserNotificationActivationMode.Background,
				Destructive = false,
				AuthenticationRequired = false
			};

			// Create a Reply action
			UIMutableUserNotificationAction replyAction = new UIMutableUserNotificationAction () {
				Identifier = "REPLY_ID",
				Title = "Reply",
				ActivationMode = UIUserNotificationActivationMode.Foreground,
				Destructive = false,
				AuthenticationRequired = true
			};

			// Create a Trash action
			UIMutableUserNotificationAction trashAction = new UIMutableUserNotificationAction (){
				Identifier = "TRASH_ID",
				Title = "Trash",
				ActivationMode = UIUserNotificationActivationMode.Background,
				Destructive = true,
				AuthenticationRequired = true
			};

			// Create MonkeyMessage Category
			UIMutableUserNotificationCategory monkeyMessageCategory = new UIMutableUserNotificationCategory ();
			monkeyMessageCategory.Identifier = "MONKEYMESSAGE_ID";
			monkeyMessageCategory.SetActions (new UIUserNotificationAction[]{ acceptAction, replyAction, trashAction }, UIUserNotificationActionContext.Default);
			monkeyMessageCategory.SetActions (new UIUserNotificationAction[]{ acceptAction, trashAction }, UIUserNotificationActionContext.Minimal);

			// Create a category group
			NSSet categories = new NSSet(monkeyMessageCategory);

			// Set the requested notification types
			UIUserNotificationType type = UIUserNotificationType.Alert | UIUserNotificationType.Badge;

			// Create the setting for the given types
			UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(type, categories);

			// Register the settings
			UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);

			//Completed
			return true;
		}

		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			// Use notificationSettings.Types to test for allowed types

		}

		public override void HandleAction (UIApplication application, string actionIdentifier, UILocalNotification localNotification, Action completionHandler)
		{
			// Take action based on the notification and the action selected
			Console.WriteLine ("User selected Action '{0}' for Notification: {1}",actionIdentifier,localNotification.AlertBody);

		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}
		#endregion
	}
}

