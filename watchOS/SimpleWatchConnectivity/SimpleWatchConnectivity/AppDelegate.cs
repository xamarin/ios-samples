
namespace SimpleWatchConnectivity {
	using Foundation;
	using System;
	using UIKit;
	using WatchConnectivity;

	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		private readonly SessionDelegater sessionDelegater = new SessionDelegater ();

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Trigger WCSession activation at the early phase of app launching.
			System.Diagnostics.Debug.Assert (WCSession.IsSupported, "This sample requires Watch Connectivity support!");
			WCSession.DefaultSession.Delegate = this.sessionDelegater;
			WCSession.DefaultSession.ActivateSession ();

			// Remind the setup of WatchSettings.sharedContainerID.
			if (string.IsNullOrEmpty (WatchSettings.SharedContainerId)) {
				Console.WriteLine ("Specify a shared container ID for WatchSettings.sharedContainerID to use watch settings!");
			}

			return true;
		}
	}
}
