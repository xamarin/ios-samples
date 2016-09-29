using System;

using AVFoundation;
using Foundation;
using UIKit;

namespace AutoWait
{
	public partial class MediaViewController : UIViewController
	{
		[Outlet("stackView")]
		UIStackView stackView { get; set; }

		PlaybackViewController playbackViewController;
		PlaybackDetailsViewController playbackDetailsViewController;
		readonly AVPlayer player = new AVPlayer ();

		NSUrl mediaUrl;
		public NSUrl MediaUrl {
			get {
				return mediaUrl;
			}
			set {
				mediaUrl = value;
				// Create a new item for our AVPlayer
				UpdatePlayerItem ();
			}
		}

		public MediaViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Make sure our AVPlayer has an AVPlayerItem if we already got a URL
			UpdatePlayerItem ();

			// Setup sub-view controllers.
			// 1) A PlaybackViewController for the video and playback controls.
			playbackViewController = (PlaybackViewController)Storyboard.InstantiateViewController ("Playback");
			playbackViewController.Player = player;

			// 2) A PlaybackDetailsViewController for property values.
			playbackDetailsViewController = (PlaybackDetailsViewController)Storyboard.InstantiateViewController ("PlaybackDetails");
			playbackDetailsViewController.Player = player;

			// Add both new views to our stackView.
			stackView.AddArrangedSubview (playbackViewController.View);
			stackView.AddArrangedSubview (playbackDetailsViewController.View);
		}

		void UpdatePlayerItem ()
		{
			var url = MediaUrl;
			var playerItem = (url != null) ? new AVPlayerItem (url) : null;
			player.ReplaceCurrentItemWithPlayerItem (playerItem);
		}
	}
}
