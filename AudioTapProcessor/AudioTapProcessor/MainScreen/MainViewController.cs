using System;

using UIKit;
using Foundation;
using AVFoundation;
using AssetsLibrary;
using System.Linq;
using CoreMedia;
using CoreFoundation;
using System.Threading;

namespace AudioTapProcessor
{
	[Register ("MainViewController")]
	public class MainViewController : UIViewController
	{
		static NSUrl movieUrl;

		NSObject playerTimeObserver;
		NSObject playerItemDidPlayToEndTimeObserver;

		AVPlayer player;

		public AVPlayer Player {
			get {
				return player = player ?? new AVPlayer (movieUrl);
			}
		}

		bool IsStopped {
			get {
				return Player.Rate == 0;
			}
		}

		AudioTapProcessor audioTapProcessor;

		public AudioTapProcessor AudioTapProcessor {
			get {
				return audioTapProcessor = audioTapProcessor ?? CreateTapProcessor ();
			}
		}

		[Outlet ("playerView")]
		PlayerView PlayerView { get; set; }

		[Outlet ("leftChannelVolumeUnitMeterView")]
		VolumeUnitMeterView LeftChannelVolumeUnitMeterView { get; set; }

		[Outlet ("rightChannelVolumeUnitMeterView")]
		VolumeUnitMeterView RightChannelVolumeUnitMeterView { get; set; }

		[Outlet ("playPauseButton")]
		UIButton PlayPauseButton { get; set; }

		[Outlet ("elapsedTimeLabel")]
		UILabel ElapsedTimeLabel { get; set; }

		[Outlet ("currentTimeSlider")]
		UISlider CurrentTimeSlider { get; set; }

		[Outlet ("remainingTimeLabel")]
		UILabel RemainingTimeLabel { get; set; }

		[Outlet ("settingsPopoverButton")]
		UIButton SettingsPopoverButton { get; set; }

		static MainViewController ()
		{
			movieUrl = fetchMovieUrl ();
		}

		public MainViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Attach player to player view.
			PlayerView.Player = Player;

			// Disable play pause button and current time slider (until player is ready to play).
			PlayPauseButton.Enabled = false;
			CurrentTimeSlider.Enabled = false;

			// Disable settings popover button (until audio tap is created).
			SettingsPopoverButton.Enabled = false;

			// Start observing player's status.
			// TODO: https://trello.com/c/SeE2FBoN
			Player.AddObserver (this, "status", NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, IntPtr.Zero);

			// Add player item did play to end time observer.
			AVPlayerItem.Notifications.ObserveDidPlayToEndTime (DidPlayToEndTime);
		}

		void DidPlayToEndTime (object sender, NSNotificationEventArgs args)
		{
			if (args.Notification.Object != Player.CurrentItem)
				return;

			// Update play pause button.
			PlayPauseButton.SetImage (UIImage.FromBundle ("PlayButton"), UIControlState.Normal);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "ShowSettingsSegue")
				SetupSettingsViewController (segue);
		}

		void SetupSettingsViewController (UIStoryboardSegue segue)
		{
			// Setup settings view controller before it is shown.
			UIViewController topViewController = ((UINavigationController)segue.DestinationViewController).TopViewController;
			var settingsViewController = (SettingsViewController)topViewController;

			settingsViewController.Controller = this;
			settingsViewController.EnabledSwitchValue = AudioTapProcessor.IsBandpassFilterEnabled;
			settingsViewController.CenterFrequencySliderValue = AudioTapProcessor.CenterFrequency;
			settingsViewController.BandwidthSliderValue = AudioTapProcessor.Bandwidth;
		}

		// TODO: https://trello.com/c/SeE2FBoN
		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (FetchStatus (change) == AVPlayerStatus.ReadyToPlay)
				OnReadyToPlay ();
		}

		AVPlayerStatus FetchStatus (NSDictionary change)
		{
			var ch = new NSObservedChange (change);
			var newValue = (NSNumber)ch.NewValue;
			return newValue == null ? AVPlayerStatus.Unknown : (AVPlayerStatus)newValue.Int32Value;
		}

		void OnReadyToPlay ()
		{
			CMTime duration = Player.CurrentItem.Duration;
			ResetUpdateTimeIndicators (duration);

			// Add time observer for current time slider and label.
			playerTimeObserver = Player.AddPeriodicTimeObserver (new CMTime (1, 1), DispatchQueue.MainQueue, time =>
				UpdateTimeIndicators (time, duration)
			);

			// Add audio mix with audio tap processor to current player item.
			TryAddAudioMix ();

			// Enable play pause button and current time slider.
			PlayPauseButton.Enabled = true;
			CurrentTimeSlider.Enabled = true;
		}

		void ResetUpdateTimeIndicators (CMTime duration)
		{
			ElapsedTimeLabel.Text = BuildTimeString (CMTime.Zero);
			CurrentTimeSlider.Value = 0;
			RemainingTimeLabel.Text = BuildTimeString (duration, "-");
		}

		void UpdateTimeIndicators (CMTime time, CMTime duration)
		{
			ElapsedTimeLabel.Text = BuildTimeString (time);
			CurrentTimeSlider.Value = (float)(time.Seconds / duration.Seconds);
			RemainingTimeLabel.Text = BuildTimeString (duration - time, "-");
		}

		void TryAddAudioMix ()
		{
			AVAudioMix audioMix = AudioTapProcessor.AudioMix;
			if (audioMix == null)
				return;

			// Add audio mix with first audio track.
			Player.CurrentItem.AudioMix = audioMix;

			// Enable settings popover button.
			SettingsPopoverButton.Enabled = true;
		}

		#region Actions

		[Export ("togglePlayPause:")]
		void TogglePlayPause (NSObject sender)
		{
			AssertPlayerNotNull ();

			if (IsStopped) {
				// Play from beginning when playhead is at end.
				TryResetPlayhead ();
				StartPlayback ();
			} else {
				StopPlayback ();
			}
		}

		void AssertPlayerNotNull ()
		{
			if (Player == null)
				throw new InvalidOperationException ("Player is null");
		}

		void TryResetPlayhead ()
		{
			AVPlayerItem cItem = Player.CurrentItem;
			if (cItem.CurrentTime >= cItem.Duration)
				cItem.Seek (CMTime.Zero, CMTime.Zero, CMTime.Zero);
		}

		void StartPlayback ()
		{
			player.Rate = 1;
			PlayPauseButton.SetImage (UIImage.FromBundle ("PauseButton"), UIControlState.Normal);
		}

		void StopPlayback ()
		{
			Player.Rate = 0;
			PlayPauseButton.SetImage (UIImage.FromBundle ("PlayButton"), UIControlState.Normal);
		}

		[Export ("seekToTime:")]
		void SeekToTime (UISlider slider)
		{
			AssertPlayerNotNull ();
			Player.Seek (Player.CurrentItem.Duration * slider.Value, CMTime.Zero, CMTime.Zero);
		}

		#endregion

		public void OnNewLeftRightChanelValue (AudioTapProcessor processor, float lChannelValue, float rChannelValue)
		{
			// Update left and right channel volume unit meter.
			LeftChannelVolumeUnitMeterView.Value = lChannelValue;
			RightChannelVolumeUnitMeterView.Value = rChannelValue;
		}

		#region SettingsViewController callbacks

		public void DidUpdateEnabledSwitchValue (bool enable)
		{
			// Forward value to audio tap processor.
			AudioTapProcessor.IsBandpassFilterEnabled = enable;
		}

		public void DidUpdateCenterFrequencySliderValue (float sliderValue)
		{
			// Forward value to audio tap processor.
			AudioTapProcessor.CenterFrequency = sliderValue;
		}

		public void DidUpdateBandwidthSliderValue (float sliderValue)
		{
			// Forward value to audio tap processor.
			AudioTapProcessor.Bandwidth = sliderValue;
		}

		#endregion

		static NSUrl fetchMovieUrl ()
		{
			NSUrl movieURL = null;
			var waitHandle = new AutoResetEvent (false);

			ThreadPool.QueueUserWorkItem (_ => {
				ALAssetsLibrary assetsLibrary = new ALAssetsLibrary ();
				assetsLibrary.Enumerate (ALAssetsGroupType.All, (ALAssetsGroup group, ref bool stop) => {
					if (group == null) {
						waitHandle.Set ();
						return;
					}

					NSDate latest = NSDate.FromTimeIntervalSince1970(0);
					group.Enumerate ((ALAsset result, nint index, ref bool stopGroup) => {
						if(result == null || result.AssetType != ALAssetType.Video)
							return;

						NSUrl url = result.DefaultRepresentation.Url;
						if (url == null)
							return;

						var diff = result.Date.SecondsSinceReferenceDate - latest.SecondsSinceReferenceDate;
						if(diff > 0) {
							latest = result.Date;
							movieURL = url;
						}
					});

					stop = movieURL != null;
				}, (error) => {
					waitHandle.Set ();
				});
			});

			waitHandle.WaitOne ();

			if (movieURL == null) {
				UIAlertView alertView = new UIAlertView (null, "Could not find any movies in assets library to use as sample content.", null, "OK", null);
				alertView.Show ();
			}

			return movieURL;
		}

		AudioTapProcessor CreateTapProcessor ()
		{
			AudioTapProcessor processor = null;

			AVAssetTrack firstAudioAssetTrack = FetchFirstAudioTrack (Player);
			if (firstAudioAssetTrack != null) {
				processor = new AudioTapProcessor (firstAudioAssetTrack);
				processor.Controller = this;
			}

			return processor;
		}

		protected override void Dispose (bool disposing)
		{
			playerTimeObserver.Dispose ();
			playerItemDidPlayToEndTimeObserver.Dispose ();

			base.Dispose (disposing);
		}

		static AVAssetTrack FetchFirstAudioTrack (AVPlayer player)
		{
			AVAssetTrack[] tracks = player.CurrentItem.Asset.Tracks;
			AVAssetTrack firstAudioTrack = tracks.FirstOrDefault (t => t.MediaType == AVMediaType.Audio);
			return firstAudioTrack;
		}

		static string BuildTimeString (CMTime time, string sign = null)
		{
			sign = sign ?? string.Empty;
			var seconds = Math.Round (time.Seconds);

			int hh = (int)Math.Floor (seconds / 3600);
			int mm = (int)Math.Floor ((seconds - hh * 3600) / 60);
			int ss = (int)seconds % 60;

			return hh > 0
				? string.Format ("{0}{1:00}:{2:00}:{3:00}", sign, hh, mm, ss)
				: string.Format ("{0}{1:00}:{2:00}", sign, mm, ss);
		}
	}
}