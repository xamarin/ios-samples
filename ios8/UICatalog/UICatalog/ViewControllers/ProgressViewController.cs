using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class ProgressViewController : UITableViewController
	{
		private const int MaxProgress = 100;

		[Outlet]
		private UIProgressView DefaultStyleProgressView { get; set; }

		[Outlet]
		private UIProgressView BarStyleProgressView { get; set; }

		[Outlet]
		private UIProgressView TintedProgressView { get; set; }

		private int _completedProgress;
		private int CompletedProgress
		{
			get { return _completedProgress; }
			set {
				_completedProgress = value;

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
			SimulateProgress();
		}

		private void ConfigureDefaultStyleProgressView()
		{
			DefaultStyleProgressView.Style = UIProgressViewStyle.Default;
		}

		private void ConfigureBarStyleProgressView()
		{
			BarStyleProgressView.Style = UIProgressViewStyle.Bar;
		}

		private void ConfigureTintedProgressView()
		{
			TintedProgressView.Style = UIProgressViewStyle.Default;
			TintedProgressView.TrackTintColor = ApplicationColors.Blue;
			TintedProgressView.ProgressTintColor = ApplicationColors.Purple;
		}

		private void SimulateProgress()
		{
			// create completed task
			Task task = Task.FromResult<object> (null);

			Random rnd = new Random();

			// In this example we will simulate progress with a "sleep operation".
			foreach (int i in Enumerable.Range(0, MaxProgress)) {
				// Delay the system for a random number of seconds.
				// This code is _not_ intended for production purposes. The "sleep" call is meant to simulate work done in another subsystem.
				var delay = rnd.Next (100, 1000);

				// TaskScheduler.FromCurrentSynchronizationContext() – means run on main thread
				task = task.ContinueWith(_ => {
					Thread.Sleep(delay);
				}).ContinueWith (_ => {
						CompletedProgress++;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}
	}
}
