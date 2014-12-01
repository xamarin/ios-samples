using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.Threading;

namespace BackgroundExecution
{
	public partial class HomeScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need
		// to be able to be created from a xib rather than from managed code

		public HomeScreen (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public HomeScreen (NSCoder coder) : base (coder)
		{
		}

		public HomeScreen () : base ("HomeScreen_iPhone", null)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.Title = "Background Execution";

			this.btnStartLongRunningTask.TouchUpInside += (s, e) => {
				ThreadStart ts = new ThreadStart (() => {
					this.DoSomething ();
				});
				ts.Invoke ();
			};
		}

		public void DoSomething ()
		{
			// register our background task
			int taskID = (int)UIApplication.SharedApplication.BeginBackgroundTask (() => {
				this.BackgroundTaskExpiring ();
			});

			Console.WriteLine ("Starting background task " + taskID.ToString ());

			// sleep for five seconds
			Thread.Sleep (5000);

			Console.WriteLine ("Background task " + taskID.ToString () + " completed.");

			// mark our background task as complete
			UIApplication.SharedApplication.EndBackgroundTask (taskID);
		}

		public void BackgroundTaskExpiring ()
		{
			Console.WriteLine ("Running out of time to complete you background task!");
		}
	}
}

