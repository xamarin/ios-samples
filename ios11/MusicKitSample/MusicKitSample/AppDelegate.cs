using Foundation;
using UIKit;
using MusicKitSample.Controllers;
using System;

namespace MusicKitSample
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window {
			get;
			set;
		}

		// The instance of `AppleMusicManager` which handles making web
		// service calls to Apple Music Web Services.
		public AppleMusicManager AppleMusicManager { get; private set; }

		// The instance of `MusicPlayerManager` which handles media playback.
		public MusicPlayerManager MusicPlayerManager { get; private set; }

		// The instance of `AuthorizationManager` which is responsible 
		// for managing authorization for the application.
		public AuthorizationManager AuthorizationManager { get; private set; }

		// The instance of `MediaLibraryManager` which manages the 
		// `MPPMediaPlaylist` this application creates.
		public MediaLibraryManager MediaLibraryManager { get; private set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			AppleMusicManager = new AppleMusicManager ();
			MusicPlayerManager = new MusicPlayerManager ();
			AuthorizationManager = new AuthorizationManager (AppleMusicManager);
			MediaLibraryManager = new MediaLibraryManager (AuthorizationManager);

			if (TopViewController (0) is AuthorizationTableViewController authorizationTableViewController)
				authorizationTableViewController.AuthorizationManager = AuthorizationManager;
			else
				throw new InvalidCastException ($"Unable to find expected {nameof (AuthorizationTableViewController)} in at TabBar Index 0");

			if (TopViewController (1) is PlaylistTableViewController playlistTableViewController) {
				playlistTableViewController.AuthorizationManager = AuthorizationManager;
				playlistTableViewController.MediaLibraryManager = MediaLibraryManager;
				playlistTableViewController.MusicPlayerManager = MusicPlayerManager;
			} else
				throw new InvalidCastException ($"Unable to find expected {nameof (PlaylistTableViewController)} in at TabBar Index 1");

			if (TopViewController (2) is PlayerViewController playerViewController)
				playerViewController.MusicPlayerManager = MusicPlayerManager;
			else
				throw new InvalidCastException ($"Unable to find expected {nameof (PlayerViewController)} in at TabBar Index 2");

			if (TopViewController (3) is RecentlyPlayedTableViewController recentlyPlayedTableViewController) {
				recentlyPlayedTableViewController.AuthorizationManager = AuthorizationManager;
				recentlyPlayedTableViewController.AppleMusicManager = AppleMusicManager;
				recentlyPlayedTableViewController.MediaLibraryManager = MediaLibraryManager;
				recentlyPlayedTableViewController.MusicPlayerManager = MusicPlayerManager;
			} else
				throw new InvalidCastException ($"Unable to find expected {nameof (RecentlyPlayedTableViewController)} in at TabBar Index 3");
			
			if (TopViewController (4) is MediaSearchTableViewController mediaSearchTableViewController) {
				mediaSearchTableViewController.AuthorizationManager = AuthorizationManager;
				mediaSearchTableViewController.MediaLibraryManager = MediaLibraryManager;
				mediaSearchTableViewController.MusicPlayerManager = MusicPlayerManager;
			} else
				throw new InvalidCastException ($"Unable to find expected {nameof (MediaSearchTableViewController)} in at TabBar Index 4");

			return true;
		}

		UIViewController TopViewController (int index)
		{
			if (Window.RootViewController is UITabBarController tabBarController &&
			    tabBarController.ViewControllers [index] is UINavigationController navigationController)
				return navigationController.TopViewController;
			else
				throw new InvalidCastException ("Unable to find expected View Controller in Main.storyboard.");
		}
	}
}

