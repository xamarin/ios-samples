using System;

using UIKit;
using Foundation;
using AVFoundation;

namespace AutoWait
{
	public partial class PlaybackViewController : UIViewController
	{
		[Outlet ("playerView")]
		PlayerView PlayerView { get; set; }

		[Outlet ("waitingIndicatorView")]
		UIView waitingIndicatorView { get; set; }

		[Outlet ("pauseButton")]
		UIButton pauseButton { get; set; }

		[Outlet ("playButton")]
		UIButton playButton { get; set; }

		[Outlet ("playImmediatelyButton")]
		UIButton playImmediatelyButton { get; set; }

		[Outlet ("automaticWaitingSwitch")]
		UISwitch automaticWaitingSwitch { get; set; }

		AVPlayer player;
		AVPlayer Player {
			get {
				return player;
			}
			set {
				player = value;
				var playerView = PlayerView;
				if (playerView != null)
					playerView.Player = player;

				// Make sure the players automaticallyWaitsToMinimizeStalling follows the switch in the UI.
				if (player != null && IsViewLoaded)
					automaticWaitingSwitch.On = player.GetAutomaticallyWaitsToMinimizeStalling ();
			}
		}

		public PlaybackViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Load value for the automatic waiting switch from user defaults.
			automaticWaitingSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("disableAutomaticWaiting");
			player?.SetAutomaticallyWaitsToMinimizeStalling (automaticWaitingSwitch.On);

			var playerView = PlayerView;
			if (playerView != null)
				playerView.Player = player;

			// TODO: add observer or .net event
			// We will use this to toggle our waiting indicator view.
			//addObserver (self, forKeyPath: #keyPath(PlaybackViewController.player.reasonForWaitingToPlay), options: [.new, .initial], context: &observerContext)
		}

		protected override void Dispose (bool disposing)
		{
			// TODO: unsubscribe from #keyPath(PlaybackViewController.player.reasonForWaitingToPlay)
			base.Dispose (disposing);
		}

		#region User Actions

		[Action ("toggleAutomaticWaiting:")]
		void toggleAutomaticWaiting (UISwitch sender)
		{
			// Check for the new value of the switch and update AVPlayer property and user defaults
			var val = automaticWaitingSwitch.On;
			Player?.SetAutomaticallyWaitsToMinimizeStalling (val);
			NSUserDefaults.StandardUserDefaults.SetBool (val, "disableAutomaticWaiting");
		}

		[Action ("pause:")]
		void pause (NSObject sender)
		{
			Player?.Pause ();
		}

		[Action ("play:")]
		void Play (NSObject sender)
		{
			Player?.Play ();
		}

		[Action("playImmediately:")]
		void playImmediately (NSObject sender)
		{
			Player?.PlayImmediately (1);
		}





		#endregion
	}

	public static class AVPlayerExtensions
	{
		public static bool GetAutomaticallyWaitsToMinimizeStalling (this AVPlayer player)
		{
			// TODO: automaticallyWaitsToMinimizeStalling is not bound
			throw new NotImplementedException ();
		}

		public static void SetAutomaticallyWaitsToMinimizeStalling (this AVPlayer player, bool value)
		{
			// TODO: automaticallyWaitsToMinimizeStalling is not bound
			throw new NotImplementedException ();
		}
	}
}
