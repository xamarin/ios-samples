using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("ImageDetailController")]
	partial class ImageDetailController
	{
		[Outlet]
		WKInterfaceImage cachedImage { get; set; }

		[Outlet]
		WKInterfaceImage staticImage { get; set; }

		[Outlet]
		WKInterfaceImage animatedImage { get; set; }

		[Outlet]
		WKInterfaceButton playButton { get; set; }

		[Outlet]
		WKInterfaceButton stopButton { get; set; }

		[Action ("playAnimation:")]
		partial void PlayAnimation (NSObject obj);

		[Action ("stopAnimation:")]
		partial void StopAnimation (NSObject obj);

		void ReleaseDesignerOutlets ()
		{
			if (cachedImage != null) {
				cachedImage.Dispose ();
				cachedImage = null;
			}

			if (staticImage != null) {
				staticImage.Dispose ();
				staticImage = null;
			}

			if (animatedImage != null) {
				animatedImage.Dispose ();
				animatedImage = null;
			}

			if (playButton != null) {
				playButton.Dispose ();
				playButton = null;
			}

			if (stopButton != null) {
				stopButton.Dispose ();
				stopButton = null;
			}
		}
	}
}

