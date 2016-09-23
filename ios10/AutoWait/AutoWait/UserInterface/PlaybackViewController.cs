using System;

using AVFoundation;
using Foundation;
using UIKit;

namespace AutoWait
{
	public partial class PlaybackViewController : UIViewController
	{
		[Outlet ("playerView")]
		PlayerView PlayerView { get; set; }

		[Outlet ("waitingIndicatorView")]
		UIView WaitingIndicatorView { get; set; }

		[Outlet ("pauseButton")]
		UIButton PauseButton { get; set; }

		[Outlet ("playButton")]
		UIButton PlayButton { get; set; }

		[Outlet ("playImmediatelyButton")]
		UIButton PlayImmediatelyButton { get; set; }

		[Outlet ("automaticWaitingSwitch")]
		UISwitch AutomaticWaitingSwitch { get; set; }

		IDisposable reasonForWaitingToPlayToken;

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
					AutomaticWaitingSwitch.On = player.AutomaticallyWaitsToMinimizeStalling;
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
			AutomaticWaitingSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("disableAutomaticWaiting");
			if(Player != null)
				Player.AutomaticallyWaitsToMinimizeStalling = AutomaticWaitingSwitch.On;

			var playerView = PlayerView;
			if (playerView != null)
				playerView.Player = player;

			// We will use this to toggle our waiting indicator view.
			reasonForWaitingToPlayToken = Player?.AddObserver ("reasonForWaitingToPlay", NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, ReasonForWaitingToPlayChanged);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			reasonForWaitingToPlayToken?.Dispose ();
		}

		#region User Actions

		[Action ("toggleAutomaticWaiting:")]
		void toggleAutomaticWaiting (UISwitch sender)
		{
			// Check for the new value of the switch and update AVPlayer property and user defaults
			if(Player != null)
				Player.AutomaticallyWaitsToMinimizeStalling = AutomaticWaitingSwitch.On;
			NSUserDefaults.StandardUserDefaults.SetBool (AutomaticWaitingSwitch.On, "disableAutomaticWaiting");
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

		void ReasonForWaitingToPlayChanged (NSObservedChange obj)
		{
			// Hide the indicator view if we are not waiting to minimize stalls.
			WaitingIndicatorView.Hidden = (Player?.ReasonForWaitingToPlay != AVPlayer.WaitingToMinimizeStallsReason);
		}
	}
}
