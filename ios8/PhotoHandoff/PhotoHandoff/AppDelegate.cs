using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace PhotoHandoff
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		DataSource dataSource;

		public override UIWindow Window { get; set; }

		UINavigationController NavigationController {
			get {
				return (UINavigationController)Window.RootViewController;
			}
		}

		CollectionViewController PrimaryViewController {
			get {
				return (CollectionViewController)NavigationController.ViewControllers[0];
			}
		}

		public override bool WillFinishLaunching (UIApplication application, NSDictionary launchOptions)
		{
			dataSource = new DataSource ();
			UIApplication.RegisterObjectForStateRestoration (dataSource, "DataSource");

			PrimaryViewController.DataSource = dataSource;

			Window.MakeKeyAndVisible ();

			return true;
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// any app setup done here after state restoration has occurred
			return true;
		}

		#region NSUserActivity

		public override bool WillContinueUserActivity (UIApplication application, string userActivityType)
		{
			PrimaryViewController.DataSource = dataSource;

			PrimaryViewController.PrepareForActivity ();
			return true;
		}

		// Called on the main thread after the NSUserActivity object is available.
		// Use the data you stored in the NSUserActivity object to re-create what the user was doing.
		// You can create/fetch any restorable objects associated with the user activity, and pass them to the restorationHandler.
		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			var vc = PrimaryViewController;

			vc.HandleUserActivity (userActivity);
			completionHandler(new NSObject[]{ NavigationController, vc});

			return true;
		}

		// If the user activity cannot be fetched after WillContinueUserActivity is called,
		// this will be called on the main thread when implemented.
		public override void DidFailToContinueUserActivitiy (UIApplication application, string userActivityType, NSError error)
		{
			PrimaryViewController.HandleActivityFailure ();
		}

		// This is called on the main thread when a user activity managed by UIKit has been updated.
		// You can use this as a last chance to add additional data to the userActivity.
		public override void UserActivityUpdated (UIApplication application, NSUserActivity userActivity)
		{
			Console.WriteLine ("UserActivityUpdated");
		}

		#endregion

		#region UIStateRestoration

		public override bool ShouldSaveApplicationState (UIApplication application, NSCoder coder)
		{
			return true;
		}

		public override bool ShouldRestoreApplicationState (UIApplication application, NSCoder coder)
		{
			return true;
		}

		#endregion
	}
}

