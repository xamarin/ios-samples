// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SimpleBackgroundTransfer
{
	[Register ("SimpleBackgroundTransferViewController")]
	partial class SimpleBackgroundTransferViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem crashButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIProgressView progressView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem startButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (crashButton != null) {
				crashButton.Dispose ();
				crashButton = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (progressView != null) {
				progressView.Dispose ();
				progressView = null;
			}

			if (startButton != null) {
				startButton.Dispose ();
				startButton = null;
			}
		}
	}
}
