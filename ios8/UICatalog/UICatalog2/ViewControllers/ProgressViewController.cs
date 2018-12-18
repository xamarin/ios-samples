using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("ProgressViewController")]
	public partial class ProgressViewController : UITableViewController
	{
		const int MaxProgress = 100;

		[Outlet]
		UIProgressView DefaultStyleProgressView { get; set; }

		[Outlet]
		UIProgressView BarStyleProgressView { get; set; }

		[Outlet]
		UIProgressView TintedProgressView { get; set; }

		int completedProgress;

		int CompletedProgress {
			get { return completedProgress; }
			set {
				completedProgress = value;

				float fractionalProgress = (float)CompletedProgress / (float)MaxProgress;

				// We only want to animate the progress if we're in the process of updating progress.
				bool animated = CompletedProgress != 0;

				DefaultStyleProgressView.SetProgress (fractionalProgress, animated);
				BarStyleProgressView.SetProgress (fractionalProgress, animated);
				TintedProgressView.SetProgress (fractionalProgress, animated);
			}
		}

		public ProgressViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// All progress views should initially have zero progress.
			CompletedProgress = 0;

			ConfigureDefaultStyleProgressView ();
			ConfigureBarStyleProgressView ();
			ConfigureTintedProgressView ();

			// As progress is received from another subsystem (i.e. NSProgress, NSURLSessionTaskDelegate, etc.), update the progressView's progress.
			SimulateProgress ();
		}

		void ConfigureDefaultStyleProgressView ()
		{
			DefaultStyleProgressView.Style = UIProgressViewStyle.Default;
		}

		void ConfigureBarStyleProgressView ()
		{
			BarStyleProgressView.Style = UIProgressViewStyle.Bar;
		}

		void ConfigureTintedProgressView ()
		{
			TintedProgressView.Style = UIProgressViewStyle.Default;
			TintedProgressView.TrackTintColor = ApplicationColors.Blue;
			TintedProgressView.ProgressTintColor = ApplicationColors.Purple;
		}

		void SimulateProgress ()
		{
			// create completed task
			Task task = Task.FromResult<object> (null);

			Random rnd = new Random ();

			// In this example we will simulate progress with a "sleep operation".
			foreach (int i in Enumerable.Range(0, MaxProgress)) {
				// Delay the system for a random number of seconds.
				// This code is _not_ intended for production purposes. The "sleep" call is meant to simulate work done in another subsystem.
				var delay = rnd.Next (100, 1000);

				// TaskScheduler.FromCurrentSynchronizationContext() – means run on main thread
				task = task.ContinueWith (_ => {
					Thread.Sleep (delay);
				}).ContinueWith (_ => {
					CompletedProgress++;
				}, TaskScheduler.FromCurrentSynchronizationContext ());
			}
		}
	}
}
