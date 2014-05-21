// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AVCompositionDebugVieweriOS
{
	[Register ("APLViewController")]
	partial class APLViewController
	{
		[Outlet]
		AVCompositionDebugVieweriOS.APLCompositionDebugView compositionDebugView { get; set; }

		[Outlet]
		UIKit.UILabel currentTimeLabel { get; set; }

		[Outlet]
		AVCompositionDebugVieweriOS.APLPlayerView playerView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem playPauseButton { get; set; }

		[Outlet]
		UIKit.UISlider scrubber { get; set; }

		[Outlet]
		UIKit.UIToolbar toolbar { get; set; }

		[Action ("Click:")]
		partial void Click (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (compositionDebugView != null) {
				compositionDebugView.Dispose ();
				compositionDebugView = null;
			}

			if (currentTimeLabel != null) {
				currentTimeLabel.Dispose ();
				currentTimeLabel = null;
			}

			if (playerView != null) {
				playerView.Dispose ();
				playerView = null;
			}

			if (playPauseButton != null) {
				playPauseButton.Dispose ();
				playPauseButton = null;
			}

			if (scrubber != null) {
				scrubber.Dispose ();
				scrubber = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}
}
