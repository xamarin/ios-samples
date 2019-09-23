/*
See LICENSE folder for this sample’s licensing information.

Abstract:
The app delegate submits task requests and and registers the launch handlers for the app refresh and database cleaning background tasks.
*/

using System;
using System.Diagnostics;

using BackgroundTasks;
using Foundation;
using UIKit;

namespace ColorFeed {
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		// class-level declarations
		
		static readonly string caveat = "Change me!!";

		public static string RefreshTaskId { get; } = "com.xamarin.ColorFeed.refresh";
		public static NSString RefreshSuccessNotificationName { get; } = new NSString ($"{RefreshTaskId}.success");

		public static string CleaningDbTaskId { get; } = "com.xamarin.ColorFeed.cleaning_db";
		public static NSString CleaningDbSuccessNotificationName { get; } = new NSString ($"{CleaningDbTaskId}.success");

		IServer server;
		Operations operations;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			server = new MockServer ();
			operations = new Operations (server);

			var feedVC = (Window.RootViewController as UINavigationController).ViewControllers [0] as FeedTableViewController;
			feedVC.Operations = operations;
			operations.PostsDeleted += Operations_PostsDeleted;

			DBManager.SharedInstance.LoadInitialData ();

			// MARK: Registering Launch Handlers for Tasks
			// Downcast the parameter to an app refresh task as this identifier is used for a refresh request.
			BGTaskScheduler.Shared.Register (RefreshTaskId, null, task => HandleAppRefresh (task as BGAppRefreshTask));

			// Downcast the parameter to a processing task as this identifier is used for a processing request.
			BGTaskScheduler.Shared.Register (CleaningDbTaskId, null, task => HandleDatabaseCleaning (task as BGProcessingTask));

			return true;
		}

		public override void DidEnterBackground (UIApplication application)
		{
			ScheduleAppRefresh ();
			ScheduleDatabaseCleaningIfNeeded ();
		}

		#region Scheduling Tasks

		void ScheduleAppRefresh ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (RefreshSuccessNotificationName, RefreshSuccess);

			var request = new BGAppRefreshTaskRequest (RefreshTaskId) {
				EarliestBeginDate = (NSDate)DateTime.Now.AddMinutes (15) // Fetch no earlier than 15 minutes from now
			};

			BGTaskScheduler.Shared.Submit (request, out NSError error);

			if (error != null)
				Debug.WriteLine ($"Could not schedule app refresh: {error}");
		}

		void ScheduleDatabaseCleaningIfNeeded ()
		{
			var lastCleanDate = DBManager.SharedInstance.LastCleaned ?? DateTime.MinValue;
			var now = DateTime.Now;

			// Clean the database at most once per week.
			if (now <= lastCleanDate.AddDays (7))
				return;

			var request = new BGProcessingTaskRequest (CleaningDbTaskId) {
				RequiresNetworkConnectivity = false,
				RequiresExternalPower = true
			};

			BGTaskScheduler.Shared.Submit (request, out NSError error);

			if (error != null)
				Debug.WriteLine ($"Could not schedule app refresh: {error}");
		}

		#endregion

		#region Handling Launch for Tasks

		// Fetch the latest feed entries from server.
		void HandleAppRefresh (BGAppRefreshTask task)
		{
			ScheduleAppRefresh ();

			task.ExpirationHandler = () => operations.CancelOperations ();

			operations.FetchLatestPosts (task);
		}

		void RefreshSuccess (NSNotification notification)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (RefreshSuccessNotificationName);
			var task = notification.Object as BGAppRefreshTask;
			task?.SetTaskCompleted (true);
		}

		// Delete feed entries older than one day.
		void HandleDatabaseCleaning (BGProcessingTask task)
		{
			var beforeDate = DateTime.Now.AddDays (-1);

			task.ExpirationHandler = () => operations.CancelOperations ();

			operations.DeletePosts (beforeDate, task);
		}

		private void Operations_PostsDeleted (object sender, PostsDeletedEventArgs e)
		{
			DBManager.SharedInstance.LastCleaned = DateTime.Now;
			e.Task?.SetTaskCompleted (true);
		}

		#endregion
	}
}

