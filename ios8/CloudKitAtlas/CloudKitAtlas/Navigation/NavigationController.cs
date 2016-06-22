using System;

using UIKit;
using Foundation;
using CoreFoundation;

namespace CloudKitAtlas
{
	public partial class NavigationController : UINavigationController
	{
		public NavigationController (IntPtr handle)
			: base (handle)
		{
		}

		public async override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "ShowLoadingView")
				return;

			var arg = sender as SegueArg;
			if (arg == null)
				return;

			var selectedCodeSample = arg.Sample;
			if (selectedCodeSample == null)
				return;

			var navigationController = segue.DestinationViewController as UINavigationController;
			var loadingViewController = navigationController?.TopViewController as LoadingViewController;
			if (loadingViewController == null)
				return;

			string segueIdentifier = "ShowResult";

			try {
				var results = await selectedCodeSample.Run ();
				loadingViewController.Results = results;
				loadingViewController.CodeSample = selectedCodeSample;
			} catch (NSErrorException ex) {
				loadingViewController.Error = ex.Error;
				segueIdentifier = "ShowError";
			}


			DispatchQueue.MainQueue.DispatchAsync (() => {
				loadingViewController.PerformSegue (segueIdentifier, loadingViewController);
			});
		}
	}
}
