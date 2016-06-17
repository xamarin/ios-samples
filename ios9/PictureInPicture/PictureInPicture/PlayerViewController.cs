using System;

using AVFoundation;
using AVKit;
using CoreFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace PictureInPicture
{
	// Manages the view used for playback and sets up the `AVPictureInPictureController` for video playback in picture in picture.
	public partial class PlayerViewController : UIViewController, IAVPictureInPictureControllerDelegate
	{
		IDisposable durationObserver;
		IDisposable rateObserver;
		IDisposable statusObserver;

		// Attempt to load and test these asset keys before playing
		static readonly NSString[] assetKeysRequiredToPlay = {
			(NSString)"playable",
			(NSString)"hasProtectedContent"
		};

		AVPictureInPictureController pictureInPictureController;
		NSObject timeObserverToken;

		AVPlayer player;
		AVPlayer Player {
			get {
				player = player ?? new AVPlayer ();
				return player;
			}
		}

		PlayerView PlayerView {
			get {
				return (PlayerView)View;
			}
		}

		AVPlayerLayer PlayerLayer {
			get {
				return PlayerView.PlayerLayer;
			}
		}

		AVPlayerItem playerItem;
		AVPlayerItem PlayerItem {
			get {
				return playerItem;
			}
			set {
				playerItem = value;

				// If needed, configure player item here before associating it with a player
				// (example: adding outputs, setting text style rules, selecting media options)
				Player.ReplaceCurrentItemWithPlayerItem (playerItem);
				if (playerItem == null)
					CleanUpPlayerPeriodicTimeObserver ();
				else
					SetupPlayerPeriodicTimeObserver ();
			}
		}

		double CurrentTime {
			get {
				return Player.CurrentTime.Seconds;
			}
			set {
				var newTime = CMTime.FromSeconds (value, 1);
				Player.Seek (newTime, CMTime.Zero, CMTime.Zero);
			}
		}

		double Duration {
			get {
				if (Player == null)
					return 0;

				var currentItem = player.CurrentItem;
				return currentItem.Duration.Seconds;
			}
		}

		[Outlet]
		UISlider TimeSlider { get; set; }

		[Outlet]
		UIBarButtonItem PlayPauseButton { get; set; }

		[Outlet]
		UIBarButtonItem PictureInPictureButton { get; set; }

		[Outlet]
		UIToolbar Toolbar { get; set; }

		[Outlet("pictureInPictureActiveLabel")]
		UILabel PictureInPictureActiveLabel { get; set; }

		public PlayerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Action("playPauseButtonWasPressed:")]
		void PlayPauseButtonWasPressed(UIButton sender)
		{
			PlayPauseButtonWasPressed ();
		}

		void PlayPauseButtonWasPressed (object sender, EventArgs e)
		{
			PlayPauseButtonWasPressed ();
		}

		void PlayPauseButtonWasPressed ()
		{
			if (Math.Abs (Player.Rate - 1) > float.Epsilon) {
				// Not playing foward, so play.
				if (Math.Abs (CurrentTime - Duration) < float.Epsilon) {
					// At end, so got back to beginning.
					CurrentTime = 0;
				}

				Player.Play ();
			} else {
				// Playing, so pause.
				Player.Pause ();
			}
		}

		[Action("togglePictureInPictureMode:")]
		void togglePictureInPictureMode(UIButton sender)
		{
			// Toggle picture in picture mode.
			// If active, stop picture in picture and return to inline playback.
			// If not active, initiate picture in picture.
			// Both these calls will trigger delegate callbacks which should be used
			// to set up UI appropriate to the state of the application.
			if (pictureInPictureController.PictureInPictureActive)
				pictureInPictureController.StopPictureInPicture ();
			else
				pictureInPictureController.StartPictureInPicture ();
		}

		[Action("timeSliderDidChange:")]
		void timeSliderDidChange(UISlider sender)
		{
			CurrentTime = sender.Value;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Update the UI when these player properties change.
			//
			// Use the context parameter to distinguish KVO for our particular observers
			// and not those destined for a subclass that also happens
			// to be observing these properties.
			var options = NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial;
			durationObserver = Player.AddObserver ("currentItem.duration", options, ObserveCurrentItemDuration);
			rateObserver = Player.AddObserver ("rate", options, ObserveCurrentRate);
			statusObserver = Player.AddObserver ("currentItem.status", options, ObserveCurrentItemStatus);

			PlayerView.PlayerLayer.Player = Player;

			SetupPlayback ();

			TimeSlider.TranslatesAutoresizingMaskIntoConstraints = true;
			TimeSlider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			Player.Pause ();
			CleanUpPlayerPeriodicTimeObserver ();

			rateObserver.Dispose ();
			durationObserver.Dispose ();
			statusObserver.Dispose ();
		}

		void SetupPlayback()
		{
			var movieURL = NSBundle.MainBundle.GetUrlForResource ("samplemovie", "mov");
			var asset = new AVUrlAsset (movieURL, (AVUrlAssetOptions)null);

			// Create a new `AVPlayerItem` and make it our player's current item.
			//
			// Using `AVAsset` now runs the risk of blocking the current thread (the
			// main UI thread) whilst I/O happens to populate the properties. It's prudent
			// to defer our work until the properties we need have been loaded.
			//
			// These properties can be passed in at initialization to `AVPlayerItem`,
			// which are then loaded automatically by `AVPlayer`.
			PlayerItem = new AVPlayerItem (asset, assetKeysRequiredToPlay);
		}

		void SetupPlayerPeriodicTimeObserver()
		{
			// Only add the time observer if one hasn't been created yet.
			if (timeObserverToken != null)
				return;

			var time = new CMTime (1, 1);
			timeObserverToken = Player.AddPeriodicTimeObserver (time, DispatchQueue.MainQueue, t => TimeSlider.Value = (float)t.Seconds);
		}

		void CleanUpPlayerPeriodicTimeObserver ()
		{
			if (timeObserverToken == null)
				return;

			Player.RemoveTimeObserver (timeObserverToken);
			timeObserverToken = null;
		}

		void SetupPictureInPicturePlayback ()
		{
			// Check to make sure Picture in Picture is supported for the current
			// setup (application configuration, hardware, etc.).

			if (AVPictureInPictureController.IsPictureInPictureSupported) {
				// Create `AVPictureInPictureController` with our `PlayerLayer`.
				// Set this as Delegate to receive callbacks for picture in picture events.
				// Add observer to be notified when PictureInPicturePossible changes value,
				// so that we can enable `PictureInPictureButton`.
				pictureInPictureController = new AVPictureInPictureController (PlayerView.PlayerLayer);
				pictureInPictureController.Delegate = this;
				pictureInPictureController.AddObserver (this, "pictureInPicturePossible", NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, IntPtr.Zero);
			} else {
				PictureInPictureButton.Enabled = false;
			}
		}

		#region IAVPictureInPictureControllerDelegate

		[Export ("pictureInPictureControllerDidStartPictureInPicture:")]
		public void DidStartPictureInPicture (AVPictureInPictureController pictureInPictureController)
		{
			// If your application contains a video library or other interesting views,
			// this delegate callback can be used to dismiss player view controller
			// and to present the user with a selection of videos to play next.
			PictureInPictureActiveLabel.Hidden = false;
			Toolbar.Hidden = true;
		}

		[Export ("pictureInPictureControllerWillStopPictureInPicture:")]
		public void WillStopPictureInPicture (AVPictureInPictureController pictureInPictureController)
		{
			// Picture in picture mode will stop soon, hide the active label and show the toolbar.
			PictureInPictureActiveLabel.Hidden = true;
			Toolbar.Hidden = false;
		}

		[Export ("pictureInPictureControllerFailedToStartPictureInPicture:withError:")]
		public void FailedToStartPictureInPicture (AVPictureInPictureController pictureInPictureController, NSError error)
		{
			// Picture in picture failed to start with an error, restore UI to continue
			// inline playback. Hide the active label and show the toolbar.
			PictureInPictureActiveLabel.Hidden = true;
			Toolbar.Hidden = false;
			HandleError (error);
		}

		void ObserveCurrentItemDuration (NSObservedChange change)
		{
			CMTime newDuration;
			var newDurationAsValue = change.NewValue as NSValue;
			newDuration = (newDurationAsValue != null) ? newDurationAsValue.CMTimeValue : CMTime.Zero;
			var hasValidDuration = newDuration.IsNumeric && newDuration.Value != 0;
			var newDurationSeconds = hasValidDuration ? newDuration.Seconds : 0;

			TimeSlider.MaxValue = (float)newDurationSeconds;

			var currentTime = Player.CurrentTime.Seconds;
			TimeSlider.Value = (float)(hasValidDuration ? currentTime : 0);

			PlayPauseButton.Enabled = hasValidDuration;
			TimeSlider.Enabled = hasValidDuration;
		}

		void ObserveCurrentRate (NSObservedChange change)
		{
			// Update playPauseButton type.
			var newRate = ((NSNumber)change.NewValue).DoubleValue;

			UIBarButtonSystemItem style = (Math.Abs(newRate) < float.Epsilon) ? UIBarButtonSystemItem.Play : UIBarButtonSystemItem.Pause;
			var newPlayPauseButton = new UIBarButtonItem(style, PlayPauseButtonWasPressed);

			// Replace the current button with the updated button in the toolbar.
			UIBarButtonItem[] items = Toolbar.Items;

			var playPauseItemIndex = Array.IndexOf(items, PlayPauseButton);
			if (playPauseItemIndex >= 0) {
				items[playPauseItemIndex] = newPlayPauseButton;
				PlayPauseButton = newPlayPauseButton;
				Toolbar.SetItems(items, false);
			}
		}

		void ObserveCurrentItemStatus (NSObservedChange change)
		{
			// Display an error if status becomes Failed
			var newStatusAsNumber = change.NewValue as NSNumber;
			AVPlayerItemStatus newStatus = (newStatusAsNumber != null)
				? (AVPlayerItemStatus)newStatusAsNumber.Int32Value
				: AVPlayerItemStatus.Unknown;

			if (newStatus == AVPlayerItemStatus.Failed) {
				HandleError(Player.CurrentItem.Error);
			} else if (newStatus == AVPlayerItemStatus.ReadyToPlay) {
				var asset = Player.CurrentItem != null ? Player.CurrentItem.Asset : null;
				if (asset != null) {

					// First test whether the values of `assetKeysRequiredToPlay` we need
					// have been successfully loaded.
					foreach (var key in assetKeysRequiredToPlay) {
						NSError error;
						if (asset.StatusOfValue(key, out error) == AVKeyValueStatus.Failed) {
							HandleError(error);
							return;
						}
					}

					if (!asset.Playable || asset.ProtectedContent) {
						// We can't play this asset.
						HandleError(null);
						return;
					}

					// The player item is ready to play,
					// setup picture in picture.
					SetupPictureInPicturePlayback ();
				}
			}
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (context != IntPtr.Zero) {
				base.ObserveValue (keyPath, ofObject, change, context);
				return;
			}

			var ch = new NSObservedChange (change);

			if (keyPath == "pictureInPicturePossible") {
				// Enable the `PictureInPictureButton` only if `PictureInPicturePossible`
				// is true. If this returns false, it might mean that the application
				// was not configured as shown in the AppDelegate.
				PictureInPictureButton.Enabled = ((NSNumber)ch.NewValue).BoolValue;
			}
		}

		void HandleError (NSError error)
		{
			string message = error != null ? error.LocalizedDescription : string.Empty;
			var alertController = UIAlertController.Create ("Error", message, UIAlertControllerStyle.Alert);
			var alertAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null);
			alertController.AddAction (alertAction);
			PresentViewController (alertController, true, null);
		}

		#endregion
	}
}