using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

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

		ListsController ListsController { get; set; }

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
			PrimaryViewController.PopViewController (true);
			SetupUserStoragePreferences ();
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
			// In this delegate method, the reverse of the collapsing procedure described above needs to be
			// carried out if a list is being displayed. The appropriate controller to display in the detail area
			// should be returned. If not, the standard behavior is obtained by returning null.
			var secondaryVC = PrimaryViewController.TopViewController as UINavigationController;
			if (secondaryVC != null) {
				var listViewController = secondaryVC.TopViewController as ListViewController;
				if (listViewController != null) {
					PrimaryViewController.PopViewController(false);

					// Obtain the `textAttributes` and `tintColor` to setup the separated navigation controller.
					var textAttributes = listViewController.TextAttributes;
					UIColor tintColor = AppColors.ColorFrom (listViewController.Document.ListPresenter.Color);

					// Transfer the settings for the `navigationBar` and the `toolbar` to the detail navigation controller.
					secondaryVC.NavigationBar.TitleTextAttributes = textAttributes;
					secondaryVC.NavigationBar.TintColor = tintColor;
					secondaryVC.Toolbar.TintColor = tintColor;

					listViewController.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
					return secondaryVC;
				}
			}

			return null;
		}

		#endregion

		void SetupUserStoragePreferences()
		{
			StorageState storageState = AppConfig.SharedAppConfiguration.StorageState;

			// Check to see if the account has changed since the last time the method was called. If it has, let the
			// user know that their documents have changed. If they've already chosen local storage (i.e. not iCloud),
			// don't notify them since there's no impact.

			if (storageState.AccountDidChange) {
				NotifyUserOfAccountChange (storageState);
				// Return early. State resolution will take place after the user acknowledges the change.
				return;
			}

			ResolveStateForUserStorageState (storageState);
		}

		void ResolveStateForUserStorageState(StorageState storageState)
		{
			if (storageState.CloudAvailable) {
				bool changedToLocal = storageState.StorageOption == StorageType.Local && storageState.AccountDidChange;
				if (storageState.StorageOption == StorageType.NotSet || changedToLocal) // iCloud is available, but we need to ask the user what they prefer.
					PromptUserForStorageOption ();
				else // The user has already selected a specific storage option. Set up the lists controller to use that storage option.
					ConfigureListsController (storageState.AccountDidChange, null);
			} else {
				// iCloud is not available, so we'll reset the storage option and configure the lists controller. The
				// next time that the user signs in with an iCloud account, he or she can change provide their desired
				// storage option.

				if (storageState.StorageOption != StorageType.NotSet)
					AppConfig.SharedAppConfiguration.StorageOption = StorageType.NotSet;

				ConfigureListsController (storageState.AccountDidChange, null);
			}
		}

		#region Alerts

		void NotifyUserOfAccountChange(StorageState state)
		{
			// Copy a 'Today' list from the bundle to the local documents directory if a 'Today' list doesn't exist.
			// This provides more context for the user than no lists and ensures the user always has a 'Today' list (a
			// design choice made in Lister).

			if (!state.CloudAvailable)
				ListUtilities.CopyTodayList ();

			var title = "iCloud Sign Out";
			var message = "You have signed out of the iCloud account previously used to store documents. Sign back in to access those documents.";
			var okActionTitle = "OK";

			UIAlertController signedOutController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			UIAlertAction action = UIAlertAction.Create (okActionTitle, UIAlertActionStyle.Cancel, _ => {
				ResolveStateForUserStorageState (state);
			});
			signedOutController.AddAction(action);

			ListViewController.PresentViewController(signedOutController, true, null);
		}

		void PromptUserForStorageOption()
		{
			var title = "Choose Storage Option";
			var message = "Do you want to store documents in iCloud or only on this device?";
			var localOnlyActionTitle = "Local Only";
			var cloudActionTitle = "iCloud";

			UIAlertController storageController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			UIAlertAction localOption = UIAlertAction.Create (localOnlyActionTitle, UIAlertActionStyle.Default, _ => {
				AppConfig.SharedAppConfiguration.StorageOption = StorageType.Local;
				ConfigureListsController (true, null);
			});
			storageController.AddAction (localOption);

			UIAlertAction cloudOption = UIAlertAction.Create (cloudActionTitle, UIAlertActionStyle.Default, _ => {
				AppConfig.SharedAppConfiguration.StorageOption = StorageType.Cloud;

				ConfigureListsController (true, null);
				ListUtilities.MigrateLocalListsToCloud ();
			});
			storageController.AddAction (cloudOption);

			ListViewController.PresentViewController (storageController, true, null);
		}

		#endregion

		void ConfigureListsController(bool accountChanged, Action storageOptionChangeHandler)
		{
			// The current controller is correct. There is no need to reconfigure it.
			if (ListsController != null && !accountChanged)
				return;

			if (ListsController == null) {
				// There is currently no lists controller. Configure an appropriate one for the current configuration.
				ListsController = AppConfig.SharedAppConfiguration.ListsControllerForCurrentConfigurationWithPathExtension (AppConfig.ListerFileExtension, storageOptionChangeHandler);

				// Ensure that this controller is passed along to the `AAPLListDocumentsViewController`.
				ListViewController.ListsController = ListsController;
				ListsController.StartSearching ();
			} else if (accountChanged) {
				// A lists controller is configured; however, it needs to have its coordinator updated based on the account change. 
				ListsController.ListCoordinator = AppConfig.SharedAppConfiguration.ListsCoordinatorForCurrentConfigurationWithPathExtension (AppConfig.ListerFileExtension, storageOptionChangeHandler);
			}
		}
	}
}
