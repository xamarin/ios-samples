// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace AVCompositionDebugVieweriOS
{
	[Register ("APLViewController")]
	partial class APLViewController
	{
		[Outlet]
		AVCompositionDebugVieweriOS.APLCompositionDebugView compositionDebugView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel currentTimeLabel { get; set; }

		[Outlet]
		AVCompositionDebugVieweriOS.APLPlayerView playerView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem playPauseButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider scrubber { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Action ("Click:")]
		partial void Click (MonoTouch.UIKit.UIBarButtonItem sender);
		
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
