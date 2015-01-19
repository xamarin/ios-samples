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

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("FinishedLaunching");

			AppConfig.SharedAppConfiguration.RunHandlerOnFirstLaunch(()=> {
				ListCoordinator.SharedListCoordinator.CopyInitialDocuments();
			});

			var splitViewController = (UISplitViewController)Window.RootViewController;
			splitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;
			splitViewController.WeakDelegate = this;

			return true;
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
			splitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.Automatic;

			// If there's a list that's currently selected in separated mode and we want to show it in collapsed mode, we'll transfer over the view controller's settings.
			var secondaryNavigationController = secondaryViewController as UINavigationController;
			if (secondaryNavigationController != null) {
				UIStringAttributes textAttributes = secondaryNavigationController.NavigationBar.TitleTextAttributes;
				PrimaryViewController.NavigationBar.TitleTextAttributes = textAttributes;
				PrimaryViewController.NavigationBar.TintColor = secondaryNavigationController.NavigationBar.TintColor;
				PrimaryViewController.Toolbar.TintColor = secondaryNavigationController.Toolbar.TintColor;

				PrimaryViewController.ShowDetailViewController (secondaryNavigationController.TopViewController, null);
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
	}
}
