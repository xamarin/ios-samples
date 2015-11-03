// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MySingleView
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

		[Outlet]
		UIKit.UIProgressView ProgressBar { get; set; }

		[Outlet]
		UIKit.UIButton StartStopButton { get; set; }

		[Action ("LessAction:")]
		partial void LessAction (Foundation.NSObject sender);

		[Action ("MoreAction:")]
		partial void MoreAction (Foundation.NSObject sender);

		[Action ("StartStopAction:")]
		partial void StartStopAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}

			if (StartStopButton != null) {
				StartStopButton.Dispose ();
				StartStopButton = null;
			}
		}
	}
}
