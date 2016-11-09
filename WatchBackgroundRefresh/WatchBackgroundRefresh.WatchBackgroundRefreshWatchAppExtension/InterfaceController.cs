using System;
using ObjCRuntime;
using WatchKit;
using Foundation;
using System.Collections.Generic;

namespace WatchBackgroundRefresh.WatchBackgroundRefreshWatchAppExtension
{
	public partial class InterfaceController : WKInterfaceController, IWKExtensionDelegate, INSUrlSessionDownloadDelegate
	{

		private NSUrl sampleDownloadURL = new NSUrl("http://devstreaming.apple.com/videos/wwdc/2015/802mpzd3nzovlygpbg/802/802_designing_for_apple_watch.pdf?dl=1");

		private NSDateFormatter dateFormatter = new NSDateFormatter()
		{
			DateStyle = NSDateFormatterStyle.None,
			TimeStyle = NSDateFormatterStyle.Long
		};

		protected InterfaceController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake(NSObject context)
		{
			base.Awake(context);
			WKExtension.SharedExtension.Delegate = this;
			updateLabel();

		}

		//WKExtensionDelegate

		[Export("handleBackgroundTasks:")]
		public void HandleBackgroundTasks(NSSet<WKRefreshBackgroundTask> backgroundTasks)
		{
			foreach (WKRefreshBackgroundTask task in backgroundTasks)
			{
				Console.WriteLine($"received background task: {task}");
				// only handle these while running in the background
				if (WKExtension.SharedExtension.ApplicationState == WKApplicationState.Background)
				{
					if (task is WKApplicationRefreshBackgroundTask)
					{
						// this task is completed below, our app will then suspend while the download session runs
						Console.WriteLine("application task received, start URL session");
						scheduleURLSession();
					}

				}
				else if (task is WKUrlSessionRefreshBackgroundTask)
				{
					var backgroundConfigObject = NSUrlSessionConfiguration.BackgroundSessionConfiguration(((WKUrlSessionRefreshBackgroundTask)task).SessionIdentifier);
					var backgroundSession = NSUrlSession.FromWeakConfiguration(backgroundConfigObject, this, null);
					Console.WriteLine($"Rejoining session {backgroundSession}");
				}
				task.SetTaskCompleted();
			}

		}


		//Snapshot and UI updating
		private void scheduleSnapshot()
		{
			// fire now, we're ready
			var fireDate = DateTime.Now;
			WKExtension.SharedExtension.ScheduleSnapshotRefresh((NSDate)fireDate, null, (NSError error) =>
			  {
				  if (error == null)
				  {
					  Console.WriteLine("successfully scheduled snapshot.  All background work completed.");
				  }
			  });

		}

		private void updateLabel()
		{

			var now = DateTime.Now;
			timeDisplayLabel.SetText(dateFormatter.ToString((NSDate)now));
		}


		//URLSession handling

		[Export("URLSession:downloadTask:didFinishDownloadingToURL:")]
		public void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
		{
			Console.WriteLine($"NSURLSession finished to url: {location}");
			updateLabel();
			scheduleSnapshot();
		}



		private void scheduleURLSession()
		{
			var uuuid = new NSUuid();
			var backgroundConfigObject = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(uuuid.AsString());
			backgroundConfigObject.SessionSendsLaunchEvents = true;
			var backgroundSession = NSUrlSession.FromConfiguration(backgroundConfigObject);
			var downloadTask = backgroundSession.CreateDownloadTask(sampleDownloadURL);
			downloadTask.Resume();
		}


		partial void ScheduleRefreshButtonTapped()
		{

			// fire in 20 seconds
			var fireDate = DateTime.Now.AddSeconds(20);

			// optional, any SecureCoding compliant data can be passed here
			var userInfo = NSDictionary.FromObjectAndKey((NSString)"background update", (NSString)"reason");

			WKExtension.SharedExtension.ScheduleBackgroundRefresh((NSDate)fireDate, userInfo, (NSError error) =>
			  {
				  if (error == null)
				  {
					  Console.WriteLine("successfully scheduled background task, use the crown to send the app to the background and wait for handle:BackgroundTasks to fire.");
				  }
			  });

		}
	}
}