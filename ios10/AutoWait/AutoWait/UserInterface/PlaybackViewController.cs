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
		public AVPlayer Player {
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
					automaticWaitingSwitch.On = player.AutomaticallyWaitsToMinimizeStalling;
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
			if(player != null)
				player.AutomaticallyWaitsToMinimizeStalling = automaticWaitingSwitch.On;

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
			if(Player != null)
				Player.AutomaticallyWaitsToMinimizeStalling = automaticWaitingSwitch.On;
			NSUserDefaults.StandardUserDefaults.SetBool (automaticWaitingSwitch.On, "disableAutomaticWaiting");
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
			Player?.PlayImmediatelyAtRate(1);
		}

		#endregion
	}
}
