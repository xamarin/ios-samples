// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace WatchL10n_iOSWatchKitExtension
{
	[Register ("DetailController")]
	partial class DetailController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		WatchKit.WKInterfaceImage displayImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		WatchKit.WKInterfaceLabel displayText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		WatchKit.WKInterfaceLabel displayTime { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (displayImage != null) {
				displayImage.Dispose ();
				displayImage = null;
			}
			if (displayText != null) {
				displayText.Dispose ();
				displayText = null;
			}
			if (displayTime != null) {
				displayTime.Dispose ();
				displayTime = null;
			}
		}
	}
}
