// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace AVCustomEdit
{
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UILabel currentTimeLabel { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem exportButton { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIProgressView exportProgressView { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UITapGestureRecognizer gestureRecognizer { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		AVCustomEdit.PlayerView playerView { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem playPauseButton { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UISlider scrubber { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIToolbar toolBar { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem transitionButton { get; set; }

		[Action ("beginScrubbing:")]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		partial void beginScrubbing (UISlider sender);

		[Action ("endScrubbing:")]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		partial void endScrubbing (UISlider sender);

		[Action ("exportToMovie:")]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		partial void exportToMovie (UIBarButtonItem sender);

		[Action ("scrub:")]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		partial void scrub (UISlider sender);

		[Action ("togglePlayPause:")]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		partial void togglePlayPause (UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
			if (currentTimeLabel != null) {
				currentTimeLabel.Dispose ();
				currentTimeLabel = null;
			}
			if (exportButton != null) {
				exportButton.Dispose ();
				exportButton = null;
			}
			if (exportProgressView != null) {
				exportProgressView.Dispose ();
				exportProgressView = null;
			}
			if (gestureRecognizer != null) {
				gestureRecognizer.Dispose ();
				gestureRecognizer = null;
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
			if (toolBar != null) {
				toolBar.Dispose ();
				toolBar = null;
			}
			if (transitionButton != null) {
				transitionButton.Dispose ();
				transitionButton = null;
			}
		}
	}
}
