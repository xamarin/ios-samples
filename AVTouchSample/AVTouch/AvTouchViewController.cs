using AVFoundation;
using Foundation;
using System;
using System.IO;
using UIKit;

namespace AVTouch {
	public partial class AvTouchViewController : UIViewController {
		// amount to skip on rewind or fast forward
		private const float SKIP_TIME = 1f;
		// amount to play between skips
		private const float SKIP_INTERVAL = .2f;

		private AVAudioPlayer player;
		private NSTimer rewindTimer;
		private NSTimer forwardTimer;
		private NSTimer updateTimer;

		private bool inBackground;

		public AvTouchViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.updateTimer = null;
			this.rewindTimer = null;
			this.forwardTimer = null;

			// Load the the sample file, use mono or stero sample
			var fileUrl = NSBundle.MainBundle.PathForResource ("sample", "m4a");
			this.player = AVAudioPlayer.FromUrl (new NSUrl (fileUrl, false));
			if (this.player != null) {
				this.fileNameLabel.Text = $"Mono {Path.GetFileName (this.player.Url.RelativePath)} ({this.player.NumberOfChannels} ch)";
				this.UpdateViewForPlayerInfo (this.player);
				this.UpdateViewForPlayerState (this.player);
				this.player.NumberOfLoops = 1;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.RegisterForBackgroundNotifications ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}

		private void PausePlaybackForPlayer (AVAudioPlayer p)
		{
			p.Pause ();
			this.UpdateViewForPlayerState (p);
		}

		private void StartPlaybackForPlayer (AVAudioPlayer p)
		{
			if (p.Play ()) {
				this.UpdateViewForPlayerState (p);
			} else {
				Console.WriteLine ($"Could not play {p.Url}");
			}
		}

		partial void PlayButtonPressed (UIBarButtonItem sender)
		{
			if (this.player.Playing) {
				this.PausePlaybackForPlayer (this.player);
			} else {
				this.StartPlaybackForPlayer (this.player);
			}
		}

		partial void RewindButtonPressed (UIBarButtonItem sender)
		{
			if (this.rewindTimer != null) {
				this.DestroyTimer (this.rewindTimer);
			}

			this.rewindTimer = NSTimer.CreateRepeatingScheduledTimer (SKIP_INTERVAL, (timer) => this.Rewind (this.player));
		}

		partial void RewindButtonReleased (UIBarButtonItem sender)
		{
			if (this.rewindTimer != null) {
				this.DestroyTimer (this.rewindTimer);
				this.rewindTimer = null;
			}
		}

		partial void ForwardButtonPressed (UIBarButtonItem sender)
		{
			if (this.forwardTimer != null) {
				this.DestroyTimer (this.forwardTimer);
			}

			this.forwardTimer = NSTimer.CreateRepeatingScheduledTimer (SKIP_INTERVAL, (timer) => this.Forward (this.player));
		}

		partial void ForwardButtonReleased (UIBarButtonItem sender)
		{
			if (this.forwardTimer != null) {
				this.DestroyTimer (this.forwardTimer);
				this.forwardTimer = null;
			}
		}

		partial void VolumeSliderMoved (UISlider sender)
		{
			this.player.Volume = sender.Value;
		}

		partial void ProgressSliderMoved (UISlider sender)
		{
			this.player.CurrentTime = sender.Value;
			this.UpdateCurrentTimeForPlayer (this.player);
		}

		private void UpdateCurrentTimeForPlayer (AVAudioPlayer p)
		{
			this.currentTimeLabel.Text = TimeSpan.FromSeconds (player.CurrentTime).ToString (@"mm\:ss");
			this.progressSlider.Value = (float) p.CurrentTime;
		}

		private void UpdateCurrentTime (AVAudioPlayer p)
		{
			this.UpdateCurrentTimeForPlayer (p);
		}

		private void UpdateViewForPlayerState (AVAudioPlayer p)
		{
			this.UpdateCurrentTimeForPlayer (p);

			if (this.updateTimer != null) {
				this.DestroyTimer (this.updateTimer);
				this.updateTimer = null;
			}

			this.UpdatePlayButtonState (p);

			if (p.Playing) {
				this.lvlMeter.Player = p;
				this.updateTimer = NSTimer.CreateRepeatingScheduledTimer (.01f, (timer) => this.UpdateCurrentTime (p));
			} else {
				this.lvlMeter.Player = null;
				this.updateTimer = null;
			}
		}

		private void UpdateViewForPlayerStateInBackground (AVAudioPlayer p)
		{
			this.UpdateCurrentTimeForPlayer (p);
			this.UpdatePlayButtonState (p);
		}

		private void UpdatePlayButtonState (AVAudioPlayer p)
		{
			var style = p.Playing ? UIBarButtonSystemItem.Pause : UIBarButtonSystemItem.Play;
			using (var playButton = new UIBarButtonItem (style, (sender, e) => this.PlayButtonPressed (sender as UIBarButtonItem))) {
				var items = this.toolbar.Items;
				items [3] = playButton;
				this.toolbar.Items = items;
			}
		}

		private void UpdateViewForPlayerInfo (AVAudioPlayer p)
		{
			this.durationLabel.Text = TimeSpan.FromSeconds (p.Duration).ToString (@"mm\:ss");
			this.progressSlider.MaxValue = (float) p.Duration;
			this.volumeSlider.Value = p.Volume;
		}

		private void Rewind (AVAudioPlayer p)
		{
			p.CurrentTime -= SKIP_TIME;
			this.UpdateCurrentTimeForPlayer (p);
		}

		private void Forward (AVAudioPlayer p)
		{
			p.CurrentTime += SKIP_TIME;
			this.UpdateCurrentTimeForPlayer (p);
		}

		#region AVAudioPlayer delegate methods

		[Export ("audioPlayerDidFinishPlaying:successfully:")]
		public void FinishedPlaying (AVAudioPlayer p, bool flag)
		{
			if (!flag) {
				Console.WriteLine (@"Playback finished unsuccessfully");
			}

			p.CurrentTime = 0d;
			if (this.inBackground) {
				this.UpdateViewForPlayerStateInBackground (p);
			} else {
				this.UpdateViewForPlayerState (p);
			}
		}

		[Export ("audioPlayerDecodeErrorDidOccur:error:")]
		public void DecoderError (AVAudioPlayer player, NSError error)
		{
			Console.WriteLine ($"ERROR IN DECODE: {error}");
		}

		// We will only get these notifications if playback was interrupted
		[Export ("audioPlayerBeginInterruption:")]
		public void BeginInterruption (AVAudioPlayer p)
		{
			Console.WriteLine ("Interruption begin. Updating UI for new state");
			// the object has already been paused,  we just need to update UI
			if (this.inBackground) {
				this.UpdateViewForPlayerStateInBackground (p);
			} else {
				this.UpdateViewForPlayerState (p);
			}
		}

		[Export ("audioPlayerEndInterruption:")]
		public void EndInterruption (AVAudioPlayer p)
		{
			Console.WriteLine ("Interruption ended. Resuming playback");
			StartPlaybackForPlayer (p);
		}

		#endregion

		#region mark background notifications

		private void RegisterForBackgroundNotifications ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillResignActiveNotification, SetInBackgroundFlag);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillEnterForegroundNotification, ClearInBackgroundFlag);
		}

		private void SetInBackgroundFlag (NSNotification notification)
		{
			this.inBackground = true;
		}

		private void ClearInBackgroundFlag (NSNotification notification)
		{
			this.inBackground = false;
		}

		#endregion

		private void DestroyTimer (NSTimer timer)
		{
			timer.Invalidate ();
			timer.Dispose ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (this.player != null) {
				this.player.Dispose ();
				this.player = null;
			}

			if (this.rewindTimer != null) {
				this.DestroyTimer (this.rewindTimer);
				this.rewindTimer = null;
			}

			if (this.forwardTimer != null) {
				this.DestroyTimer (this.forwardTimer);
				this.forwardTimer = null;
			}

			if (this.updateTimer != null) {
				this.DestroyTimer (this.updateTimer);
				this.updateTimer = null;
			}
		}
	}
}
