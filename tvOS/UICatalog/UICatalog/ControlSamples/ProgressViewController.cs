using System;
using System.Timers;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class ProgressViewController : UIViewController {

		bool disposed;
		IntPtr progressViewKVOContext = IntPtr.Zero;
		Timer timer;
		NSProgress progress = new NSProgress {
			TotalUnitCount = 10
		};

		[Export ("initWithCoder:")]
		public ProgressViewController (NSCoder coder): base (coder)
		{
			Initialize ();
		}

		public void Initialize ()
		{
			progress.AddObserver (this, "fractionCompleted", NSKeyValueObservingOptions.New, progressViewKVOContext);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposed)
				return; 

			if (disposing)
				progress.RemoveObserver (this, "fractionCompleted");

			disposed = true;
			base.Dispose (disposing);
		}

		public override void ViewDidAppear (bool animated)
		{
			foreach (var progressView in ProgressViews)
				progressView.SetProgress (0f, false);

			progress.CompletedUnitCount = 0;
			timer = new Timer (1000);
			timer.Elapsed += TimeDidFire;
			timer.Enabled = true;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			timer.Stop ();
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (context == progressViewKVOContext && keyPath == "fractionCompleted" && ofObject == progress) {
				InvokeOnMainThread (() => {
					foreach (var progressView in ProgressViews)
						progressView.SetProgress ((float)progress.FractionCompleted, true);
				});
			} else {
				base.ObserveValue (keyPath, ofObject, change, context);
			}
		}

		void TimeDidFire (object sender, ElapsedEventArgs e)
		{
			if (progress.CompletedUnitCount < progress.TotalUnitCount)
				progress.CompletedUnitCount += 1;
			else
				timer.Stop ();
		}
	}
}
