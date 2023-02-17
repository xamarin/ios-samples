
namespace SimpleWatchConnectivity.WatchKitExtension {
	using CoreFoundation;
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using WatchConnectivity;
	using WatchKit;

	/// <summary>
	/// The extension delegate of the WatchKit extension.
	/// </summary>
	[Register ("ExtensionDelegate")]
	public class ExtensionDelegate : WKExtensionDelegate {
		private readonly SessionDelegater sessionDelegater = new SessionDelegater ();

		// An array to keep the background tasks.
		private readonly List<WKWatchConnectivityRefreshBackgroundTask> wcBackgroundTasks = new List<WKWatchConnectivityRefreshBackgroundTask> ();

		// Hold the KVO observers as we want to keep oberving in the extension life time.
		private readonly IDisposable activationStateObservation;
		private readonly IDisposable hasContentPendingObservation;

		public ExtensionDelegate () : base ()
		{
			System.Diagnostics.Debug.Assert (WCSession.IsSupported, "This sample requires a platform supporting Watch Connectivity!");
			if (string.IsNullOrEmpty (WatchSettings.SharedContainerId)) {
				Console.WriteLine ("Specify a shared container ID for WatchSettings.SharedContainerID to use watch settings!");
			}

			// WKWatchConnectivityRefreshBackgroundTask should be completed – Otherwise they will keep consuming
			// the background executing time and eventually causes an app crash.
			// The timing to complete the tasks is when the current WCSession turns to not .activated or
			// hasContentPending flipped to false (see completeBackgroundTasks), so KVO is set up here to observe
			// the changes if the two properties.
			this.activationStateObservation = WCSession.DefaultSession.AddObserver ("activationState", NSKeyValueObservingOptions.New, (_) => {
				DispatchQueue.MainQueue.DispatchAsync (() => this.CompleteBackgroundTasks ());
			});

			this.hasContentPendingObservation = WCSession.DefaultSession.AddObserver ("hasContentPending", NSKeyValueObservingOptions.New, (_) => {
				DispatchQueue.MainQueue.DispatchAsync (() => this.CompleteBackgroundTasks ());
			});

			// Activate the session asynchronously as early as possible.
			// In the case of being background launched with a task, this may save some background runtime budget.
			WCSession.DefaultSession.Delegate = this.sessionDelegater;
			WCSession.DefaultSession.ActivateSession ();
		}

		/// <summary>
		/// Compelete the background tasks, and schedule a snapshot refresh.
		/// </summary>
		private void CompleteBackgroundTasks ()
		{
			if (this.wcBackgroundTasks.Any ()) {
				if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated &&
				   !WCSession.DefaultSession.HasContentPending) {
					foreach (var item in this.wcBackgroundTasks) {
						item.SetTaskCompleted (false);
					}

					// Use Logger to log the tasks for debug purpose. A real app may remove the log
					// to save the precious background time.
					Logger.Shared.Append ($"'CompleteBackgroundTasks':{wcBackgroundTasks} was completed!");

					// Schedule a snapshot refresh if the UI is updated by background tasks.
					var date = NSDate.FromTimeIntervalSinceNow (1d);
					WKExtension.SharedExtension.ScheduleSnapshotRefresh (date, null, (error) => {
						if (error != null) {
							Console.WriteLine ($"scheduleSnapshotRefresh error: {error}");
						}
					});

					this.wcBackgroundTasks.Clear ();
				}
			}
		}

		public override void HandleBackgroundTasks (NSSet<WKRefreshBackgroundTask> backgroundTasks)
		{
			foreach (WKRefreshBackgroundTask task in backgroundTasks) {
				// Use Logger to log the tasks for debug purpose. A real app may remove the log
				// to save the precious background time.
				if (task is WKWatchConnectivityRefreshBackgroundTask wcTask) {
					this.wcBackgroundTasks.Add (wcTask);
					Logger.Shared.Append ($"'HandleBackgroundTasks':{wcTask.Description} was appended!");
				} else {
					task.SetTaskCompleted (false);
					Logger.Shared.Append ($"'HandleBackgroundTasks':{task.Description} was completed!");
				}
			}

			this.CompleteBackgroundTasks ();
		}
	}
}
