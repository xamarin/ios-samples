using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.IO;

namespace TicTacToe
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		TTTProfile profile;

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			UINavigationBar.Appearance.SetBackgroundImage (
				UIImage.FromBundle ("navigationBarBackground"), UIBarMetrics.Default);
			UINavigationBar.Appearance.SetTitleTextAttributes (
				new UITextAttributes () { TextColor = UIColor.Black });
			profile = loadProfileWithPath (profilePath);

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			UIViewController viewController1 = TTTPlayViewController.FromProfile (profile, profilePath);
			UIViewController viewController2 = TTTMessagesViewController.FromProfile (profile, profilePath);
			UIViewController viewController3 = TTTProfileViewController.FromProfile (profile, profilePath);
			UITabBarController tabBarController = new UITabBarController ();
			tabBarController.TabBar.BackgroundImage = UIImage.FromBundle ("barBackground");
			tabBarController.ViewControllers = new [] {
				viewController1,
				viewController2,
				viewController3
			};
			window.RootViewController = tabBarController;

			updateTintColor ();

			window.MakeKeyAndVisible ();

			NSNotificationCenter.DefaultCenter.AddObserver ((NSString)TTTProfile.IconDidChangeNotification,
			                                                iconDidChange);

			return true;
		}

		string profilePath {
			get {
				return Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), 
				                            "Profile.ttt");
			}
		}

		TTTProfile loadProfileWithPath (string path)
		{
			TTTProfile profile = TTTProfile.FromPath (path);
			if (profile == null)
				profile = new TTTProfile ();

			return profile;
		}

		void updateTintColor ()
		{
			if (profile.Icon == TTTProfileIcon.X)
				window.TintColor = UIColor.FromHSBA (0f, 1f, 1f, 1f);
			else
				window.TintColor = UIColor.FromHSBA (1f / 3f, 1f, 0.8f, 1f);
		}

		void iconDidChange (NSNotification notification)
		{
			updateTintColor ();
		}
	}
}

