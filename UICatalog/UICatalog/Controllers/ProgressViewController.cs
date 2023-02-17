using System;
using System.Timers;
using UIKit;

namespace UICatalog {
	public partial class ProgressViewController : UITableViewController {
		private Timer timer = new Timer (500);

		public ProgressViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			defaultProgressView.SetProgress (0f, false);
			tintedProgressView.SetProgress (0f, false);
			barProgressView.SetProgress (0f, false);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			timer.Elapsed += OnTick;
			timer.Start ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			timer.Stop ();
			timer.Elapsed -= OnTick;
		}

		private void OnTick (object sender, ElapsedEventArgs e)
		{
			UpdateProgress ();
		}

		private void UpdateProgress ()
		{
			InvokeOnMainThread (() => {
				var value = (new Random ()).Next (1, 10) / 100f;
				var nextValue = defaultProgressView.Progress + value;
				if (nextValue > 1f) {
					nextValue = 1f;
					timer.Stop ();
					timer.Elapsed -= OnTick;
				}

				defaultProgressView.SetProgress (nextValue, true);
				tintedProgressView.SetProgress (nextValue, true);
				barProgressView.SetProgress (nextValue, true);
			});
		}
	}
}
