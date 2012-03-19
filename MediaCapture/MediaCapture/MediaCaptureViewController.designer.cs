// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MediaCapture
{
	[Register ("MediaCaptureViewController")]
	partial class MediaCaptureViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIToolbar mainToolBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem buttonSettings { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem buttonStartStop { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem buttonBrowse { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (mainToolBar != null) {
				mainToolBar.Dispose ();
				mainToolBar = null;
			}

			if (buttonSettings != null) {
				buttonSettings.Dispose ();
				buttonSettings = null;
			}

			if (buttonStartStop != null) {
				buttonStartStop.Dispose ();
				buttonStartStop = null;
			}

			if (buttonBrowse != null) {
				buttonBrowse.Dispose ();
				buttonBrowse = null;
			}
		}
	}
}
