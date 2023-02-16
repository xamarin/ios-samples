using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas {
	public partial class LoadingViewController : UIViewController {
		[Outlet]
		public UIActivityIndicatorView ActivityIndicator { get; set; }

		public Results Results { get; set; } = new Results ();
		public CodeSample CodeSample { get; set; }
		public NSError Error { get; set; }

		public LoadingViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public LoadingViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ActivityIndicator.StartAnimating ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var spinner = ActivityIndicator;
			if (spinner != null)
				spinner.StopAnimating ();

			if (segue.Identifier == "ShowResult") {
				var resultsViewController = segue.DestinationViewController as ResultsViewController;
				if (resultsViewController != null) {
					resultsViewController.CodeSample = CodeSample;
					resultsViewController.Results = (Results.Items.Count > 0) ? Results : new Results (new IResult [] { new NoResults () });
				}
			} else if (segue.Identifier == "ShowError") {
				var errorViewController = segue.DestinationViewController as ErrorViewController;
				if (errorViewController != null)
					errorViewController.Error = Error;
			}
		}
	}
}
