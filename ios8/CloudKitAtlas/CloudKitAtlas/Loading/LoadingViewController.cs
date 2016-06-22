using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class LoadingViewController : UIViewController
	{
		public UIActivityIndicatorView ActivityIndicator { get; set; }

		readonly Results results = new Results ();

		CodeSample codeSample;
		NSError error;

		public LoadingViewController (IntPtr handle)
			: base (handle)
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
					resultsViewController.CodeSample = codeSample;
					resultsViewController.Results = (results.Items.Count > 0) ? results : new Results (new IResult [] { new NoResults () });
				}
			} else if (segue.Identifier == "ShowError") {
				var errorViewController = segue.DestinationViewController as ErrorViewController;
				if (errorViewController != null)
					errorViewController.Error = error;
			}
		}
	}
}
