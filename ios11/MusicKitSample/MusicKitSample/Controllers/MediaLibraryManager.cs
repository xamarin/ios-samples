using System;
using Foundation;
using MediaPlayer;
using System.Threading.Tasks;
using UIKit;
namespace MusicKitSample.Controllers {
	public class MediaLibraryManager : NSObject {
		#region Types

		// The Key for the `UserDefaults` value representing the UUID of
		// the Playlist this sample creates.
		public static readonly string playlistUuidKey = "playlistUUIDKey";

		// Notification that is posted whenever the contents of the 
		// device's Media Library changed.
		public static readonly NSString LibraryDidUpdate = new NSString ("libraryDidUpdate");

		#endregion

		#region Fields

		NSObject authorizationDidUpdateNotificationToken;
		NSObject didChangeNotificationToken;
		NSObject willEnterForegroundNotificationToken;

		#endregion

		#region Properties

		// The instance of `AuthorizationManager` used for looking up 
		// the current device's Media Library and Cloud Services 
		// authorization status.
		public AuthorizationManager AuthorizationManager { get; set; }

		// The instance of `MPMediaPlaylist` that corresponds to the 
		// playlist created by this sample in the current device's Media
		// Library.
		public MPMediaPlaylist MediaPlaylist { get; set; }

		#endregion

		#region Constructors

		public MediaLibraryManager (AuthorizationManager authorizationManager)
		{
			AuthorizationManager = authorizationManager;

			// Add the notification observers needed to respond to 
			// events from the `AuthorizationManager`, 
			// `MPMediaLibrary` and `UIApplication`.
			var notificationCenter = NSNotificationCenter.DefaultCenter;
			authorizationDidUpdateNotificationToken = notificationCenter.AddObserver (AuthorizationManager.AuthorizationDidUpdateNotification,
																					  HandleAuthorizationManagerAuthorizationDidUpdateNotification,
																					  null);
			didChangeNotificationToken = notificationCenter.AddObserver (MPMediaLibrary.DidChangeNotification,
																		 HandleMediaLibraryDidChangeNotification,
																		 null);
			willEnterForegroundNotificationToken = notificationCenter.AddObserver (UIApplication.WillEnterForegroundNotification,
																				   HandleMediaLibraryDidChangeNotification,
																				   null);

			HandleAuthorizationManagerAuthorizationDidUpdateNotification (null);
		}

		#endregion

		#region Object Life Cycle

		protected override void Dispose (bool disposing)
		{
			// Remove all notification observers.
			var notificationCenter = NSNotificationCenter.DefaultCenter;
			notificationCenter.RemoveObserver (authorizationDidUpdateNotificationToken);
			notificationCenter.RemoveObserver (didChangeNotificationToken);
			notificationCenter.RemoveObserver (willEnterForegroundNotificationToken);

			authorizationDidUpdateNotificationToken = didChangeNotificationToken = willEnterForegroundNotificationToken = null;

			base.Dispose (disposing);
		}

		#endregion

		#region Private Functionality

		async Task CreatePlaylistIfNeededAsync ()
		{
			if (MediaPlaylist != null)
				return;

			// To create a new playlist or lookup a playlist there 
			// are several steps you need to do.
			NSUuid playlistUuid;
			MPMediaPlaylistCreationMetadata playlistCreationMetadata = null;
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			if (userDefaults.StringForKey (playlistUuidKey) is string playlistUuidString) {
				// In this case, the sample already created a playlist in
				// a previous run. In this case we lookup the UUID that
				// was used before.
				playlistUuid = new NSUuid (playlistUuidString);
			} else {
				// Create an instance of `UUID` to identify the new playlist.
				playlistUuid = new NSUuid ();

				// Create an instance of `MPMediaPlaylistCreationMetadata`, 
				// this represents the metadata to associate with the new playlist.
				playlistCreationMetadata = new MPMediaPlaylistCreationMetadata ("Test Playlist") {
					DescriptionText = $"This playlist was created using {NSBundle.MainBundle.InfoDictionary ["CFBundleName"]} to demonstrate how to use the Apple Music APIs"
				};

				// Store the `UUID` that the sample will use for looking 
				// up the playlist in the future.
				userDefaults.SetString (playlistUuid.AsString (), playlistUuidKey);
				userDefaults.Synchronize ();
			}

			// Request the new or existing playlist from the device.
			MediaPlaylist = await MPMediaLibrary.DefaultMediaLibrary.GetPlaylistAsync (playlistUuid, playlistCreationMetadata);

			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (LibraryDidUpdate, null));
		}

		#endregion

		#region Playlist Modification Method

		public async Task AddItemAsync (string id)
		{
			if (MediaPlaylist == null)
				throw new ArgumentNullException (nameof (MediaPlaylist), "Playlist has not been created");

			await MediaPlaylist.AddItemAsync (id);

			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (LibraryDidUpdate, null));
		}

		#endregion

		#region Notification Observing Methods

		void HandleAuthorizationManagerAuthorizationDidUpdateNotification (NSNotification notification)
		{
			if (MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.Authorized)
				Task.Factory.StartNew (async () => await CreatePlaylistIfNeededAsync ());
		}

		void HandleMediaLibraryDidChangeNotification (NSNotification notification)
		{
			if (MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.Authorized)
				Task.Factory.StartNew (async () => {
					await CreatePlaylistIfNeededAsync ();
					InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (LibraryDidUpdate, null));
				});
		}

		#endregion
	}
}
