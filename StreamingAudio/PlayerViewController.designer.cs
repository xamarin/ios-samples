// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace StreamingAudio
{
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		UIKit.UILabel playbackTime { get; set; }

		[Outlet]
		UIKit.UIButton playPauseButton { get; set; }

		[Outlet]
		UIKit.UIProgressView progressBar { get; set; }

		[Outlet]
		UIKit.UILabel timeLabel { get; set; }

		[Outlet]
		UIKit.UISlider volumeSlider { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (playPauseButton != null) {
				playPauseButton.Dispose ();
				playPauseButton = null;
			}

			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}

			if (timeLabel != null) {
				timeLabel.Dispose ();
				timeLabel = null;
			}

			if (volumeSlider != null) {
				volumeSlider.Dispose ();
				volumeSlider = null;
			}

			if (playbackTime != null) {
				playbackTime.Dispose ();
				playbackTime = null;
			}
		}
	}
}
