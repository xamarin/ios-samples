using System;
using Foundation;
using MediaPlayer;
namespace MusicKitSample.Controllers
{
	public class MusicPlayerManager : NSObject
	{
		#region Types

		// Notification that is fired when there is an update to the 
		// playback state or currently playing asset in 
		// `MPMusicPlayerController`.
		public static readonly NSString DidUpdateState = new NSString ("didUpdateState");

		#endregion

		#region Fields

		NSObject nowPlayingItemDidChangeNotificationToken;
		NSObject playbackStateDidChangeNotificationChangeToken;

		#endregion

		#region Properties

		/* 
		 * The instance of `MPMusicPlayerController` that is used for 
		 * playing back titles from either the device media library
		 * or from the Apple Music Catalog.
		 */
		public MPMusicPlayerController MusicPlayerController { get; } = MPMusicPlayerController.SystemMusicPlayer;

		#endregion

		#region Constructors

		public MusicPlayerManager ()
		{
			/*
			 * It is important to call 
			 * `MPMusicPlayerController.BeginGeneratingPlaybackNotifications()` 
			 * so that playback notifications are generated and other parts 
			 * of the can update their state if needed.
			 */
			MusicPlayerController.BeginGeneratingPlaybackNotifications ();

			var notificationCenter = NSNotificationCenter.DefaultCenter;
			nowPlayingItemDidChangeNotificationToken = notificationCenter.AddObserver (MPMusicPlayerController.NowPlayingItemDidChangeNotification,
			                                                                           HandleMusicPlayerControllerNowPlayingItemDidChange,
			                                                                           null);
			playbackStateDidChangeNotificationChangeToken = notificationCenter.AddObserver (MPMusicPlayerController.PlaybackStateDidChangeNotification,
			                                                                                HandleMusicPlayerControllerPlaybackStateDidChange,
			                                                                                null);
		}

		#endregion

		#region Object Life Cycle

		protected override void Dispose (bool disposing)
		{
			/*
			 * It is important to call 
			 * `MPMusicPlayerController.endGeneratingPlaybackNotifications()` 
			 * so that playback notifications are no longer generated.
			 */
			MusicPlayerController.EndGeneratingPlaybackNotifications ();

			// Remove all notification observers.
			var notificationCenter = NSNotificationCenter.DefaultCenter;
			notificationCenter.RemoveObserver (nowPlayingItemDidChangeNotificationToken);
			notificationCenter.RemoveObserver (playbackStateDidChangeNotificationChangeToken);

			base.Dispose (disposing);
		}

		#endregion

		#region Playback Loading Methods

		public void BeginPlayback (MPMediaItemCollection itemCollection)
		{
			MusicPlayerController.SetQueue (itemCollection);
			MusicPlayerController.Play ();
		}

		public void BeginPlayback (string itemId)
		{
			MusicPlayerController.SetQueue (new [] { itemId });
			MusicPlayerController.Play ();
		}

		#endregion

		#region Playback Control Methods

		public void TogglePlayPause ()
		{
			if (MusicPlayerController.PlaybackState == MPMusicPlaybackState.Playing)
				MusicPlayerController.Pause ();
			else
				MusicPlayerController.Play ();
		}

		public void SkipToNextItem () => MusicPlayerController.SkipToNextItem ();

		public void SkipBackToBeginningOrPreviousItem ()
		{
			if (MusicPlayerController.CurrentPlaybackTime < 5) {
				// If the currently playing `MPMediaItem` is less than 5 seconds
				// into playback then skip to the previous item.
				MusicPlayerController.SkipToPreviousItem ();
			} else {
				// Otherwise skip back to the beginning of the currently 
				// playing `MPMediaItem`.
				MusicPlayerController.SkipToBeginning ();
			}
		}

		#endregion

		#region Notification Observing Methods

		void HandleMusicPlayerControllerNowPlayingItemDidChange (NSNotification notification) =>
		NSNotificationCenter.DefaultCenter.PostNotificationName (DidUpdateState, null);

		void HandleMusicPlayerControllerPlaybackStateDidChange (NSNotification notification) =>
		NSNotificationCenter.DefaultCenter.PostNotificationName (DidUpdateState, null);

		#endregion
	}
}
