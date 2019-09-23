/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This class demonstrates how to use the scene delegate to configure a scene's interface.
 It also implements basic state restoration.
*/

using System;

using Foundation;
using UIKit;

namespace Gallery {
	[Register ("SceneDelegate")]
	public class SceneDelegate : UIWindowSceneDelegate {
		public override UIWindow Window { get; set; }

		#region UIWindowScene Delegate

		public override void WillConnect (UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
		{
			var userActivities = connectionOptions.UserActivities?.ToArray () ?? new NSUserActivity [0];
			var userActivity = userActivities.Length > 0 ? userActivities [0] : session.StateRestorationActivity;
			if (userActivity == null) {
				// If there were no user activities, we don't have to do anything.
				// The `Window` property will automatically be loaded with the storyboard's initial view controller.
				return;
			}

			if (!Configure (Window, userActivity))
				Console.WriteLine ($"Failed to restore from {userActivity}");
		}

		public override NSUserActivity GetStateRestorationActivity (UIScene scene) => scene.UserActivity;

		#endregion

		#region Utilities

		bool Configure (UIWindow window, NSUserActivity activity)
		{
			if (activity.Title == GalleryOpenDetailData.DetailPath) {
				if (activity.UserInfo [GalleryOpenDetailData.PhotoIdKey] is NSString photoId) {
					if (PhotoDetailViewController.LoadFromStoryboard () is PhotoDetailViewController photoDetailViewController) {
						photoDetailViewController.Photo = new Photo { Name = photoId };

						if (window.RootViewController is UINavigationController navigationController) {
							navigationController.PushViewController (photoDetailViewController, true);
							return true;
						}
					}
				}
			}

			return false;
		}

		#endregion
	}
}
