using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Common;
using ListerKit;

namespace Lister
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate
	{
		const string MainStoryboardName = "Main";
		const string MainStoryboardEmptyViewControllerIdentifier = "emptyViewController";

		NSObject subscribtionToken;

		public override UIWindow Window { get; set; }

		UISplitViewController SplitViewController {
			get {
				return (UISplitViewController)Window.RootViewController;
			}
		}

		UINavigationController PrimaryViewController {
			get {
				return (UINavigationController)SplitViewController.ViewControllers [0];
			}
		}

		ListViewController ListViewController {
			get {
				return (ListViewController)PrimaryViewController.ViewControllers [0];
			}
		}

		UIViewController ListsController { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("FinishedLaunching");

			subscribtionToken = NSFileManager.Notifications.ObserveUbiquityIdentityDidChange (OnUbiquityIdentityChanged);

			AppConfig.SharedAppConfiguration.RunHandlerOnFirstLaunch(()=> {
				ListCoordinator.SharedListCoordinator.CopyInitialDocuments();
			});

			SplitViewController.WeakDelegate = this;
			SplitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;

			// Configure the detail controller in the `UISplitViewController` at the root of the view hierarchy.
			var navigationController = (UINavigationController)SplitViewController.ViewControllers.Last ();
			var navItem = navigationController.TopViewController.NavigationItem;
			navItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
			navItem.LeftItemsSupplementBackButton = true;

			return true;
		}

		public override void OnActivated (UIApplication application)
		{
			SetupUserStoragePreferences ();
		}

		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			// Lister only supports a single user activity type; if you support more than one the type is available from the userActivity parameter.
			if (completionHandler == null || ListViewController == null)
				return false;

			completionHandler (new NSObject[]{ ListViewController });
			return true;
		}

		void OnUbiquityIdentityChanged(object sender, NSNotificationEventArgs e)
		{
		}

		#region UISplitViewControllerDelegate

		[Export ("targetDisplayModeForActionInSplitViewController:")]
		public UISplitViewControllerDisplayMode GetTargetDisplayModeForAction (UISplitViewController svc)
		{
			return UISplitViewControllerDisplayMode.AllVisible;
		}

		[Export ("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:")]
		public bool CollapseSecondViewController (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
		{
			// In a regular width size class, Lister displays a split view controller with a navigation controller
			// displayed in both the master and detail areas.
			// If there's a list that's currently selected, it should be on top of the stack when collapsed.
			// Ensuring that the navigation bar takes on the appearance of the selected list requires the
			// transfer of the configuration of the navigation controller that was shown in the detail area.

			var secondaryNavigationController = secondaryViewController as UINavigationController;
			if (secondaryNavigationController != null) {
				var top = secondaryNavigationController.TopViewController as ListViewController;
				if (top != null) {
					UIStringAttributes textAttributes = secondaryNavigationController.NavigationBar.TitleTextAttributes;
					PrimaryViewController.NavigationBar.TitleTextAttributes = textAttributes;
					PrimaryViewController.NavigationBar.TintColor = secondaryNavigationController.NavigationBar.TintColor;
					PrimaryViewController.Toolbar.TintColor = secondaryNavigationController.Toolbar.TintColor;

					return false;
				}
			}

			return true;
		}

		[Export ("splitViewController:separateSecondaryViewControllerFromPrimaryViewController:")]
		public UIViewController SeparateSecondaryViewController (UISplitViewController splitViewController, UIViewController primaryViewController)
		{
			if (PrimaryViewController.TopViewController == PrimaryViewController.ViewControllers[0]) {
				// If no list is on the stack, fill the detail area with an empty controller.
				UIStoryboard storyboard = UIStoryboard.FromName (MainStoryboardName, null);
				UIViewController emptyViewController = (UIViewController)storyboard.InstantiateViewController (MainStoryboardEmptyViewControllerIdentifier);

				return emptyViewController;
			}

			UIStringAttributes textAttributes = PrimaryViewController.NavigationBar.TitleTextAttributes;
			UIColor tintColor = PrimaryViewController.NavigationBar.TintColor;
			UIViewController poppedViewController = PrimaryViewController.PopViewController (false);

			UINavigationController navigationViewController = new UINavigationController (poppedViewController);
			navigationViewController.NavigationBar.TitleTextAttributes = textAttributes;
			navigationViewController.NavigationBar.TintColor = tintColor;
			navigationViewController.Toolbar.TintColor = tintColor;

			return navigationViewController;
		}

		#endregion

		#region User Storage Preferences

		void SetupUserStoragePreferences()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}
