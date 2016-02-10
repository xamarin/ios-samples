using System;

using UIKit;

namespace MediaCapture {
	public class RootViewController : UINavigationController {
		MediaCaptureViewController mediaCaptureViewController;

		#region events
		public class ViewControllerPoppedEventArgs : EventArgs
		{
			public UIViewController Controller = null;
		}

		public event EventHandler<ViewControllerPoppedEventArgs> ViewControllerPopped;

		void OnViewControllerPopped (UIViewController controller)
		{
			if (ViewControllerPopped != null) {
				var args = new ViewControllerPoppedEventArgs {
					Controller = controller
				};
				ViewControllerPopped (this, args);
			}
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utilities.ShowLastErrorLog ();
			mediaCaptureViewController = new MediaCaptureViewController ();
			PushViewController (mediaCaptureViewController, true);
		}

		public override UIViewController PopViewController (bool animated)
		{
			UIViewController controller = base.PopViewController (animated);
			OnViewControllerPopped (controller);
			return controller;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

