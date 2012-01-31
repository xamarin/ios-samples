// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace StreamingAudio
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoTouch.UIKit.UIProgressView progress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIWindow window { get; set; }

		[Outlet]
		MonoTouch.UIKit.UINavigationController viewController { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField entry { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIViewController playController { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider volume { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel status { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton button { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel playbackTime { get; set; }

		[Action ("startPlayback:")]
		partial void startPlayback (MonoTouch.UIKit.UIButton sender);

		[Action ("playControlClicked:")]
		partial void playControlClicked (MonoTouch.UIKit.UIButton sender);

		[Action ("volumeSet:")]
		partial void volumeSet (MonoTouch.UIKit.UISlider sender);

		[Action ("startPlaybackAndSave:")]
		partial void startPlaybackAndSave (MonoTouch.UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (progress != null) {
				progress.Dispose ();
				progress = null;
			}

			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (viewController != null) {
				viewController.Dispose ();
				viewController = null;
			}

			if (entry != null) {
				entry.Dispose ();
				entry = null;
			}

			if (playController != null) {
				playController.Dispose ();
				playController = null;
			}

			if (volume != null) {
				volume.Dispose ();
				volume = null;
			}

			if (status != null) {
				status.Dispose ();
				status = null;
			}

			if (button != null) {
				button.Dispose ();
				button = null;
			}

			if (playbackTime != null) {
				playbackTime.Dispose ();
				playbackTime = null;
			}
		}
	}
}
