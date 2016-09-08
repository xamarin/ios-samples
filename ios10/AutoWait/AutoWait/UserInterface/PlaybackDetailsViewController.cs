using System;

using UIKit;
using Foundation;
using AVFoundation;

namespace AutoWait
{
	public partial class PlaybackDetailsViewController : UIViewController
	{
		[Outlet("playerView")]
		PlayerView PlayerView { get; set; }

		[Outlet("waitingIndicatorView")]
		UIView WaitingIndicatorView { get; set; }

		[Outlet ("pauseButton")]
		UIButton PauseButton { get; set; }

		[Outlet("playButton")]
		UIButton PlayButton { get; set; }

		[Outlet ("playImmediatelyButton")]
		UIButton PlayImmediatelyButton { get; set; }

		[Outlet ("automaticWaitingSwitch")]
		UISwitch AutomaticWaitingSwitch { get; set; }

		AVPlayer player;
		public AVPlayer Player {
			get {
				return player;
			}
			set {
				player = value;

				if (PlayerView != null)
					PlayerView.Player = player;

				// Make sure the players automaticallyWaitsToMinimizeStalling follows the switch in the UI.
				if (player != null && IsViewLoaded)
					AutomaticWaitingSwitch.On = player.AutomaticallyWaitsToMinimizeStalling;
			}
		}

		public PlaybackDetailsViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Load value for the automatic waiting switch from user defaults.
			AutomaticWaitingSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("disableAutomaticWaiting");
			if (Player != null)
				Player.AutomaticallyWaitsToMinimizeStalling = AutomaticWaitingSwitch.On;

			if (PlayerView != null)
				PlayerView.Player = player;

			// We will use this to toggle our waiting indicator view.
			// TODO: add observer
			//addObserver (self, forKeyPath: #keyPath(PlaybackViewController.player.reasonForWaitingToPlay), options: [.new, .initial], context: &observerContext)
		}

		protected override void Dispose (bool disposing)
		{
			// TODO: unsubscribe
			base.Dispose (disposing);
		}

		#region Actions

		[Action ("toggleAutomaticWaiting:")]
		void toggleAutomaticWaiting (UISwitch sender)
		{
			// Check for the new value of the switch and update AVPlayer property and user defaults
			if (Player != null)
				Player.AutomaticallyWaitsToMinimizeStalling = AutomaticWaitingSwitch.On;

			NSUserDefaults.StandardUserDefaults.SetBool (AutomaticWaitingSwitch.On, "disableAutomaticWaiting");
		}

		[Action ("pause:")]
		void Pause (NSObject sender)
		{
			Player?.Pause ();
		}

		[Action ("play:")]
		void Play (NSObject sender)
		{
			Player.Play ();
		}

		[Action ("playImmediately:")]
		void PlayImmediately (NSObject sender)
		{
			Player?.PlayImmediatelyAtRate (1);
		}

		#endregion

		// TODO: do KVO here
	}
}
